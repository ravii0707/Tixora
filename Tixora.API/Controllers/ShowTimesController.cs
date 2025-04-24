using Microsoft.AspNetCore.Mvc;
using Tixora.Core.DTOs;
using Tixora.Service.Interfaces;

namespace Tixora.API.Controllers;

[Route("api/movies/{movieId}/showtimes")]
[ApiController]
public class ShowTimesController : ControllerBase
{
    private readonly IShowTimeService _showTimeService;

    public ShowTimesController(IShowTimeService showTimeService)
    {
        _showTimeService = showTimeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByMovieId(int movieId)
    {
        var showTimes = await _showTimeService.GetByMovieIdAsync(movieId);
        return Ok(showTimes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int movieId, int id)
    {
        var showTime = await _showTimeService.GetByIdAsync(id);
        if (showTime == null || showTime.MovieId != movieId)
        {
            return NotFound();
        }
        return Ok(showTime);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int movieId, [FromBody] ShowTimeCreateDTO showTimeDto)
    {
        try
        {
            showTimeDto.MovieId = movieId; // Ensure movieId matches route

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _showTimeService.CreateAsync(showTimeDto);
            return CreatedAtAction(nameof(GetById),
                new { movieId, id = result.ShowtimeId },
                result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int movieId, int id, [FromBody] ShowTimeCreateDTO showTimeDto)
    {
        showTimeDto.MovieId = movieId;
        var showTime = await _showTimeService.UpdateAsync(id, showTimeDto);
        return Ok(showTime);
    }
    //[HttpPost("bulk")]
    //public async Task<IActionResult> BulkCreate(
    //       int movieId,
    //       [FromBody] BulkShowTimeCreateDTO request)
    //{
    //    try
    //    {
    //        var results = await _showTimeService.BulkCreateAsync(movieId, request);
    //        return Ok(results);
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //}
    //public class BulkShowTimeCreateDTO
    //{
    //    public DateOnly ShowDate { get; set; }
    //    public required List<TimeOnly> ShowTimes { get; set; }  // Example: ["10:00", "13:30"]
    //    public int AvailableSeats { get; set; } = 290;
    //}
}
