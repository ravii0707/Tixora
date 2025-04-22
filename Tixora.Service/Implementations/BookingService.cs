using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tixora.Core.DTOs;
using Tixora.Core.Entities;
using Tixora.Repository.Interfaces;
using Tixora.Service.Exceptions;
using Tixora.Service.Interfaces;

namespace Tixora.Service.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IShowTimeRepository _showTimeRepository;
        private readonly IMapper _mapper;

        public BookingService(
            IBookingRepository bookingRepository,
            IUserRepository userRepository,
            IMovieRepository movieRepository,
            IShowTimeRepository showTimeRepository,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _movieRepository = movieRepository;
            _showTimeRepository = showTimeRepository;
            _mapper = mapper;
        }

        public async Task<BookingResponseDTO> CreateAsync(BookingCreateDTO bookingDto)
        {
            // Validate user exists
            var user = await _userRepository.GetByIdAsync(bookingDto.UserId);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            // Validate movie exists
            var movie = await _movieRepository.GetByIdAsync(bookingDto.MovieId);
            if (movie == null)
            {
                throw new NotFoundException("Movie not found");
            }

            // Validate showtime exists
            var showtime = await _showTimeRepository.GetByIdAsync(bookingDto.ShowtimeId);
            if (showtime == null)
            {
                throw new NotFoundException("Showtime not found");
            }

            // Check if showtime belongs to the movie
            if (showtime.MovieId != bookingDto.MovieId)
            {
                throw new BadRequestException("Showtime doesn't belong to the specified movie");
            }

            // Check available seats
            if (showtime.AvailableSeats < bookingDto.TicketCount)
            {
                throw new BadRequestException("Not enough available seats");
            }

            // Check for duplicate booking
            if (await _bookingRepository.ExistsAsync(bookingDto.UserId, bookingDto.ShowtimeId, bookingDto.MovieId))
            {
                throw new BadRequestException("Booking already exists for this user, showtime and movie");
            }

            // Create booking
            var booking = _mapper.Map<TbBookingHistory>(bookingDto);
            booking.TotalAmount = bookingDto.TicketCount * 200;
            var createdBooking = await _bookingRepository.AddAsync(booking);

            // Update available seats
            showtime.AvailableSeats -= bookingDto.TicketCount;
            await _showTimeRepository.UpdateAsync(showtime);

            return _mapper.Map<BookingResponseDTO>(createdBooking);
        }

        public async Task<BookingResponseDTO> GetByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                throw new NotFoundException("Booking not found");
            }

            return _mapper.Map<BookingResponseDTO>(booking);
        }

        public async Task<IEnumerable<BookingResponseDTO>> GetAllAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BookingResponseDTO>>(bookings);
        }

        public async Task<IEnumerable<BookingResponseDTO>> GetByUserIdAsync(int userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<BookingResponseDTO>>(bookings);
        }
    }
}
