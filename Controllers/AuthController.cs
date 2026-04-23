using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTextApi.Interfaces;
using MyTextApi.Models.DTOs;
using System.Security.Claims;

namespace MyTextApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;
    private readonly ICookieService _cookieService;

    public AuthController(
        IAuthService authService,
        IJwtService jwtService,
        ICookieService cookieService
    )
    {
        _authService = authService;
        _jwtService = jwtService;
        _cookieService = cookieService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        if (response == null)
        {
            return BadRequest(new { message = "Email already exists" });
        }

        // Генерируем и устанавливаем токены в cookies
        var (accessToken, refreshToken, expiry) = _jwtService.GenerateTokenPair(
            new Models.Entities.User
            {
                Id = response.Id,
                Name = response.Name,
                Email = response.Email,
            }
        );

        _cookieService.SetAccessToken(accessToken, expiry);
        _cookieService.SetRefreshToken(refreshToken);

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        if (response == null)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        // Генерируем и устанавливаем токены в cookies
        var (accessToken, refreshToken, expiry) = _jwtService.GenerateTokenPair(
            new Models.Entities.User
            {
                Id = response.Id,
                Name = response.Name,
                Email = response.Email,
            }
        );

        _cookieService.SetAccessToken(accessToken, expiry);
        _cookieService.SetRefreshToken(refreshToken);

        return Ok(response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = _cookieService.GetRefreshToken();
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { message = "Refresh token is missing" });
        }

        var response = await _authService.RefreshTokenAsync(refreshToken);
        if (response == null)
        {
            _cookieService.ClearAuthCookies();
            return Unauthorized(new { message = "Invalid refresh token" });
        }

        // Новые токены
        var (accessToken, newRefreshToken, expiry) = _jwtService.GenerateTokenPair(
            new Models.Entities.User
            {
                Id = response.Id,
                Name = response.Name,
                Email = response.Email,
            }
        );

        _cookieService.SetAccessToken(accessToken, expiry);
        _cookieService.SetRefreshToken(newRefreshToken);

        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var response = await _authService.GetMeAsync(userId);
        return response == null
            ? NotFound()
            : Ok(response);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        _cookieService.ClearAuthCookies();
        return Ok(new { message = "Logged out successfully" });
    }
}