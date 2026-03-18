using StudentCourseManagement.CORE;

namespace StudentCourseManagement.Services.Interfaces
{
    public interface IEmailService
    {
        Task<ApiResponse<string>> SendPasswordResetLinkAsync(string toEmail, string userName, string resetLink);
        Task<ApiResponse<bool>> SendVerificationCodeAsync(string email, string username, string code);
    }
}
