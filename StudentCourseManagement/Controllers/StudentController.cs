using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentCourseManagement.Requests;
using StudentCourseManagement.Services.Interfaces;

namespace StudentCourseManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            var result = await _studentService.GetStudents();
            return StatusCode((int)result.Status, result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var result = await _studentService.GetStudentById(id);
            return StatusCode((int)result.Status, result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateStudent(AddStudent request)
        {
            var result = await _studentService.CreateStudent(request);
            return StatusCode((int)result.Status, result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateStudent(int id, AddStudent request)
        {
            var result = await _studentService.UpdateStudent(id, request);
            return StatusCode((int)result.Status, result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var result = await _studentService.DeleteStudent(id);
            return StatusCode((int)result.Status, result);
        }
    }
}