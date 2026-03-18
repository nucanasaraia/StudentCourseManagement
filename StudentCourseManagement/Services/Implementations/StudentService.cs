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
            var student = _mapper.Map<Student>(request);

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            var data = _mapper.Map<StudentDto>(student);

            return ApiResponseFactory.Success(
                data,
                "Student created successfully",
                HttpStatusCode.Created
            );
        }

        public async Task<ApiResponse<bool>> DeleteStudent(int id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return ApiResponseFactory.NotFound<bool>($"Student with Id {id} does not exist");

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return ApiResponseFactory.Success(true, $"Student with Id {id} deleted successfully");
        }

        public async Task<ApiResponse<StudentDto>> GetStudentById(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
                return ApiResponseFactory.NotFound<StudentDto>($"Student with Id {id} does not exist");

            var data = _mapper.Map<StudentDto>(student);

            return ApiResponseFactory.Success(data);
        }

        public async Task<ApiResponse<List<StudentDto>>> GetStudents()
        {
            var students = await _context.Students.ToListAsync();

            var data = _mapper.Map<List<StudentDto>>(students);

            return ApiResponseFactory.Success(data);
        }

        public async Task<ApiResponse<StudentDto>> UpdateStudent(int id, AddStudent request)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return ApiResponseFactory.NotFound<StudentDto>($"Student with Id {id} does not exist");

            _mapper.Map(request, student);
            await _context.SaveChangesAsync();

            var data = _mapper.Map<StudentDto>(student);

            return ApiResponseFactory.Success(data, $"Student with Id {id} updated successfully");
        }
    }
}