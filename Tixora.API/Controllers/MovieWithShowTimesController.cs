using Microsoft.AspNetCore.Mvc;
using Tixora.Core.DTOs;
using Tixora.Service.Interfaces;
using Tixora.Service.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace Tixora.API.Controllers
{
    [Route("api/movie-showtimes")]
    [ApiController]
    [Authorize]
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
        [Authorize(Roles = "admin")]
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
                    Message = "..you cannot add movie with past showtimes or date"
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
        [Authorize(Roles = "admin")]
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
                        Message = "Validation failed",
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
                    Message = "Movie and showtimes updated successfully"
                });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found");
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message,
                    Source = ex.Source // Added to identify error origin
                });
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning(ex, "Invalid request");
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message,
                    Field = ex.Data["Field"], // Added to pinpoint problematic field
                    Source = "Validation"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Server error");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while updating",
                    Details = ex.Message,
                    Source = "Server"
                });
            }
        }
    }
}