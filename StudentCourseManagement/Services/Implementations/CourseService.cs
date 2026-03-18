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
    public class CourseService : ICourseService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CourseService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<CourseDto>> CreateCourse(AddCourse request)
        {
            var course = _mapper.Map<Course>(request);
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var data = _mapper.Map<CourseDto>(course);
            return ApiResponseFactory.Success(data, "Course created successfully", HttpStatusCode.Created);
        }

        public async Task<ApiResponse<bool>> DeleteCourse(int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null)
                return ApiResponseFactory.NotFound<bool>($"Course with Id {id} not found");

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return ApiResponseFactory.Success(true, "Course deleted successfully");
        }

        public async Task<ApiResponse<List<CourseDto>>> GetCourses()
        {
            var courses = await _context.Courses.ToListAsync();

            if (!courses.Any())
                return ApiResponseFactory.NotFound<List<CourseDto>>("No courses found");

            var data = _mapper.Map<List<CourseDto>>(courses);
            return ApiResponseFactory.Success(data, "Courses retrieved successfully");
        }

        public async Task<ApiResponse<CourseDto>> UpdateCourse(int id, AddCourse request)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null)
                return ApiResponseFactory.NotFound<CourseDto>($"Course with Id {id} not found");

            _mapper.Map(request, course);
            await _context.SaveChangesAsync();

            var data = _mapper.Map<CourseDto>(course);
            return ApiResponseFactory.Success(data, "Course updated successfully");
        }
    }
}