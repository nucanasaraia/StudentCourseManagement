using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentCourseManagement.CORE;
using StudentCourseManagement.Data;
using StudentCourseManagement.DTOs;
using StudentCourseManagement.Models;
using StudentCourseManagement.Requests;
using StudentCourseManagement.Services.Interfaces;
using System.Net;

namespace StudentCourseManagement.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<ApiResponse<UserDto>> CreateUser(AddUser request)
        {
            var exists = await _context.Users.AnyAsync(x => x.Email == request.Email);
            if (exists)
                return ApiResponseFactory.Conflict<UserDto>("Email already exists");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Role = request.Role,
                EmailVerified = true
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var data = _mapper.Map<UserDto>(user);

            return ApiResponseFactory.Success(
                data,
                "User created successfully",
                HttpStatusCode.Created
            );
        }

        public async Task<ApiResponse<bool>> DeleteUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(s => s.Id == id);

            if (user == null)
                return ApiResponseFactory.NotFound<bool>($"user with Id {id} does not exist");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return ApiResponseFactory.Success(true, $"user with Id {id} deleted successfully");
        }

        public async Task<ApiResponse<UserDto>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return ApiResponseFactory.NotFound<UserDto>($"user with Id {id} does not exist");

            var data = _mapper.Map<UserDto>(user);

            return ApiResponseFactory.Success(data);
        }

        public async Task<ApiResponse<List<UserDto>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            var data = _mapper.Map<List<UserDto>>(users);

            return ApiResponseFactory.Success(data);
        }

        public async Task<ApiResponse<UserDto>> UpdateUser(int id, AddUser request)
        {
            var user = await _context.Users.FindAsync(id); 

            if (user == null)
                return ApiResponseFactory.NotFound<UserDto>($"user with Id {id} does not exist");

            _mapper.Map(request, user);
            await _context.SaveChangesAsync();

            var data = _mapper.Map<UserDto>(user);

            return ApiResponseFactory.Success(data, $"user with Id {id} updated successfully");
        }
    }
}