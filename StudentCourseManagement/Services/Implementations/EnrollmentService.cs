using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentCourseManagement.CORE;
using StudentCourseManagement.Data;
using StudentCourseManagement.DTOs;
using StudentCourseManagement.Models;
using StudentCourseManagement.Requests;
using StudentCourseManagement.Services.Interfaces;

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
            try
            {
                var response = _mapper.Map<Enrollment>(request);
                _context.Enrollments.Add(response);
                await _context.SaveChangesAsync();

                var data = _mapper.Map<EnrollmentDto>(response);
                return ApiResponseFactory.CreateSuccessResponse(data);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.CreateErrorResponse<EnrollmentDto>(ex.Message);
            }
        }

        public async Task<ApiResponse<List<CourseDto>>> GetCoursesByStudentId(int id)
        {
            try
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
                if (student == null)
                {
                    return ApiResponseFactory.CreateErrorResponse<List<CourseDto>>("Student not found");
                }

                var courses = await _context.Enrollments
                    .Where(e => e.StudentId == id)
                    .Include(e => e.Course)
                    .Select(e => e.Course)
                    .ToListAsync();

                var data = _mapper.Map<List<CourseDto>>(courses);
                return ApiResponseFactory.CreateSuccessResponse(data);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.CreateErrorResponse<List<CourseDto>>(ex.Message);
            }
        }
    }
}
