using StudentCourseManagement.CORE;
using StudentCourseManagement.DTOs;
using StudentCourseManagement.Models;
using StudentCourseManagement.Requests;

namespace StudentCourseManagement.Services.Interfaces
{
    public interface ICourseService
    {
        Task<ApiResponse<List<CourseDto>>> GetCourses();
        Task<ApiResponse<CourseDto>> CreateCourse(AddCourse request);
        Task<ApiResponse<CourseDto>> UpdateCourse(int id, AddCourse request);
        Task<ApiResponse<bool>> DeleteCourse(int id);
    }
}
