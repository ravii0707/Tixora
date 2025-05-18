//using Microsoft.AspNetCore.Mvc;
//using Tixora.Core.DTOs;
//using Tixora.Service.Interfaces;
//using Microsoft.Extensions.Logging;
//using Tixora.Service.Exceptions;
//using Microsoft.AspNetCore.Authorization;

//namespace Tixora.API.Controllers
//{
//    [Route("api/movies/{movieId}/showtimes")]
//    [ApiController]
//    public class ShowTimesController : ControllerBase
//    {
//        private readonly IShowTimeService _showTimeService;
//        private readonly ILogger<ShowTimesController> _logger;

//        public ShowTimesController(
//            IShowTimeService showTimeService,
//            ILogger<ShowTimesController> logger)
//        {
//            _showTimeService = showTimeService;
//            _logger = logger;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetByMovieId(int movieId)
//        {
//            var showTimes = await _showTimeService.GetByMovieIdAsync(movieId);
//            return Ok(new
//            {
//                Success = true,
//                Data = showTimes,
//                Message = "Showtimes retrieved successfully"
//            });
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(int movieId, int id)
//        {
//            try
//            {
//                var showTime = await _showTimeService.GetByIdAsync(id);
//                if (showTime.MovieId != movieId)
//                {
//                    _logger.LogWarning("Showtime {ShowtimeId} doesn't belong to movie {MovieId}", id, movieId);
//                    return NotFound(new
//                    {
//                        Success = false,
//                        Message = "Showtime not found for the specified movie"
//                    });
//                }

//                return Ok(new
//                {
//                    Success = true,
//                    Data = showTime,
//                    Message = "Showtime retrieved successfully"
//                });
//            }
//            catch (NotFoundException ex)
//            {
//                _logger.LogWarning(ex, "Showtime not found with ID: {ShowtimeId}", id);
//                return NotFound(new
//                {
//                    Success = false,
//                    Message = ex.Message
//                });
//            }
//        }

//        [HttpPost]
//        [Authorize(Roles = "admin")]
//        public async Task<IActionResult> Create(int movieId, [FromBody] ShowTimeCreateDTO showTimeDto)
//        {
//            try
//            {
//                showTimeDto.MovieId = movieId; // Ensure movieId matches route

//                if (!ModelState.IsValid)
//                {
//                    _logger.LogWarning("Invalid model state for showtime creation");
//                    return BadRequest(new
//                    {
//                        Success = false,
//                        Message = "Invalid request data",
//                        Errors = ModelState.Values
//                            .SelectMany(v => v.Errors)
//                            .Select(e => e.ErrorMessage)
//                    });
//                }

//                var result = await _showTimeService.CreateAsync(showTimeDto);

//                return CreatedAtAction(
//                    nameof(GetById),
//                    new { movieId, id = result.ShowtimeId },
//                    new
//                    {
//                        Success = true,
//                        Data = result,
//                        Message = "Showtime created successfully"
//                    });
//            }
//            catch (BadRequestException ex)
//            {
//                _logger.LogWarning(ex, "Bad request for showtime creation");
//                return BadRequest(new
//                {
//                    Success = false,
//                    Message = ex.Message
//                });
//            }
//        }

//        [HttpPut("{id}")]
//        [Authorize(Roles ="admin")]
//        public async Task<IActionResult> Update(int movieId, int id, [FromBody] ShowTimeCreateDTO showTimeDto)
//        {
//            try
//            {
//                showTimeDto.MovieId = movieId;
//                var showTime = await _showTimeService.UpdateAsync(id, showTimeDto);

//                return Ok(new
//                {
//                    Success = true,
//                    Data = showTime,
//                    Message = "Showtime updated successfully"
//                });
//            }
//            catch (NotFoundException ex)
//            {
//                _logger.LogWarning(ex, "Showtime not found for update: {ShowtimeId}", id);
//                return NotFound(new
//                {
//                    Success = false,
//                    Message = ex.Message
//                });
//            }
//            catch (BadRequestException ex)
//            {
//                _logger.LogWarning(ex, "Bad request for showtime update: {ShowtimeId}", id);
//                return BadRequest(new
//                {
//                    Success = false,
//                    Message = ex.Message
//                });
//            }
//        }
//    }
//}
