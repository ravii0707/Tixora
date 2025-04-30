using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tixora.Core.DTOs;
using Tixora.Core.Entities;
using Tixora.Repository.Interfaces;
using Tixora.Service.Exceptions;
using Microsoft.Extensions.Logging;
using Tixora.Service.Interfaces;
using System.Globalization;

namespace Tixora.Service.Implementations
{
    public class ShowTimeService : IShowTimeService
    {
        private readonly IShowTimeRepository _showTimeRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ShowTimeService> _logger;

        // Business rule constants
        private const int MAX_DAYS_IN_ADVANCE = 4;
        private const int MAX_SHOWS_PER_DAY = 4;
        private const int MIN_GAP_BETWEEN_SHOWS_MINUTES = 180;

        public ShowTimeService(
            IShowTimeRepository showTimeRepository,
            IMovieRepository movieRepository,
            IMapper mapper,
            ILogger<ShowTimeService> logger)
        {
            _showTimeRepository = showTimeRepository;
            _movieRepository = movieRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ShowTimeResponseDTO> CreateAsync(ShowTimeCreateDTO showTimeDto)
        {
            try
            {
                // 1. Validate movie exists
                _logger.LogInformation("Validating movie exists for ID: {MovieId}", showTimeDto.MovieId);
                var movie = await _movieRepository.GetByIdAsync(showTimeDto.MovieId);
                if (movie == null)
                {
                    _logger.LogWarning("Movie not found with ID: {MovieId}", showTimeDto.MovieId);
                    throw new NotFoundException("Movie not found");
                }

                // 2. Validate date is not in the past
                var today = DateOnly.FromDateTime(DateTime.Today);
                if (showTimeDto.ShowDate < today)
                {
                    _logger.LogWarning("Attempt to create showtime in past date: {ShowDate}", showTimeDto.ShowDate);
                    throw new BadRequestException("Cannot create showtimes for past dates.");
                }

                // 3. Validate date is within allowed range (4 days)
                if (showTimeDto.ShowDate > today.AddDays(MAX_DAYS_IN_ADVANCE))
                {
                    _logger.LogWarning("Attempt to create showtime beyond {MAX_DAYS_IN_ADVANCE} days: {ShowDate}",
                        MAX_DAYS_IN_ADVANCE, showTimeDto.ShowDate);
                    throw new BadRequestException(
                        $"Showtime can only be scheduled up to {MAX_DAYS_IN_ADVANCE} days in advance.");
                }

                // 4. Parse and validate show time format
                _logger.LogInformation("Parsing show time: {ShowTime}", showTimeDto.ShowTime);
                var newShowTime = TimeOnly.ParseExact(showTimeDto.ShowTime, "HH:mm", CultureInfo.InvariantCulture);
                var newShowDateTime = showTimeDto.ShowDate.ToDateTime(newShowTime);

                // 5. Verify show is not in the past (including time)
                if (newShowDateTime < DateTime.Now)
                {
                    _logger.LogWarning("Attempt to create showtime in past: {DateTime}", newShowDateTime);
                    throw new BadRequestException("Cannot create showtimes in the past.");
                }

                // 6. Get existing shows for the same date
                _logger.LogInformation("Retrieving existing shows for date: {ShowDate}", showTimeDto.ShowDate);
                var existingShows = await _showTimeRepository.GetByDateAsync(showTimeDto.ShowDate);

                // 7. Validate maximum shows per day
                if (existingShows.Count() >= MAX_SHOWS_PER_DAY)
                {
                    _logger.LogWarning("Maximum shows per day reached for date: {ShowDate}", showTimeDto.ShowDate);
                    throw new BadRequestException($"Maximum {MAX_SHOWS_PER_DAY} shows allowed per day.");
                }

                // 8. Validate time gap between shows
                foreach (var existingShow in existingShows)
                {
                    var existingShowTime = TimeOnly.ParseExact(existingShow.ShowTime, "HH:mm", CultureInfo.InvariantCulture);
                    var existingDateTime = existingShow.ShowDate.ToDateTime(existingShowTime);

                    var timeDifference = Math.Abs((newShowDateTime - existingDateTime).TotalMinutes);
                    if (timeDifference < MIN_GAP_BETWEEN_SHOWS_MINUTES)
                    {
                        _logger.LogWarning("Insufficient time gap between shows. Existing: {ExistingTime}, New: {NewTime}",
                            existingShow.ShowTime, showTimeDto.ShowTime);
                        throw new BadRequestException(
                            $"There must be at least {MIN_GAP_BETWEEN_SHOWS_MINUTES} minutes gap between shows. " +
                            $"Conflict with existing show at {existingShow.ShowTime}");
                    }
                }

                // 9. Map and create the showtime
                _logger.LogInformation("Creating new showtime for movie {MovieId}", showTimeDto.MovieId);
                var showTimeEntity = _mapper.Map<TbShowTime>(showTimeDto);
                var createdShowTime = await _showTimeRepository.AddAsync(showTimeEntity);

                _logger.LogInformation("Successfully created showtime with ID: {ShowtimeId}", createdShowTime.ShowtimeId);
                return _mapper.Map<ShowTimeResponseDTO>(createdShowTime);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid time format provided: {ShowTime}", showTimeDto.ShowTime);
                throw new BadRequestException("Invalid time format. Use HH:mm in 24-hour format.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating showtime");
                throw new BadRequestException("Failed to create showtime. Please check your input and try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating showtime");
                throw;
            }
        }

        public async Task<ShowTimeResponseDTO> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching showtime with ID: {ShowtimeId}", id);
            var showTime = await _showTimeRepository.GetByIdAsync(id);

            if (showTime == null)
            {
                _logger.LogWarning("Showtime not found with ID: {ShowtimeId}", id);
                throw new NotFoundException("Showtime not found");
            }

            return _mapper.Map<ShowTimeResponseDTO>(showTime);
        }

