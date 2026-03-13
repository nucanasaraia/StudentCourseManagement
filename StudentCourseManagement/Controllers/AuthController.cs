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
    public async Task<ActionResult> Register([FromBody] StudentDto dto)
    {
        var result = await _authService.Register(dto);

        return result.Status switch
        {
            HttpStatusCode.OK => Ok(result),
            HttpStatusCode.Conflict => Conflict(result),
            _ => BadRequest(result)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] StudentDto dto)
    {
        var result = await _authService.Login(dto);

        return result.Status switch
        {
            HttpStatusCode.OK => Ok(result),
            HttpStatusCode.BadRequest => BadRequest(result),
            HttpStatusCode.NotFound => NotFound(result),
            _ => BadRequest(result)
        };
    }
}