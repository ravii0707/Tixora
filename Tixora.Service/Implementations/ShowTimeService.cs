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
    public class ShowTimeService : IShowTimeService
    {
        private readonly IShowTimeRepository _showTimeRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ShowTimeService> _logger;

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
                var movie = await _movieRepository.GetByIdAsync(showTimeDto.MovieId);
                if (movie == null)
                {
                    _logger.LogWarning("Movie not found for showtime creation: {MovieId}", showTimeDto.MovieId);
                    throw new NotFoundException("Movie", showTimeDto.MovieId);
                }

                // Validate show date is not in the past
                if (showTimeDto.ShowDate < DateOnly.FromDateTime(DateTime.Now))
                {
                    _logger.LogWarning("Attempt to create showtime in the past: {ShowDate}", showTimeDto.ShowDate);
                    throw new BadRequestException("Cannot create showtimes for past dates.");
                }

                var showTime = _mapper.Map<TbShowTime>(showTimeDto);
                var createdShowTime = await _showTimeRepository.AddAsync(showTime);

                _logger.LogInformation("Showtime created with ID: {ShowtimeId}", createdShowTime.ShowtimeId);
                return _mapper.Map<ShowTimeResponseDTO>(createdShowTime);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating showtime");
                throw new BadRequestException("Failed to create showtime. Please check your input and try again.");
            }
        }

        public async Task<ShowTimeResponseDTO> GetByIdAsync(int id)
        {
            var showTime = await _showTimeRepository.GetByIdAsync(id);
            if (showTime == null)
            {
                _logger.LogWarning("Showtime not found with ID: {ShowtimeId}", id);
                throw new NotFoundException("Showtime", id);
            }
            return _mapper.Map<ShowTimeResponseDTO>(showTime);
        }

        public async Task<IEnumerable<ShowTimeResponseDTO>> GetAllAsync()
        {
            var showTimes = await _showTimeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ShowTimeResponseDTO>>(showTimes);
        }

        public async Task<IEnumerable<ShowTimeResponseDTO>> GetByMovieIdAsync(int movieId)
        {
            // Validate movie exists first
            var movie = await _movieRepository.GetByIdAsync(movieId);
            if (movie == null)
            {
                _logger.LogWarning("Movie not found when fetching showtimes: {MovieId}", movieId);
                throw new NotFoundException("Movie", movieId);
            }

            var showTimes = await _showTimeRepository.GetByMovieIdAsync(movieId);
            return _mapper.Map<IEnumerable<ShowTimeResponseDTO>>(showTimes);
        }

        public async Task<ShowTimeResponseDTO> UpdateAsync(int id, ShowTimeCreateDTO showTimeDto)
        {
            try
            {
                var existingShowTime = await _showTimeRepository.GetByIdAsync(id);
                if (existingShowTime == null)
                {
                    _logger.LogWarning("Showtime not found for update: {ShowtimeId}", id);
                    throw new NotFoundException("Showtime", id);
                }

                // Validate movie exists
                var movie = await _movieRepository.GetByIdAsync(showTimeDto.MovieId);
                if (movie == null)
                {
                    _logger.LogWarning("Movie not found during showtime update: {MovieId}", showTimeDto.MovieId);
                    throw new NotFoundException("Movie", showTimeDto.MovieId);
                }

                _mapper.Map(showTimeDto, existingShowTime);
                var updatedShowTime = await _showTimeRepository.UpdateAsync(existingShowTime);

                _logger.LogInformation("Showtime updated with ID: {ShowtimeId}", id);
                return _mapper.Map<ShowTimeResponseDTO>(updatedShowTime);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error updating showtime with ID: {ShowtimeId}", id);
                throw new BadRequestException("Failed to update showtime. Please check your input and try again.");
            }
        }
    }
}