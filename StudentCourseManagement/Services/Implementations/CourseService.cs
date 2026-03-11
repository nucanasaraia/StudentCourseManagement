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
    public class CourseService : ICourseService
    {
        private readonly DataContext _context;
        private IMapper _mapper;

        public CourseService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<CourseDto>> CreateCourse(AddCourse request)
        {
            try
            {
                var course = _mapper.Map<Course>(request);
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                var data = _mapper.Map<CourseDto>(course);
                return ApiResponseFactory.CreateSuccessResponse(data);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.CreateErrorResponse<CourseDto>(ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> DeleteCourse(int id)
        {
            try
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
                if (course == null)
                {
                    return ApiResponseFactory.CreateErrorResponse<bool>("Course not found");
                }

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                return ApiResponseFactory.CreateSuccessResponse(true, "Course deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.CreateErrorResponse<bool>(ex.Message);
            }
        }

        public async Task<ApiResponse<List<CourseDto>>> GetCourses()
        {
            try
            {
                var courses = await _context.Courses.ToListAsync();
                var data = _mapper.Map<List<CourseDto>>(courses);

                return ApiResponseFactory.CreateSuccessResponse(data);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.CreateErrorResponse<List<CourseDto>>(ex.Message);
            }
        }

        public async Task<ApiResponse<CourseDto>> UpdateCourse(int id, AddCourse request)
        {
            try
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
                if (course == null)
                {
                    return ApiResponseFactory.CreateErrorResponse<CourseDto>("Course not found");
                }

                _mapper.Map(request, course);
                await _context.SaveChangesAsync();

                var data = _mapper.Map<CourseDto>(course);
                return ApiResponseFactory.CreateSuccessResponse(data, "Course updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.CreateErrorResponse<CourseDto>(ex.Message);
            }
    }
}
