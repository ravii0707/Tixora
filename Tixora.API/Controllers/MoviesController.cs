using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tixora.Core.Constants;
using Tixora.Core.DTOs;
using Tixora.Service.Exceptions;
using Tixora.Service.Implementations;
using Tixora.Service.Interfaces;

namespace Tixora.API.Controllers;

[Route("api/movies")]
[ApiController]

public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly ILogger<MoviesController> _logger;
    private readonly IShowTimeService _showTimeService;

    public MoviesController(IMovieService movieService, ILogger<MoviesController> logger, IShowTimeService showTimeService)
    {
        _movieService = movieService;
        _logger = logger;
        _showTimeService = showTimeService;
    }
    [HttpGet("genres")]
    public IActionResult GetValidGenres()
    {
        return Ok(new
        {
            Success = true,
            Data = ValidGenres.Genres.OrderBy(g => g).ToList(),
            Message = "List of Valid Genres"
        });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var movies = await _movieService.GetAllAsync();
        return Ok(new
        {
            Success = true,
            Data = movies,
            Message = "Movies retrieved successfully"
        });
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllActive()
    {
        var movies = await _movieService.GetAllActiveAsync();
        return Ok(new
        {
            Success = true,
            Data = movies,
            Message = "Active movies retrieved successfully"
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var movie = await _movieService.GetByIdAsync(id);
        return Ok(new
        {
            Success = true,
            Data = movie,
            Message = "Movie retrieved successfully"
        });
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] MovieCreateDTO movieDto)
    {
        var movie = await _movieService.CreateAsync(movieDto);
        return CreatedAtAction(nameof(GetById), new { id = movie.MovieId }, new
        {
            Success = true,
            Data = movie,
            Message = "Movie created successfully"
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, [FromBody] MovieCreateDTO movieDto)
    {
        var movie = await _movieService.UpdateAsync(id, movieDto);
        return Ok(new
        {
            Success = true,
            Data = movie,
            Message = "Movie updated successfully"
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _movieService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ToggleStatus(int id, [FromQuery] bool isActive)
    {
        await _movieService.ToggleMovieStatusAsync(id, isActive);
        return Ok(new
        {
            Success = true,
            Message = $"Movie status updated to {(isActive ? "Active" : "Inactive")}"
        });
    }
}