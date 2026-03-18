using Microsoft.AspNetCore.Mvc;
using StudentCourseManagement.DTOs;
using StudentCourseManagement.Services.Interfaces;
using System.Net;

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
    public async Task<IActionResult> Register(RegisterDto dto)
    {
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
        return StatusCode((int)result.Status, result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh(RefreshTokenDto dto)
    {
        var result = await _authService.RefreshToken(dto.RefreshToken);
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