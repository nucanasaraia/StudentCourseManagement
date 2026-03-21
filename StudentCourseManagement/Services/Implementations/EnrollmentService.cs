using AutoMapper;
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
    public class EnrollmentService : IEnrollmentService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EnrollmentService> _logger;

        public EnrollmentService(DataContext context, IMapper mapper, ILogger<EnrollmentService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<EnrollmentDto>> CreateEnrollment(int userId, int courseId)
        {
            _logger.LogInformation("User {UserId} enrolling in course {CourseId}", userId, courseId);

            try
            {
                var exists = await _context.Enrollments
                    .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);

                if (exists)
                {
                    _logger.LogWarning("User {UserId} already enrolled in course {CourseId}", userId, courseId);
                    return ApiResponseFactory.Fail<EnrollmentDto>("Already enrolled", HttpStatusCode.BadRequest);
                }

                var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId);

                if (!courseExists)
                {
                    _logger.LogWarning("Course {CourseId} not found for user {UserId}", courseId, userId);
                    return ApiResponseFactory.NotFound<EnrollmentDto>("Course not found");
                }

                var enrollment = new Enrollment
                {
                    UserId = userId,
                    CourseId = courseId
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Enrollment created successfully for user {UserId} in course {CourseId}", userId, courseId);

                var data = _mapper.Map<EnrollmentDto>(enrollment);
                return ApiResponseFactory.Success(data, "Enrollment created successfully", HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Enrollment failed for user {UserId}", userId);
                throw;
            }
        }

        public async Task<ApiResponse<List<CourseDto>>> GetCoursesByUserId(int userId)
        {
            var exists = await _context.Users.AnyAsync(s => s.Id == userId);
            if (!exists)
                return ApiResponseFactory.NotFound<List<CourseDto>>($"Student with Id {userId} does not exist");

            var courses = await _context.Enrollments
                .Where(e => e.UserId == userId)
                .Include(e => e.Course)
                .Select(e => e.Course)
                .ToListAsync();

            var data = _mapper.Map<List<CourseDto>>(courses);
            return ApiResponseFactory.Success(data, "Courses retrieved successfully");
        }
    }
}