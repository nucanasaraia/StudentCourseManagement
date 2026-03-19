using StudentCourseManagement.CORE;
using StudentCourseManagement.DTOs;
using StudentCourseManagement.Requests;

namespace StudentCourseManagement.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<List<UserDto>>> GetUsers();
        Task<ApiResponse<UserDto>> GetUserById(int id);
        Task<ApiResponse<UserDto>> CreateUser(AddUser request);
        Task<ApiResponse<UserDto>> UpdateUser(int id, AddUser request);
        Task<ApiResponse<bool>> DeleteUser(int id);
    }
}
