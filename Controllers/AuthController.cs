using Microsoft.AspNetCore.Mvc;
using TaskManagement.Common;
using TaskManagement.DTO.Auth;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Controllers;

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
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(ApiResponse<LoginResponse>.Ok(result, "Registration successful"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<LoginResponse>.Fail(ex.Message));
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(ApiResponse<LoginResponse>.Ok(result, "Login successful"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<LoginResponse>.Fail(ex.Message));
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken([FromBody] string refreshToken)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
            return Ok(ApiResponse<LoginResponse>.Ok(result, "Token refreshed successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<LoginResponse>.Fail(ex.Message));
        }
    }

    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<object>>> Logout([FromBody] string refreshToken)
    {
        try
        {
            await _authService.LogoutAsync(refreshToken);
            return Ok(ApiResponse<object>.Ok(null, "Logout successful"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
