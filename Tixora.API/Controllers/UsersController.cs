using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tixora.Core.DTOs;
using Tixora.Service;
using Tixora.Service.Exceptions;
using Tixora.Service.Interfaces;

namespace Tixora.API.Controllers;

[Route("api/user")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;


    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDTO userDto)
    {
        var user = await _userService.RegisterAsync(userDto);
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
    {
        try
        {
            var user = await _userService.LoginAsync(loginDto);
            return Ok(user);
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // Log the exception here
            return StatusCode(500, new { message = "An error occurred during login" });
        }

    }
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        // Get current user's ID from claims
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // Only allow if current user is admin or accessing own profile
        if (currentUserId != id && currentUserRole != "admin")
        {
            return StatusCode(403, new { message = "Holdon !!!: Only admin can access for the resource" });
        }

        var user = await _userService.GetByIdAsync(id);
        return Ok(user);
    }

    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            return StatusCode(500, new { message = "An error occurred while retrieving users" });
        }
    }


}