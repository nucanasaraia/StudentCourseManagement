using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentCourseManagement.CORE;
using StudentCourseManagement.Data;
using StudentCourseManagement.DTOs;
using StudentCourseManagement.Models;
using StudentCourseManagement.Services.Interfaces;
using System.Net;
using System.Security.Cryptography;

public class AuthService : IAuthService
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly PasswordHasher<Student> _passwordHasher;

    public AuthService(
        DataContext context,
        ITokenService tokenService,
        IEmailService emailService)
    {
        _context = context;
        _tokenService = tokenService;
        _emailService = emailService;
        _passwordHasher = new PasswordHasher<Student>();
    }

    public async Task<ApiResponse<string>> Register(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        if (await _context.Students.AnyAsync(x => x.Email == email))
            return ApiResponseFactory.CreateErrorResponse<string>("Email already exists");

        var user = new Student
        {
            Username = dto.Username,
            Email = email
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        var emailResult = await _emailService.SendVerificationCodeAsync(user.Email, user.Username);

        if (emailResult.Status != HttpStatusCode.OK)
            return ApiResponseFactory.CreateErrorResponse<string>("Failed to send verification email");

        user.VerificationCode = emailResult.Data;
        user.VerificationCodeExpires = DateTime.UtcNow.AddMinutes(10);

        _context.Students.Add(user);
        await _context.SaveChangesAsync();

        return ApiResponseFactory.CreateSuccessResponse("Registration successful. Verify email.");
    }

    public async Task<ApiResponse<string>> VerifyEmail(VerifyEmailDto dto)
    {
        var user = await _context.Students.FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (user == null)
            return ApiResponseFactory.CreateErrorResponse<string>("User not found");

        if (user.VerificationCode != dto.Code)
            return ApiResponseFactory.CreateErrorResponse<string>("Invalid code");

        if (user.VerificationCodeExpires < DateTime.UtcNow)
            return ApiResponseFactory.CreateErrorResponse<string>("Code expired");

        user.EmailVerified = true;
        user.VerificationCode = null;
        user.VerificationCodeExpires = null;

        await _context.SaveChangesAsync();

        return ApiResponseFactory.CreateSuccessResponse("Email verified successfully");
    }

    public async Task<ApiResponse<UserToken>> Login(LoginDto dto)
    {
        var user = await _context.Students.FirstOrDefaultAsync(x => x.Username == dto.Username);
        var username = dto.Username.Trim().ToLower();

        if (user == null)
            return ApiResponseFactory.CreateErrorResponse<UserToken>("User not found");

        if (!user.EmailVerified)
            return ApiResponseFactory.CreateErrorResponse<UserToken>("Email not verified");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

        if (result != PasswordVerificationResult.Success)
            return ApiResponseFactory.CreateErrorResponse<UserToken>("Invalid password");

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            StudentId = user.Id
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return ApiResponseFactory.CreateSuccessResponse(new UserToken
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        });
    }

    public async Task<ApiResponse<UserToken>> RefreshToken(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .Include(x => x.Student)
            .FirstOrDefaultAsync(x => x.Token == refreshToken);

        if (token == null || token.IsRevoked || token.ExpiresAt <= DateTime.UtcNow)
            return ApiResponseFactory.CreateErrorResponse<UserToken>("Invalid refresh token");

        var newAccessToken = _tokenService.GenerateAccessToken(token.Student);

        return ApiResponseFactory.CreateSuccessResponse(new UserToken
        {
            Token = newAccessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        });
    }

    public async Task<ApiResponse<string>> ForgotPassword(string email)
    {
        var user = await _context.Students.FirstOrDefaultAsync(x => x.Email == email);

        if (user == null)
            return ApiResponseFactory.CreateErrorResponse<string>("User not found");

        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        user.PasswordResetToken = token;
        user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);

        var resetLink = $"https://yourfrontend.com/reset-password?token={token}";

        await _emailService.SendPasswordResetLinkAsync(user.Email, user.Username, resetLink);

        await _context.SaveChangesAsync();

        return ApiResponseFactory.CreateSuccessResponse("Reset link sent");
    }

    public async Task<ApiResponse<string>> ResetPassword(ResetPasswordDto dto)
    {
        var user = await _context.Students
            .FirstOrDefaultAsync(x => x.PasswordResetToken == dto.Token);

        if (user == null)
            return ApiResponseFactory.CreateErrorResponse<string>("Invalid token");

        if (user.PasswordResetTokenExpires < DateTime.UtcNow)
            return ApiResponseFactory.CreateErrorResponse<string>("Token expired");

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);

        user.PasswordResetToken = null;
        user.PasswordResetTokenExpires = null;

        await _context.SaveChangesAsync();

        return ApiResponseFactory.CreateSuccessResponse("Password reset successful");
    }
}