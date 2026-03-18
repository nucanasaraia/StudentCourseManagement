using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentCourseManagement.Requests;
using StudentCourseManagement.Services.Interfaces;

namespace StudentCourseManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpPost]
        public async Task<IActionResult> AddEnrollment(AddEnrollment request)
        {
            var result = await _enrollmentService.CreateEnrollment(request);
            return StatusCode((int)result.Status, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCoursesByStudentId(int id)
        {
            var result = await _enrollmentService.GetCoursesByStudentId(id);
            return StatusCode((int)result.Status, result);
        }
    }
}