using StudentCourseManagement.Models;

namespace StudentCourseManagement.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        string HashToken(string token); 
    }
}
