using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tixora.Core.DTOs;
using Tixora.Core.Entities;
using Tixora.Repository.Interfaces;
using Tixora.Service.Exceptions;
using Tixora.Core.Context;
using Microsoft.Extensions.Logging;
using Tixora.Service.Interfaces;
using Tixora.Repository.Implementations;

namespace Tixora.Service.Implementations
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly ILogger<MovieService> _logger;
        private readonly IShowTimeRepository _showTimeRepository;

        public MovieService(
            IMovieRepository movieRepository,
            IMapper mapper,
            AppDbContext context,
            ILogger<MovieService> logger,
        IShowTimeRepository showTimeRepository)
        {
            _movieRepository = movieRepository;
            _mapper = mapper;
            _context = context;
            _logger = logger;
            _showTimeRepository = showTimeRepository;
        }

        public async Task<MovieResponseDTO> CreateAsync(MovieCreateDTO movieDto)
        {
            try
            {
                var movie = _mapper.Map<TbMovie>(movieDto);
                var createdMovie = await _movieRepository.AddAsync(movie);
                _logger.LogInformation("Movie created with ID: {MovieId}", createdMovie.MovieId);
                return _mapper.Map<MovieResponseDTO>(createdMovie);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating movie");
                throw new BadRequestException("Failed to create movie. Please check your input and try again.");
            }
        }

        public async Task<MovieResponseDTO> GetByIdAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                _logger.LogWarning("Movie not found with ID: {MovieId}", id);
                throw new NotFoundException("Movie", id);
            }
            return _mapper.Map<MovieResponseDTO>(movie);
        }

        public async Task<IEnumerable<MovieResponseDTO>> GetAllAsync()
        {
            var movies = await _movieRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MovieResponseDTO>>(movies);
        }

        public async Task<IEnumerable<MovieResponseDTO>> GetAllActiveAsync()
        {
            var movies = await _movieRepository.GetAllActiveAsync();
            return _mapper.Map<IEnumerable<MovieResponseDTO>>(movies);
        }

        public async Task<MovieResponseDTO> UpdateAsync(int id, MovieCreateDTO movieDto)
        {
            try
            {
                var existingMovie = await _movieRepository.GetByIdAsync(id);
                if (existingMovie == null)
                {
                    _logger.LogWarning("Movie not found for update with ID: {MovieId}", id);
                    throw new NotFoundException("Movie", id);
                }

                _mapper.Map(movieDto, existingMovie);
                var updatedMovie = await _movieRepository.UpdateAsync(existingMovie);
                _logger.LogInformation("Movie updated with ID: {MovieId}", id);
                return _mapper.Map<MovieResponseDTO>(updatedMovie);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error updating movie with ID: {MovieId}", id);
                throw new BadRequestException("Failed to update movie. Please check your input and try again.");
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var result = await _movieRepository.DeleteAsync(id);
                if (!result)
                {
                    _logger.LogWarning("Attempt to delete non-existent movie with ID: {MovieId}", id);
                    throw new NotFoundException("Movie", id);
                }
                _logger.LogInformation("Movie deleted with ID: {MovieId}", id);
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error deleting movie with ID: {MovieId}", id);
                throw new BadRequestException("Failed to delete movie. It may be referenced by other records.");
            }
        }

        public async Task ToggleMovieStatusAsync(int movieId, bool isActive)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var movie = await _context.TbMovies.FindAsync(movieId);
                if (movie == null)
                {
                    _logger.LogWarning("Movie not found for status toggle with ID: {MovieId}", movieId);
                    throw new NotFoundException("Movie", movieId);
                }

                movie.IsActive = isActive;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Movie {MovieId} status changed to {Status}",
                    movieId, isActive ? "Active" : "Inactive");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<MovieWithShowTimesResponseDTO> CreateMovieWithShowTimesAsync(MovieWithShowTimesDTO movieWithShows)
        {
            // Create the movie first
            var movie = _mapper.Map<TbMovie>(movieWithShows.Movie);
            var createdMovie = await _movieRepository.AddAsync(movie);

            // Then create the showtimes
            var showTimes = new List<ShowTimeResponseDTO>();
            foreach (var showDto in movieWithShows.Shows)
            {
                var showTime = _mapper.Map<TbShowTime>(showDto);
                showTime.MovieId = createdMovie.MovieId;
                var createdShow = await _showTimeRepository.AddAsync(showTime);
                showTimes.Add(_mapper.Map<ShowTimeResponseDTO>(createdShow));
            }

            // Map to response DTO
            return new MovieWithShowTimesResponseDTO
            {
                Movie = _mapper.Map<MovieResponseDTO>(createdMovie),
                Shows = showTimes
            };
        }

        public async Task<MovieWithShowTimesResponseDTO> GetMovieWithShowTimesAsync(int movieId)
        {
            // Get the movie
            var movie = await _movieRepository.GetByIdAsync(movieId);
            if (movie == null)
            {
                throw new NotFoundException("Movie not found");
            }

            // Get its showtimes
            var showTimes = await _showTimeRepository.GetByMovieIdAsync(movieId);

            return new MovieWithShowTimesResponseDTO
            {
                Movie = _mapper.Map<MovieResponseDTO>(movie),
                Shows = _mapper.Map<List<ShowTimeResponseDTO>>(showTimes)
            };
        }
    }
}