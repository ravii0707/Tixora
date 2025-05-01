using Microsoft.AspNetCore.Mvc;
using Tixora.Core.DTOs;
using Tixora.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Tixora.Service.Exceptions;

namespace Tixora.API.Controllers;

[Route("api/bookings")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(
        IBookingService bookingService,
        ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BookingCreateDTO bookingDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid booking request data");
                return BadRequest(new
                {
                    Success = false,
                    Message = "Invalid booking data",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            var booking = await _bookingService.CreateAsync(bookingDto);

            _logger.LogInformation("Booking created with ID: {BookingId}", booking.BookingId);
            return CreatedAtAction(
                nameof(GetById),
                new { id = booking.BookingId },
                new
                {
                    Success = true,
                    Data = booking,
                    Message = "Booking created successfully"
                });
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(ex, "Bad booking request");
            return BadRequest(new
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found for booking");
            return NotFound(new
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var booking = await _bookingService.GetByIdAsync(id);
            return Ok(new
            {
                Success = true,
                Data = booking,
                Message = "Booking retrieved successfully"
            });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Booking not found with ID: {BookingId}", id);
            return NotFound(new
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        try
        {
            var bookings = await _bookingService.GetByUserIdAsync(userId);
            return Ok(new
            {
                Success = true,
                Data = bookings,
                Message = "User bookings retrieved successfully"
            });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "No bookings found for user ID: {UserId}", userId);
            return NotFound(new
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var bookings = await _bookingService.GetAllAsync();
        return Ok(new
        {
            Success = true,
            Data = bookings,
            Message = "All bookings retrieved successfully"
        });
    }
}