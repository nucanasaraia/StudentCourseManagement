using StudentCourseManagement.Models;

namespace StudentCourseManagement.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(Student user);
        string GenerateRefreshToken();
        string HashToken(string token); 
    }
}
