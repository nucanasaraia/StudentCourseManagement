using StudentCourseManagement.CORE;
using StudentCourseManagement.DTOs;
using StudentCourseManagement.Requests;

namespace StudentCourseManagement.Services.Interfaces
{
    public interface IStudentService
    {
        Task<ApiResponse<List<StudentDto>>> GetStudents();
        Task<ApiResponse<StudentDto>> GetStudentById(int id);
        Task<ApiResponse<StudentDto>> CreateStudent(AddStudent request);
        Task<ApiResponse<StudentDto>> UpdateStudent(int id, AddStudent request);
        Task<ApiResponse<bool>> DeleteStudent(int id);
    }
}
