using StudentCourseManagement.CORE;
using StudentCourseManagement.DTOs;

namespace StudentCourseManagement.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> Register(StudentDto dto);
        Task<ApiResponse<string>> Login(StudentDto dto);
    }
}
