using Microsoft.AspNetCore.Http;
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

        [HttpGet()]
        public async Task<ActionResult> GetAllStudent()
        {
            var students = await _studentService.GetStudents();
            return Ok(students);        
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetStudentById(int id)
        {
            var student = await _studentService.GetStudentById(id);
            return Ok(student);
        }

        [HttpPost()]
        public async Task<ActionResult> CreateStudent(AddStudent request)
        {
            var student = await _studentService.CreateStudent(request);
            return Ok(student);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateStudent(int id, AddStudent request)
        {
            var student = await _studentService.UpdateStudent(id, request);
            return Ok(student);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            var student = await _studentService.DeleteStudent(id);
            return Ok(student);
        }
    }
}
