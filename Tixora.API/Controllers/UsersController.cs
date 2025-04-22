using Microsoft.AspNetCore.Mvc;
using Tixora.Core.DTOs;
using Tixora.Service;
using Tixora.Service.Interfaces;

namespace Tixora.API.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
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
        var user = await _userService.LoginAsync(loginDto);
        return Ok(user);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }
}