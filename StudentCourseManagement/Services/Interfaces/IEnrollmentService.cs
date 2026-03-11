using StudentCourseManagement.CORE;
using StudentCourseManagement.DTOs;
using StudentCourseManagement.Models;
using StudentCourseManagement.Requests;

namespace StudentCourseManagement.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<ApiResponse<EnrollmentDto>> CreateEnrollment(AddEnrollment request);
        Task<ApiResponse<List<CourseDto>>> GetCoursesByStudentId(int id);
    }
}
