using AutoMapper;
using System;
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
            // 1. Validate User exists
            var user = await _userRepository.GetByIdAsync(bookingDto.UserId);
            if (user == null)
                throw new NotFoundException("User not found");

            // 2. Validate Movie exists
            var movie = await _movieRepository.GetByIdAsync(bookingDto.MovieId);
            if (movie == null)
                throw new NotFoundException("Movie not found");

            // 3. Validate Showtime exists
            var showtime = await _showTimeRepository.GetByIdAsync(bookingDto.ShowtimeId);
            if (showtime == null)
                throw new NotFoundException("Showtime not found");

            // 4. Validate Showtime belongs to Movie
            if (showtime.MovieId != bookingDto.MovieId)
                throw new BadRequestException("Showtime doesn't belong to the specified movie");

            // 5. Validate Showtime is in future
            var showDateTime = showtime.ShowDate.ToDateTime(TimeOnly.Parse(showtime.ShowTime));
            if (showDateTime < DateTime.Now)
                throw new BadRequestException("Cannot book tickets for past showtimes");

            // 6. Validate available seats
            if (showtime.AvailableSeats < bookingDto.TicketCount)
                throw new BadRequestException($"Not enough seats. Available: {showtime.AvailableSeats}");

            // 7. Prevent duplicate bookings
            if (await _bookingRepository.ExistsAsync(bookingDto.UserId, bookingDto.ShowtimeId, bookingDto.MovieId))
                throw new BadRequestException("You already have a booking for this show");

            // 8. Create booking
            var booking = _mapper.Map<TbBookingHistory>(bookingDto);
            booking.TotalAmount = CalculateTotal(bookingDto.TicketCount, showtime);
            booking.BookingDate = DateTime.Now;

            // 9. Save booking and update seats
            var createdBooking = await _bookingRepository.AddAsync(booking);
            showtime.AvailableSeats -= bookingDto.TicketCount;
            await _showTimeRepository.UpdateAsync(showtime);

            // 10. Return enriched response
            return _mapper.Map<BookingResponseDTO>(createdBooking);
        }

        private decimal CalculateTotal(int ticketCount, TbShowTime showtime)
        {
            // Replace with your actual pricing logic
            const decimal basePrice = 200m;
            return ticketCount * basePrice;
        }

        public async Task<BookingResponseDTO> GetByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                throw new NotFoundException("Booking not found");

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