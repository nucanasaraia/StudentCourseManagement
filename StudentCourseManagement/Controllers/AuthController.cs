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

        if (result.Status == HttpStatusCode.Conflict)
            return Conflict(result);

        if (result.Status != HttpStatusCode.OK)
            return BadRequest(result);

        return StatusCode(201, result);
    }

    [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(VerifyEmailDto dto)
            => Ok(await _authService.VerifyEmail(dto));

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.Login(dto);

        if (result.Status == HttpStatusCode.Unauthorized)
            return Unauthorized(result);

        if (result.Status != HttpStatusCode.OK)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh(RefreshTokenDto dto)
            => Ok(await _authService.RefreshToken(dto.RefreshToken));

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
            => Ok(await _authService.ForgotPassword(dto.Email));

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
            => Ok(await _authService.ResetPassword(dto));

}