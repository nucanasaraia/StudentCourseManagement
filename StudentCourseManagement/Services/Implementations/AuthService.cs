using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentCourseManagement.CORE;
using StudentCourseManagement.Data;
using StudentCourseManagement.DTOs;
using StudentCourseManagement.Models;
using StudentCourseManagement.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly DataContext _context;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<Student> _passwordHasher;

    public AuthService(DataContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
        _passwordHasher = new PasswordHasher<Student>();
    }

    public async Task<ApiResponse<string>> Register(StudentDto dto)
    {
        if (await _context.Students.AnyAsync(u => u.Username == dto.Username))
            return ApiResponseFactory.CreateErrorResponse<string>("User already exists");

        var user = new Student
        {
            Username = dto.Username,
            Email = dto.Email
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _context.Students.Add(user);
        await _context.SaveChangesAsync();

        return ApiResponseFactory.CreateSuccessResponse("User registered successfully");
    }

    public async Task<ApiResponse<string>> Login(StudentDto dto)
    {
        var user = await _context.Students.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null)
            return ApiResponseFactory.CreateErrorResponse<string>("User not found");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result != PasswordVerificationResult.Success)
            return ApiResponseFactory.CreateErrorResponse<string>("Invalid password");

        var token = GenerateJwtToken(user);
        return ApiResponseFactory.CreateSuccessResponse(token);
    }

    private string GenerateJwtToken(Student user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:DurationInMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}