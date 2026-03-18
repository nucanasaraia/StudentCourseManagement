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

        public async Task<ApiResponse<EnrollmentDto>> CreateEnrollment(AddEnrollment request)
        {
            var enrollment = _mapper.Map<Enrollment>(request);

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var data = _mapper.Map<EnrollmentDto>(enrollment);
            return ApiResponseFactory.Success(data, "Enrollment created successfully", HttpStatusCode.Created);
        }

        public async Task<ApiResponse<List<CourseDto>>> GetCoursesByStudentId(int studentId)
        {
            var studentExists = await _context.Students.AnyAsync(s => s.Id == studentId);
            if (!studentExists)
                return ApiResponseFactory.NotFound<List<CourseDto>>($"Student with Id {studentId} does not exist");

            var courses = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                .Select(e => e.Course)
                .ToListAsync();

            var data = _mapper.Map<List<CourseDto>>(courses);
            return ApiResponseFactory.Success(data, "Courses retrieved successfully");
        }
    }
}