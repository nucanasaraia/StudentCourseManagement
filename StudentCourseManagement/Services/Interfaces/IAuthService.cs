using StudentCourseManagement.CORE;
using StudentCourseManagement.DTOs;

namespace StudentCourseManagement.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> Register(RegisterDto dto);

        Task<ApiResponse<string>> VerifyEmail(VerifyEmailDto dto);

        Task<ApiResponse<UserToken>> Login(LoginDto dto);

        Task<ApiResponse<UserToken>> RefreshToken(string refreshToken);

        Task<ApiResponse<string>> ForgotPassword(string email);

        Task<ApiResponse<string>> ResetPassword(ResetPasswordDto dto);
    }
}
