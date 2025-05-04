using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tixora.Core.DTOs;
using Tixora.Core.Entities;
using Tixora.Repository.Interfaces;
using Tixora.Service.Exceptions;
using Tixora.Service.Interfaces;

namespace Tixora.Service.Implementations
{
    public class ShowTimeService : IShowTimeService
    {
        private readonly IShowTimeRepository _showTimeRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ShowTimeService> _logger;

        // Business rules
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
                // Validate movie exists
                var movie = await _movieRepository.GetByIdAsync(showTimeDto.MovieId)
    ?? throw new NotFoundException("Movie", showTimeDto.MovieId);

                // Validate date and time
                ValidateShowDateTime(showTimeDto.ShowDate, showTimeDto.ShowTime);

                // Check showtime conflicts
                await CheckShowTimeConflicts(showTimeDto.ShowDate, showTimeDto.ShowTime, showTimeDto.MovieId);

                // Create showtime
                var showTime = _mapper.Map<TbShowTime>(showTimeDto);
                var createdShow = await _showTimeRepository.AddAsync(showTime);

                _logger.LogInformation("Created showtime {ShowtimeId} for movie {MovieId}",
                    createdShow.ShowtimeId, showTimeDto.MovieId);

                return _mapper.Map<ShowTimeResponseDTO>(createdShow);
            }
            catch (FormatException)
            {
                throw new BadRequestException("Invalid time format. Use HH:mm")
                { Data = { ["Field"] = "ShowTime" } };
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating showtime");
                throw new BadRequestException("Failed to create showtime. Please try again.");
            }
        }

        public async Task<ShowTimeResponseDTO> GetByIdAsync(int id)
        {
            var showTime = await _showTimeRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Showtime", id);
            return _mapper.Map<ShowTimeResponseDTO>(showTime);
        }

        public async Task<IEnumerable<ShowTimeResponseDTO>> GetAllAsync()
        {
            var showTimes = await _showTimeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ShowTimeResponseDTO>>(showTimes);
        }

        public async Task<IEnumerable<ShowTimeResponseDTO>> GetByMovieIdAsync(int movieId)
        {
            // Validate movie exists
            _ = await _movieRepository.GetByIdAsync(movieId)
                ?? throw new NotFoundException("Movie", movieId);

            var showTimes = await _showTimeRepository.GetByMovieIdAsync(movieId);
            return _mapper.Map<IEnumerable<ShowTimeResponseDTO>>(showTimes);
        }

        public async Task<ShowTimeResponseDTO> UpdateAsync(int id, ShowTimeCreateDTO showTimeDto)
        {
            var existingShow = await _showTimeRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Showtime", id);

            // Validate date and time
            ValidateShowDateTime(showTimeDto.ShowDate, showTimeDto.ShowTime);

            // Check conflicts excluding current showtime
            await CheckShowTimeConflicts(showTimeDto.ShowDate, showTimeDto.ShowTime,
                showTimeDto.MovieId, id);

            _mapper.Map(showTimeDto, existingShow);
            var updatedShow = await _showTimeRepository.UpdateAsync(existingShow);

            _logger.LogInformation("Updated showtime {ShowtimeId}", id);
            return _mapper.Map<ShowTimeResponseDTO>(updatedShow);
        }

        public async Task<IEnumerable<ShowTimeResponseDTO>> CreateMultipleShowTimesAsync(
            int movieId, IEnumerable<ShowTimeCreateDTO> showTimeDtos)
        {
            var movie = await _movieRepository.GetByIdAsync(movieId)
                ?? throw new NotFoundException("Movie not found", movieId);

            var createdShows = new List<ShowTimeResponseDTO>();

            foreach (var showDto in showTimeDtos)
            {
                try
                {
                    showDto.MovieId = movieId;
                    var createdShow = await CreateAsync(showDto);
                    createdShows.Add(createdShow);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to create showtime for movie {MovieId}", movieId);
                    // Continue with other showtimes
                }
            }

            return createdShows;
        }

        public async Task ValidateShowTimeDifference(int movieId, List<ShowTimeCreateDTO> shows)
        {
            var existingShows = await _showTimeRepository.GetByMovieIdAsync(movieId);

            foreach (var show in shows)
            {
                ValidateShowDateTime(show.ShowDate, show.ShowTime);

                // Check against other new shows
                foreach (var otherShow in shows.Where(s => s != show))
                {
                    if (show.ShowDate == otherShow.ShowDate)
                    {
                        CheckTimeGap(show.ShowTime, otherShow.ShowTime,
                            $"New showtimes at {show.ShowTime} and {otherShow.ShowTime}");
                    }
                }

                // Check against existing shows
                foreach (var existing in existingShows)
                {
                    if (show.ShowDate == existing.ShowDate)
                    {
                        CheckTimeGap(show.ShowTime, existing.ShowTime,
                            $"New showtime at {show.ShowTime} conflicts with existing show");
                    }
                }
            }
        }

        public async Task ValidateShowTimes(List<ShowTimeUpdateDTO> shows)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var now = TimeOnly.FromDateTime(DateTime.Now);

            foreach (var show in shows)
            {
                if (string.IsNullOrEmpty(show.ShowTime))
                    throw new BadRequestException("Show time is required")
                    { Data = { ["Field"] = "ShowTime" } };

                if (!TimeOnly.TryParse(show.ShowTime, out var showTime))
                    throw new BadRequestException("Invalid time format (HH:mm)")
                    { Data = { ["Field"] = "ShowTime" } };

                if (show.ShowDate < today)
                    throw new BadRequestException("Cannot add shows for past dates")
                    { Data = { ["Field"] = "ShowDate" } };

                if (show.ShowDate == today && showTime < now)
                    throw new BadRequestException("Cannot add shows for past times")
                    { Data = { ["Field"] = "ShowTime" } };

                if (show.ShowtimeId.HasValue && show.ShowtimeId.Value > 0)
                {
                    var showtimeId = show.ShowtimeId.Value;
                    var existingShow = await _showTimeRepository.GetByIdAsync(showtimeId)
                        ?? throw new NotFoundException("Showtime", showtimeId);

                  
                }
            }

            await ValidateShowTimeDifferences(shows);
        }

        public async Task ValidateShowTimeDifferences(List<ShowTimeUpdateDTO> shows)
        {
            var existingShows = await _showTimeRepository.GetAllAsync();

            foreach (var show in shows)
            {
                foreach (var otherShow in shows.Where(s => s != show))
                {
                    if (show.ShowDate == otherShow.ShowDate)
                    {
                        CheckTimeGap(show.ShowTime, otherShow.ShowTime,
                            $"Showtimes at {show.ShowTime} and {otherShow.ShowTime}");
                    }
                }

                foreach (var existing in existingShows)
                {
                    if (show.ShowDate == existing.ShowDate &&
                        show.ShowtimeId != existing.ShowtimeId)
                    {
                        CheckTimeGap(show.ShowTime, existing.ShowTime,
                            $"Conflict with existing show at {existing.ShowTime}");
                    }
                }
            }
        }

        #region Private Helpers

        private void ValidateShowDateTime(DateOnly showDate, string showTime)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var now = TimeOnly.FromDateTime(DateTime.Now);

            if (showDate < today)
                throw new BadRequestException("Cannot add shows for past dates")
                { Data = { ["Field"] = "ShowDate" } };

            if (!TimeOnly.TryParse(showTime, out var parsedTime))
                throw new BadRequestException("Invalid time format (HH:mm)")
                { Data = { ["Field"] = "ShowTime" } };

            if (showDate == today && parsedTime < now)
                throw new BadRequestException("Cannot add shows for past times")
                { Data = { ["Field"] = "ShowTime" } };

            if (showDate > today.AddDays(MAX_DAYS_IN_ADVANCE))
                throw new BadRequestException(
                    $"Shows can only be scheduled up to {MAX_DAYS_IN_ADVANCE} days in advance")
                { Data = { ["Field"] = "ShowDate" } };
        }

        private async Task CheckShowTimeConflicts(DateOnly date, string time, int movieId, int? excludeShowtimeId = null)
        {
            var existingShows = (await _showTimeRepository.GetByDateAsync(date))
                .Where(s => !excludeShowtimeId.HasValue || s.ShowtimeId != excludeShowtimeId.Value)
                .ToList();

            if (existingShows.Count >= MAX_SHOWS_PER_DAY)
                throw new BadRequestException(
                    $"Maximum {MAX_SHOWS_PER_DAY} shows allowed per day")
                { Data = { ["Field"] = "ShowDate" } };

            var newTime = TimeOnly.Parse(time);
            foreach (var existing in existingShows)
            {
                var existingTime = TimeOnly.Parse(existing.ShowTime);
                var gap = Math.Abs((newTime - existingTime).TotalMinutes);

                if (gap < MIN_GAP_BETWEEN_SHOWS_MINUTES)
                    throw new BadRequestException(
                        $"Minimum {MIN_GAP_BETWEEN_SHOWS_MINUTES} minutes required between shows. " +
                        $"Conflict with show at {existing.ShowTime}")
                    { Data = { ["Field"] = "ShowTime" } };
            }
        }

        private void CheckTimeGap(string time1, string time2, string errorContext)
        {
            var t1 = TimeOnly.Parse(time1);
            var t2 = TimeOnly.Parse(time2);
            var gap = Math.Abs((t1 - t2).TotalMinutes);

            if (gap < MIN_GAP_BETWEEN_SHOWS_MINUTES)
                throw new BadRequestException(
                    $"{errorContext}. Minimum {MIN_GAP_BETWEEN_SHOWS_MINUTES} minutes required between shows");
        }

        }#endregion
    }
}