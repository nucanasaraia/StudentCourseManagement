using StudentCourseManagement.Services.Interfaces;
using StudentCourseManagement.Services.Implementations;

namespace StudentCourseManagement.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
        }
    }
}