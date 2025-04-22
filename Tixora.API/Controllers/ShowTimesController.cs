using Microsoft.AspNetCore.Mvc;
using Tixora.Core.DTOs;
using Tixora.Service;
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
    public async Task<IActionResult> GetById(int id)
    {
        var showTime = await _showTimeService.GetByIdAsync(id);
        return Ok(showTime);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int movieId, [FromBody] ShowTimeCreateDTO showTimeDto)
    {
        showTimeDto.MovieId = movieId;
        var showTime = await _showTimeService.CreateAsync(showTimeDto);
        return CreatedAtAction(nameof(GetById), new { movieId, id = showTime.ShowtimeId }, showTime);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int movieId, int id, [FromBody] ShowTimeCreateDTO showTimeDto)
    {
        showTimeDto.MovieId = movieId;
        var showTime = await _showTimeService.UpdateAsync(id, showTimeDto);
        return Ok(showTime);
    }
}