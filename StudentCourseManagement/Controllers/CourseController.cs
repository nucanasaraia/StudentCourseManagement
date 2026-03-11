using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentCourseManagement.Requests;
using StudentCourseManagement.Services.Interfaces;

namespace StudentCourseManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet()]
        public async Task<ActionResult> GetAllCourses()
        {
            var course = await _courseService.GetCourses();
            return Ok(course);
        }

        [HttpPost()]
        public async Task<ActionResult> AddCourse(AddCourse request)
        {
            var course = await _courseService.CreateCourse(request);
            return Ok(course);
        }

        [HttpDelete()]
        public async Task<ActionResult> DeleteCourse(int id)
        {
            var course = await _courseService.DeleteCourse(id);
            return Ok(course);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCourse(int id, AddCourse request)
        {
            var course = await _courseService.UpdateCourse(id, request);
            return Ok(course);
        }
    }
}
