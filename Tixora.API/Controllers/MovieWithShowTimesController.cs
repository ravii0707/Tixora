using Microsoft.AspNetCore.Mvc;
using Tixora.Core.DTOs;
using Tixora.Service.Interfaces;
using Tixora.Service.Exceptions;

namespace Tixora.API.Controllers
{
    [Route("api/movie-showtimes")]
    [ApiController]
    public class MovieWithShowTimesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MovieWithShowTimesController> _logger;

        public MovieWithShowTimesController(
            IMovieService movieService,
            ILogger<MovieWithShowTimesController> logger)
        {
            _movieService = movieService;
            _logger = logger;
        }

    
        [HttpPost]
        public async Task<IActionResult> CreateMovieWithShowTimes([FromBody] MovieWithShowTimesDTO movieWithShows)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid request data",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var result = await _movieService.CreateMovieWithShowTimesAsync(movieWithShows);

                return CreatedAtAction(
                    nameof(GetMovieWithShowTimes),
                    new { movieId = result.Movie.MovieId },
                    new
                    {
                        Success = true,
                        Data = result,
                        Message = "Movie with showtimes created successfully"
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating movie with showtimes");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while creating movie with showtimes"
                });
            }
        }

        [HttpGet("{movieId}")]
        public async Task<IActionResult> GetMovieWithShowTimes(int movieId)
        {
            try
            {
                var result = await _movieService.GetMovieWithShowTimesAsync(movieId);
                return Ok(new
                {
                    Success = true,
                    Data = result,
                    Message = "Movie with showtimes retrieved successfully"
                });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Movie not found with ID: {MovieId}", movieId);
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving movie with showtimes for ID: {MovieId}", movieId);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while retrieving movie with showtimes"
                });
            }
        }
        [HttpPut("{movieId}")]
        public async Task<IActionResult> UpdateMovieWithShowTimes(
            int movieId,
            [FromBody] MovieWithShowTimesUpdateDTO updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid request data",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var result = await _movieService.UpdateMovieWithShowTimesAsync(movieId, updateDto);

                return Ok(new
                {
                    Success = true,
                    Data = result,
                    Message = "Movie with showtimes updated successfully"
                });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Movie or showtime not found for update");
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning(ex, "Bad request for movie with showtimes update");
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating movie with showtimes");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while updating movie with showtimes"
                });
            }
        }
    }
}