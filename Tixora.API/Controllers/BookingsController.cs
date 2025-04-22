using Microsoft.AspNetCore.Mvc;
using Tixora.Core.DTOs;
using Tixora.Service;
using Tixora.Service.Interfaces;

namespace Tixora.API.Controllers;

[Route("api/booking")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] BookingCreateDTO bookingDto)
    {
        var booking = await _bookingService.CreateAsync(bookingDto);
        return CreatedAtAction(nameof(GetById), new { id = booking.BookingId }, booking);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var booking = await _bookingService.GetByIdAsync(id);
        return Ok(booking);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var bookings = await _bookingService.GetByUserIdAsync(userId);
        return Ok(bookings);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var bookings = await _bookingService.GetAllAsync();
        return Ok(bookings);
    }
}