        public async Task<IEnumerable<ShowTimeResponseDTO>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all showtimes");
            var showTimes = await _showTimeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ShowTimeResponseDTO>>(showTimes);
        }

        public async Task<IEnumerable<ShowTimeResponseDTO>> GetByMovieIdAsync(int movieId)
        {
            _logger.LogInformation("Fetching showtimes for movie ID: {MovieId}", movieId);

            // Validate movie exists first
            var movie = await _movieRepository.GetByIdAsync(movieId);
            if (movie == null)
            {
                _logger.LogWarning("Movie not found with ID: {MovieId}", movieId);
                throw new NotFoundException("Movie not found");
            }

            var showTimes = await _showTimeRepository.GetByMovieIdAsync(movieId);
            return _mapper.Map<IEnumerable<ShowTimeResponseDTO>>(showTimes);
        }

        public async Task<ShowTimeResponseDTO> UpdateAsync(int id, ShowTimeCreateDTO showTimeDto)
        {
            try
            {
                _logger.LogInformation("Updating showtime with ID: {ShowtimeId}", id);

                // 1. Get existing showtime
                var existingShowTime = await _showTimeRepository.GetByIdAsync(id);
                if (existingShowTime == null)
                {
                    _logger.LogWarning("Showtime not found with ID: {ShowtimeId}", id);
                    throw new NotFoundException("Showtime not found");
                }

                // 2. Validate movie exists
                var movie = await _movieRepository.GetByIdAsync(showTimeDto.MovieId);
                if (movie == null)
                {
                    _logger.LogWarning("Movie not found with ID: {MovieId}", showTimeDto.MovieId);
                    throw new NotFoundException("Movie not found");
                }

                // 3. Apply updates
                _mapper.Map(showTimeDto, existingShowTime);
                var updatedShowTime = await _showTimeRepository.UpdateAsync(existingShowTime);

                _logger.LogInformation("Successfully updated showtime with ID: {ShowtimeId}", id);
                return _mapper.Map<ShowTimeResponseDTO>(updatedShowTime);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating showtime with ID: {ShowtimeId}", id);
                throw new BadRequestException("Failed to update showtime. Please check your input and try again.");
            }
        }

    }
}