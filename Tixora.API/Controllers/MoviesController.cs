using Microsoft.AspNetCore.Mvc;
using Tixora.Core.DTOs;
using Tixora.Service;
using Tixora.Service.Interfaces;

namespace Tixora.API.Controllers;

[Route("api/movies")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var movies = await _movieService.GetAllAsync();
        return Ok(movies);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetAllActive()
    {
        var movies = await _movieService.GetAllActiveAsync();
        return Ok(movies);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var movie = await _movieService.GetByIdAsync(id);
        return Ok(movie);
    }

    [HttpPost]

    public async Task<IActionResult> Create([FromBody] MovieCreateDTO movieDto)
    {
        var movie = await _movieService.CreateAsync(movieDto);
        return CreatedAtAction(nameof(GetById), new { id = movie.MovieId }, movie);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] MovieCreateDTO movieDto)
    {
        var movie = await _movieService.UpdateAsync(id, movieDto);
        return Ok(movie);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _movieService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
    [HttpPatch("{id}/toggle-status")] 
    public async Task<IActionResult> ToggleStatus(int id, [FromQuery] bool isActive)
    {
        await _movieService.ToggleMovieStatusAsync(id, isActive);
        return Ok($"Movie status updated to {(isActive ? "Active" : "Inactive")}");
    }
}