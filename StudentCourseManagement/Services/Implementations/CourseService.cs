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
        private readonly ILogger<CourseService> _logger;
        public CourseService(DataContext context, IMapper mapper, ILogger<CourseService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<CourseDto>> CreateCourse(AddCourse request)
        {
            _logger.LogInformation("Creating a new course with Title: {CourseName}", request.Title);
            try
            {
                var course = _mapper.Map<Course>(request);
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                var data = _mapper.Map<CourseDto>(course);

                _logger.LogInformation("Course with Title: {CourseName} created successfully with Id: {CourseId}", course.Title, course.Id);
                return ApiResponseFactory.Success(data, "Course created successfully", HttpStatusCode.Created);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating course with Title: {CourseName}", request.Title);
                throw;
            }
        }

        public async Task<ApiResponse<bool>> DeleteCourse(int id)
        {
            _logger.LogInformation("Deleting course with id {CourseId}", id);
            try
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
                if (course == null)
                {
                    _logger.LogWarning("Course with Id {CourseId} not found for deletion", id);
                    return ApiResponseFactory.NotFound<bool>($"Course with Id {id} not found");
                }

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Course with id {CourseId} deleted successfully", id);
                return ApiResponseFactory.Success(true, "Course deleted successfully");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting course with Id: {CourseId}", id);
                throw;
            }
        }

        public async Task<ApiResponse<List<CourseDto>>> GetCourses()
        {
            _logger.LogInformation("Fetching all courses");
            try
            {
                var courses = await _context.Courses.ToListAsync();

                if (!courses.Any())
                {
                    _logger.LogWarning("No courses found in the database");
                    return ApiResponseFactory.NotFound<List<CourseDto>>("No courses found");
                }

                var data = _mapper.Map<List<CourseDto>>(courses);
                return ApiResponseFactory.Success(data, "Courses retrieved successfully");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching courses");
                throw;
            }
        }

        public async Task<ApiResponse<CourseDto>> UpdateCourse(int id, AddCourse request)
        {
            _logger.LogInformation("Updating course with id {CourseId}", id);
            try
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
                if (course == null)
                {
                    _logger.LogWarning("Course with id {CourseId} not found", id);
                    return ApiResponseFactory.NotFound<CourseDto>($"Course with Id {id} not found");
                }

                _mapper.Map(request, course);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Course with id {CourseId} updated successfully", id);

                var data = _mapper.Map<CourseDto>(course);
                return ApiResponseFactory.Success(data, "Course updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating course with Id: {CourseId}", id);
                throw;
            }
        }
    }
}