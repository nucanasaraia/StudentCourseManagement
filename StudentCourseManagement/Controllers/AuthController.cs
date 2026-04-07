using Microsoft.AspNetCore.Mvc;
using StudentCourseManagement.DTOs;
using StudentCourseManagement.Services.Interfaces;
using System.Net;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private void SetRefreshTokenCookie(string refreshToken)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", refreshToken, options);
    }
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.Register(dto);
        return StatusCode((int)result.Status, result);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailDto dto)
    {
        var result = await _authService.VerifyEmail(dto);
        return StatusCode((int)result.Status, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.Login(dto);
        if (result.Data?.RefreshToken != null)
        {
            SetRefreshTokenCookie(result.Data.RefreshToken);
            result.Data.RefreshToken = null; 
        }
        return StatusCode((int)result.Status, result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh()
    {
        var token = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(token))
            return Unauthorized("Missing refresh token cookie.");

        var result = await _authService.RefreshToken(token);
        if (result.Data?.RefreshToken != null)
        {
            SetRefreshTokenCookie(result.Data.RefreshToken);
            result.Data.RefreshToken = null;
        }
        return StatusCode((int)result.Status, result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        var result = await _authService.ForgotPassword(dto.Email);
        return StatusCode((int)result.Status, result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var result = await _authService.ResetPassword(dto);
        return StatusCode((int)result.Status, result);
    }
}