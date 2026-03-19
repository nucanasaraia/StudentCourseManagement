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

        public EnrollmentService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<EnrollmentDto>> CreateEnrollment(int userId, int courseId)
        {
            var exists = await _context.Enrollments
                .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (exists)
                return ApiResponseFactory.Fail<EnrollmentDto>("Already enrolled", HttpStatusCode.BadRequest);

            var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId);
            if (!courseExists)
                return ApiResponseFactory.NotFound<EnrollmentDto>("Course not found");

            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = courseId
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var data = _mapper.Map<EnrollmentDto>(enrollment);
            return ApiResponseFactory.Success(data, "Enrollment created successfully", HttpStatusCode.Created);
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