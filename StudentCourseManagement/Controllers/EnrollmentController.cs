using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        [HttpPost()]

        public async Task<ActionResult> AddEnrollement(AddEnrollment request)
        {
            var enrollment = await _enrollmentService.CreateEnrollment(request);
            return Ok(enrollment);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetCourseByStudentId(int id)
        {
            var courses = await _enrollmentService.GetCoursesByStudentId(id);
            return Ok(courses);
        }
    }
}
