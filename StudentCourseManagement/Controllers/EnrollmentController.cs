using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentCourseManagement.Requests;
using StudentCourseManagement.Services.Interfaces;
using System.Security.Claims;

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

        [Authorize(Roles = "STUDENT")]
        [HttpPost("enroll/{courseId}")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(claim, out var userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            var result = await _enrollmentService.CreateEnrollment(userId, courseId);

            return StatusCode((int)result.Status, result);
        }

        [Authorize(Roles = "STUDENT")]
        [HttpGet("my-courses")]
        public async Task<IActionResult> GetMyCourses()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(claim, out var userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            var result = await _enrollmentService.GetCoursesByUserId(userId);

            return StatusCode((int)result.Status, result);
        }
    }
}