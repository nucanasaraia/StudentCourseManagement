using Microsoft.AspNetCore.Authorization;
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

        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var result = await _courseService.GetCourses();
            return StatusCode((int)result.Status, result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddCourse(AddCourse request)
        {
            var result = await _courseService.CreateCourse(request);
            return StatusCode((int)result.Status, result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var result = await _courseService.DeleteCourse(id);
            return StatusCode((int)result.Status, result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCourse(int id, AddCourse request)
        {
            var result = await _courseService.UpdateCourse(id, request);
            return StatusCode((int)result.Status, result);
        }
    }
}