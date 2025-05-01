using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tixora.Core.DTOs;
using Tixora.Core.Entities;
using Tixora.Repository.Interfaces;
using Tixora.Service.Exceptions;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<BookingService> _logger;

        public BookingService(
            IBookingRepository bookingRepository,
            IUserRepository userRepository,
            IMovieRepository movieRepository,
            IShowTimeRepository showTimeRepository,
            IMapper mapper,
            ILogger<BookingService> logger)
        {
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _movieRepository = movieRepository;
            _showTimeRepository = showTimeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BookingResponseDTO> CreateAsync(BookingCreateDTO bookingDto)
        {
            using var transaction = await _bookingRepository.BeginTransactionAsync();

            try
            {
                // 1. Validate User exists
                var user = await _userRepository.GetByIdAsync(bookingDto.UserId);
                if (user == null)
                {
                    _logger.LogWarning("User not found for booking: {UserId}", bookingDto.UserId);
                    throw new NotFoundException("User", bookingDto.UserId);
                }

                // 2. Validate Movie exists
                var movie = await _movieRepository.GetByIdAsync(bookingDto.MovieId);
                if (movie == null)
                {
                    _logger.LogWarning("Movie not found for booking: {MovieId}", bookingDto.MovieId);
                    throw new NotFoundException("Movie", bookingDto.MovieId);
                }

                // 3. Validate Showtime exists
                var showtime = await _showTimeRepository.GetByIdAsync(bookingDto.ShowtimeId);
                if (showtime == null)
                {
                    _logger.LogWarning("Showtime not found for booking: {ShowtimeId}", bookingDto.ShowtimeId);
                    throw new NotFoundException("Showtime", bookingDto.ShowtimeId);
                }

                // 4. Validate Showtime belongs to Movie
                if (showtime.MovieId != bookingDto.MovieId)
                {
                    _logger.LogWarning("Showtime {ShowtimeId} doesn't belong to movie {MovieId}",
                        bookingDto.ShowtimeId, bookingDto.MovieId);
                    throw new BadRequestException("The selected showtime doesn't match the specified movie.");
                }

                // 5. Validate Showtime is in future
                var showDateTime = showtime.ShowDate.ToDateTime(TimeOnly.Parse(showtime.ShowTime));
                if (showDateTime < DateTime.Now)
                {
                    _logger.LogWarning("Attempt to book past showtime: {ShowtimeId}", bookingDto.ShowtimeId);
                    throw new BadRequestException("Cannot book tickets for past showtimes.");
                }

                // 6. Validate available seats
                if (showtime.AvailableSeats < bookingDto.TicketCount)
                {
                    var errors = new Dictionary<string, string>
                    {
                        { "AvailableSeats", showtime.AvailableSeats.ToString() },
                        { "RequestedTickets", bookingDto.TicketCount.ToString() }
                    };
                    _logger.LogWarning("Not enough seats for showtime {ShowtimeId}. Available: {Available}, Requested: {Requested}",
                        bookingDto.ShowtimeId, showtime.AvailableSeats, bookingDto.TicketCount);
                    throw new BadRequestException("Not enough available seats for your booking.", errors);
                }

                // 7. Prevent duplicate bookings
                if (await _bookingRepository.ExistsAsync(bookingDto.UserId, bookingDto.ShowtimeId, bookingDto.MovieId))
                {
                    _logger.LogWarning("Duplicate booking attempt by user {UserId} for showtime {ShowtimeId}",
                        bookingDto.UserId, bookingDto.ShowtimeId);
                    throw new BadRequestException("You already have a booking for this show.");
                }

                // 8. Create booking
                var booking = _mapper.Map<TbBookingHistory>(bookingDto);
                booking.TotalAmount = CalculateTotal(bookingDto.TicketCount, showtime);
                booking.BookingDate = DateTime.Now;

                // 9. Save booking and update seats
                var createdBooking = await _bookingRepository.AddAsync(booking);
                showtime.AvailableSeats -= bookingDto.TicketCount;
                await _showTimeRepository.UpdateAsync(showtime);

                await transaction.CommitAsync();
                _logger.LogInformation("Booking created successfully with ID: {BookingId}", createdBooking.BookingId);

                // 10. Return enriched response
                var response = _mapper.Map<BookingResponseDTO>(createdBooking);
                return response;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
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
            {
                _logger.LogWarning("Booking not found with ID: {BookingId}", id);
                throw new NotFoundException("Booking", id);
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
            if (!bookings.Any())
            {
                _logger.LogInformation("No bookings found for user ID: {UserId}", userId);
                throw new NotFoundException("No bookings found for this user.");
            }

            return _mapper.Map<IEnumerable<BookingResponseDTO>>(bookings);
        }
    }
}