using StudentCourseManagement.CORE;
using StudentCourseManagement.DTOs;
using StudentCourseManagement.Models;
using StudentCourseManagement.Requests;

namespace StudentCourseManagement.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<ApiResponse<EnrollmentDto>> CreateEnrollment(int userId, int courseId);
        Task<ApiResponse<List<CourseDto>>> GetCoursesByUserId(int userId);
    }
}
