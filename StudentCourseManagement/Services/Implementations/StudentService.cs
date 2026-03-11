using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentCourseManagement.CORE;
using StudentCourseManagement.Data;
using StudentCourseManagement.DTOs;
using StudentCourseManagement.Models;
using StudentCourseManagement.Requests;
using StudentCourseManagement.Services.Interfaces;
using System.Runtime.InteropServices;

namespace StudentCourseManagement.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public StudentService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<StudentDto>> CreateStudent(AddStudent request)
        {
            try
            {
                var student = _mapper.Map<Student>(request);
                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                var data = _mapper.Map<StudentDto>(student);
                return ApiResponseFactory.CreateSuccessResponse(data);
            }
            catch(Exception ex)
            {
                return ApiResponseFactory.CreateErrorResponse<StudentDto>($"An Error occurred: {ex.Message}");
            }
                
        }

        public async Task<ApiResponse<bool>> DeleteStudent(int id)
        {
            try
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
                if (student == null)
                {
                    return ApiResponseFactory.CreateNotFoundResponse<bool>($"Student with Id {id} does not exist");
                }

                _context.Students.Remove(student);
                await _context.SaveChangesAsync();

                return ApiResponseFactory.CreateSuccessResponse(true, $"Student with Id {id} has been deleted successfully");
            }
            catch(Exception ex)
            {
                return ApiResponseFactory.CreateErrorResponse<bool>($"An Error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentDto>> GetStudentById(int id)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                {
                    return ApiResponseFactory.CreateNotFoundResponse<StudentDto>($"Student with Id {id} does not exist");
                }

                var data = _mapper.Map<StudentDto>(student);
                return ApiResponseFactory.CreateSuccessResponse(data);
            }
            catch(Exception ex)
            {
                return ApiResponseFactory.CreateErrorResponse<StudentDto>($"An Error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<StudentDto>>> GetStudents()
        {
            try
            {
                var students = await _context.Students.ToListAsync();
                if (!students.Any())
                {
                    return ApiResponseFactory.CreateNotFoundResponse<List<StudentDto>>("No students found");
                }

                var data = _mapper.Map<List<StudentDto>>(students);
                return ApiResponseFactory.CreateSuccessResponse(data);
            }
            catch(Exception ex)
            {
                return ApiResponseFactory.CreateErrorResponse<List<StudentDto>>($"An Error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentDto>> UpdateStudent(int id, AddStudent request)
        {
            try
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
                if (student == null)
                {
                    return ApiResponseFactory.CreateNotFoundResponse<StudentDto>($"Student with Id {id} does not exist");
                }

                _mapper.Map(request, student);
                await _context.SaveChangesAsync();

                var data = _mapper.Map<StudentDto>(student);
                return ApiResponseFactory.CreateSuccessResponse(data, $"Student with Id {id} has been updated successfully");
            }

            catch(Exception ex)
            {
                return ApiResponseFactory.CreateErrorResponse<StudentDto>($"An Error occurred: {ex.Message}");
            }
        }
    }
}
