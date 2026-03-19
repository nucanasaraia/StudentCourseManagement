using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentCourseManagement.CORE;
using StudentCourseManagement.Data;
using StudentCourseManagement.DTOs;
using StudentCourseManagement.Enum;
using StudentCourseManagement.Models;
using StudentCourseManagement.Services.Interfaces;
using System.Net;
using System.Security.Cryptography;

public class AuthService : IAuthService
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthService(DataContext context, ITokenService tokenService, IEmailService emailService)
    {
        _context = context;
        _tokenService = tokenService;
        _emailService = emailService;
        _passwordHasher = new PasswordHasher<User>();
    }

    // REGISTER
    public async Task<ApiResponse<string>> Register(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLower();
        if (await _context.Users.AnyAsync(x => x.Email == email))
            return Error<string>("Email already exists");

        var user = new User
        {
            Username = dto.Username,
            Email = email,
            PasswordHash = _passwordHasher.HashPassword(null, dto.Password),
            Role = USER_ROLE.STUDENT,
            VerificationCode = GenerateVerificationCode(),
            VerificationCodeExpires = DateTime.UtcNow.AddMinutes(10)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var emailResult = await _emailService.SendVerificationCodeAsync(user.Email, user.Username, user.VerificationCode);
        if (emailResult.Status != HttpStatusCode.OK)
            return Error<string>("Failed to send verification email");

        return Success("Registration successful. Verify email.");
    }

    // VERIFY EMAIL
    public async Task<ApiResponse<string>> VerifyEmail(VerifyEmailDto dto)
    {
        var email = dto.Email.Trim().ToLower();
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            
        if (user == null)
            return Error<string>("Invalid request");

        if (user.VerificationAttempts >= 5)
            return Error<string>("Too many attempts");

        if (user.VerificationCodeExpires < DateTime.UtcNow)
            return Error<string>("Code expired");

        if (user.VerificationCode != dto.Code)
        {
            user.VerificationAttempts++;
            await _context.SaveChangesAsync();
            return Error<string>("Invalid code");
        }

        user.EmailVerified = true;
        user.VerificationCode = null;
        user.VerificationAttempts = 0;
        user.VerificationCodeExpires = null;

        await _context.SaveChangesAsync();
        return Success("Email verified successfully");
    }

    // LOGIN
    public async Task<ApiResponse<UserToken>> Login(LoginDto dto)
    {
        var username = dto.Username.Trim().ToLower();
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == username);

        if (user == null || !user.EmailVerified)
            return Error<UserToken>("Invalid credentials");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result != PasswordVerificationResult.Success)
            return Error<UserToken>("Invalid credentials");

        var tokens = await GenerateTokens(user);
        await _context.SaveChangesAsync();
        return Success(tokens);
    }

    // REFRESH TOKEN
    public async Task<ApiResponse<UserToken>> RefreshToken(string refreshToken)
    {
        var hash = _tokenService.HashToken(refreshToken);
        var token = await _context.RefreshTokens.Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == hash && !x.IsRevoked && x.ExpiresAt > DateTime.UtcNow);

        if (token == null)
            return Error<UserToken>("Invalid refresh token");

        token.IsRevoked = true;
        var tokens = await GenerateTokens(token.User);
        await _context.SaveChangesAsync();
        return Success(tokens);
    }

    // PASSWORD RESET
    public async Task<ApiResponse<string>> ForgotPassword(string email)
    {
        email = email.Trim().ToLower();
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        if (user == null)
            return Success("If this email exists, a reset link was sent");

        var rawToken = GenerateSecureToken();
        user.PasswordResetTokenHash = _tokenService.HashToken(rawToken);
        user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);

        var resetLink = $"https://yourfrontend.com/reset-password?token={rawToken}";
        await _emailService.SendPasswordResetLinkAsync(user.Email, user.Username, resetLink);
        await _context.SaveChangesAsync();

        return Success("If this email exists, a reset link was sent");
    }

    public async Task<ApiResponse<string>> ResetPassword(ResetPasswordDto dto)
    {
        var hash = _tokenService.HashToken(dto.Token);
        var user = await _context.Users.FirstOrDefaultAsync(x =>
            x.PasswordResetTokenHash == hash &&
            x.PasswordResetTokenExpires > DateTime.UtcNow);

        if (user == null)
            return Error<string>("Invalid or expired token");

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
        user.PasswordResetTokenHash = null;
        user.PasswordResetTokenExpires = null;

        await _context.SaveChangesAsync();
        return Success("Password reset successful");
    }
 
    // HELPERS
    private async Task<UserToken> GenerateTokens(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var hash = _tokenService.HashToken(refreshToken);

        _context.RefreshTokens.Add(new RefreshToken
        {
            TokenHash = hash,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        return new UserToken
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }

    private static string GenerateSecureToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    private static string GenerateVerificationCode() => RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    private static ApiResponse<T> Success<T>(T data)
    => ApiResponseFactory.Success(data);  

    private static ApiResponse<T> Error<T>(string message)
        => ApiResponseFactory.Fail<T>(message, HttpStatusCode.InternalServerError);
}