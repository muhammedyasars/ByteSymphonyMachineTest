using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ByteSymmetryTest.DTOs;
using ByteSymmetryTest.Services;

namespace ByteSymmetryTest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(new { errors });
        }

        var (success, message) = await _authService.RegisterAsync(request);

        if (!success)
        {
            return BadRequest(new { errors = new[] { message } });
        }

        return Ok(new { message });
    }

  
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(new { errors });
        }

        var (success, data, message) = await _authService.LoginAsync(request);

        if (!success)
        {
            return BadRequest(new { errors = new[] { message } });
        }

        return Ok(data);
    }
}
