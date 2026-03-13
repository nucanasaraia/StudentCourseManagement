using FluentValidation;
using StudentCourseManagement.Validations;

namespace StudentCourseManagement.Extensions
{
    public static class ValidationExtensions
    {
        public static void ConfigureValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<AddStudentValidator>();
        }
    }
}