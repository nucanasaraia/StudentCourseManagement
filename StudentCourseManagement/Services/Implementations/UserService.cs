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
        private readonly ILogger<UserService> _logger;

        public UserService(DataContext context, IMapper mapper, ILogger<UserService> logger)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = new PasswordHasher<User>();
            _logger = logger;
        }

        public async Task<ApiResponse<UserDto>> CreateUser(AddUser request)
        {
              _logger.LogInformation("Creating user with email {Email}", request.Email);
            try
            {
                var exists = await _context.Users.AnyAsync(x => x.Email == request.Email);
                if (exists)
                {
                    _logger.LogWarning("User creation failed. Email already exists: {Email}", request.Email);
                    return ApiResponseFactory.Conflict<UserDto>("Email already exists");
                }

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = _passwordHasher.HashPassword(null!, request.Password),
                    Role = request.Role,
                    EmailVerified = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User created successfully with Id {UserId}", user.Id);

                var data = _mapper.Map<UserDto>(user);

                return ApiResponseFactory.Success(
                    data,
                    "User created successfully",
                    HttpStatusCode.Created
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating user with email {Email}", request.Email);
                throw;
            }
        }

        public async Task<ApiResponse<bool>> DeleteUser(int id)
        {    
            _logger.LogInformation("Deleting user with Id {UserId}", id);
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(s => s.Id == id);

                if (user == null)
                {
                    _logger.LogWarning("User deletion failed. User with Id {UserId} not found", id);
                    return ApiResponseFactory.NotFound<bool>($"user with Id {id} does not exist");
                }
                   

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User with Id {UserId} deleted successfully", id);
                return ApiResponseFactory.Success(true, $"user with Id {id} deleted successfully");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while deleting user with Id {UserId}", id);
                throw;
            }
        }

        public async Task<ApiResponse<UserDto>> GetUserById(int id)
        {
            _logger.LogInformation("Fetching user with id {UserId}", id);
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("User not found with Id {UserId}", id);
                    return ApiResponseFactory.NotFound<UserDto>($"user with Id {id} does not exist");
                }

                var data = _mapper.Map<UserDto>(user);

                return ApiResponseFactory.Success(data);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user with Id {UserId}", id);
                throw;
            }
        }

        public async Task<ApiResponse<List<UserDto>>> GetUsers()
        {
            _logger.LogInformation("Fetching all users");
            
            try
            {
                var users = await _context.Users.ToListAsync();

                var data = _mapper.Map<List<UserDto>>(users);

                _logger.LogInformation("Fetched {Count} users", data.Count);
                return ApiResponseFactory.Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching users");
                throw;
            }
        }

        public async Task<ApiResponse<UserDto>> UpdateUser(int id, AddUser request)
        {
            _logger.LogInformation("Updating user with Id {UserId}", id);
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("User update failed. User with Id {UserId} not found", id);
                    return ApiResponseFactory.NotFound<UserDto>($"user with Id {id} does not exist");
                }

                _mapper.Map(request, user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User updated successfully with Id {UserId}", id);

                var data = _mapper.Map<UserDto>(user);

                return ApiResponseFactory.Success(data, $"user with Id {id} updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating user with Id {UserId}", id);
                throw;
            }
        }
    }
}