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
    private readonly IUserLogger _userLogger;
    public AuthService(DataContext context, ITokenService tokenService, IEmailService emailService, IUserLogger userLogger)
    {
        _context = context;
        _tokenService = tokenService;
        _emailService = emailService;
        _passwordHasher = new PasswordHasher<User>();
        _userLogger = userLogger;
    }

    // REGISTER 
    public async Task<ApiResponse<string>> Register(RegisterDto dto)
    {
        try
        {
            var email = dto.Email.Trim().ToLower();
            if (await _context.Users.AnyAsync(x => x.Email == email))
            {
                _userLogger.LogWarning(null, "Registration failed. Email already exists: {Email}", email);
                return Error<string>("Email already exists");
            }

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

            // Email service disabled - auto verify for demo purposes
            user.EmailVerified = true;
            await _context.SaveChangesAsync();

            // var emailResult = await _emailService.SendVerificationCodeAsync(user.Email, user.Username, user.VerificationCode);
            // if (emailResult.Status != HttpStatusCode.OK)
            // {
            //     _userLogger.LogError(user, null, "Failed to send verification email.");
            //     return Error<string>("Failed to send verification email");
            // }

            _userLogger.LogInfo(user, "User registered successfully.");
            return Success("Registration successful.");
        }
        catch (Exception ex)
        {
            _userLogger.LogError(null, ex, "Exception during registration for Email: {Email}", dto.Email);
            return Error<string>("An unexpected error occurred during registration");
        }
    }

    // VERIFY EMAIL 
    public async Task<ApiResponse<string>> VerifyEmail(VerifyEmailDto dto)
    {
        try
        {
            var email = dto.Email.Trim().ToLower();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                _userLogger.LogWarning(null, "Email verification failed. User not found: {Email}", email);
                return Error<string>("Invalid request");
            }

            if (user.VerificationAttempts >= 5)
            {
                _userLogger.LogWarning(user, "Email verification failed. Too many attempts.");
                return Error<string>("Too many attempts");
            }

            if (user.VerificationCodeExpires < DateTime.UtcNow)
            {
                _userLogger.LogWarning(user, "Email verification failed. Code expired.");
                return Error<string>("Code expired");
            }

            if (user.VerificationCode != dto.Code)
            {
                user.VerificationAttempts++;
                await _context.SaveChangesAsync();
                _userLogger.LogWarning(user, "Email verification failed. Invalid code.");
                return Error<string>("Invalid code");
            }

            user.EmailVerified = true;
            user.VerificationCode = null;
            user.VerificationAttempts = 0;
            user.VerificationCodeExpires = null;

            await _context.SaveChangesAsync();

            _userLogger.LogInfo(user, "Email verified successfully.");
            return Success("Email verified successfully");
        }
        catch (Exception ex)
        {
            _userLogger.LogError(null, ex, "Exception during email verification for Email: {Email}", dto.Email);
            return Error<string>("An unexpected error occurred during email verification");
        }
    }

    // LOGIN 
    public async Task<ApiResponse<UserToken>> Login(LoginDto dto)
    {
        var username = dto.Username.Trim().ToLower();
        User? user = null;

        try
        {
            user = await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == username);

            if (user == null || !user.EmailVerified)
            {
                _userLogger.LogWarning(null, "Login attempt failed. User not found or email not verified. Username: {Username}", username);
                return Error<UserToken>("Invalid credentials");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result != PasswordVerificationResult.Success)
            {
                _userLogger.LogWarning(user, "Login attempt failed. Incorrect password.");
                return Error<UserToken>("Invalid credentials");
            }

            var tokens = await GenerateTokens(user);
            await _context.SaveChangesAsync();

            _userLogger.LogInfo(user, "User logged in successfully.");
            return Success(tokens);
        }
        catch (Exception ex)
        {
                _userLogger.LogError(user, ex, "Exception during login.");
            return Error<UserToken>("An unexpected error occurred during login");
        }
    }

    // REFRESH TOKEN 
    public async Task<ApiResponse<UserToken>> RefreshToken(string refreshToken)
    {
        try
        {
            var hash = _tokenService.HashToken(refreshToken);
            var token = await _context.RefreshTokens.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.TokenHash == hash && !x.IsRevoked && x.ExpiresAt > DateTime.UtcNow);

            if (token == null)
            {
                _userLogger.LogWarning(null, "Refresh token failed. Invalid or expired token.");
                return Error<UserToken>("Invalid refresh token");
            }

            token.IsRevoked = true;
            var tokens = await GenerateTokens(token.User);
            await _context.SaveChangesAsync();

            _userLogger.LogInfo(token.User, "Refresh token successful.");
            return Success(tokens);
        }
        catch (Exception ex)
        {
            _userLogger.LogError(null, ex, "Exception during refresh token.");
            return Error<UserToken>("An unexpected error occurred during token refresh");
        }
    }

    // FORGOT PASSWORD
    public async Task<ApiResponse<string>> ForgotPassword(string email)
    {
        try
        {
            email = email.Trim().ToLower();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                _userLogger.LogInfo(null, "Forgot password requested for non-existent email: {Email}", email);
                return Success("If this email exists, a reset link was sent");
            }

            var rawToken = GenerateSecureToken();
            user.PasswordResetTokenHash = _tokenService.HashToken(rawToken);
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);

            var resetLink = $"https://yourfrontend.com/reset-password?token={rawToken}";
           // await _emailService.SendPasswordResetLinkAsync(user.Email, user.Username, resetLink);
            await _context.SaveChangesAsync();

            _userLogger.LogInfo(user, "Password reset requested.");
            return Success("If this email exists, a reset link was sent");
        }
        catch (Exception ex)
        {
            _userLogger.LogError(null, ex, "Exception during forgot password for Email: {Email}", email);
            return Error<string>("An unexpected error occurred during password reset request");
        }
    }

    // RESET PASSWORD 
    public async Task<ApiResponse<string>> ResetPassword(ResetPasswordDto dto)
    {
        try
        {
            var hash = _tokenService.HashToken(dto.Token);
            var user = await _context.Users.FirstOrDefaultAsync(x =>
                x.PasswordResetTokenHash == hash &&
                x.PasswordResetTokenExpires > DateTime.UtcNow);

            if (user == null)
            {
                _userLogger.LogWarning(null, "Password reset failed. Invalid or expired token.");
                return Error<string>("Invalid or expired token");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
            user.PasswordResetTokenHash = null;
            user.PasswordResetTokenExpires = null;

            await _context.SaveChangesAsync();

            _userLogger.LogInfo(user, "Password reset successful.");
            return Success("Password reset successful");
        }
        catch (Exception ex)
        {
            _userLogger.LogError(null, ex, "Exception during password reset.");
            return Error<string>("An unexpected error occurred during password reset");
        }
    }

    // HELPERS
    private Task<UserToken> GenerateTokens(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var hash = _tokenService.HashToken(refreshToken);

        _context.RefreshTokens.Add(new RefreshToken
        {
            TokenHash = hash,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = user
        });

        return Task.FromResult(new UserToken
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        });
    }

    private static string GenerateSecureToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    private static string GenerateVerificationCode() => RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    private static ApiResponse<T> Success<T>(T data) => ApiResponseFactory.Success(data);
    private static ApiResponse<T> Error<T>(string message) => ApiResponseFactory.Fail<T>(message, HttpStatusCode.InternalServerError);
}