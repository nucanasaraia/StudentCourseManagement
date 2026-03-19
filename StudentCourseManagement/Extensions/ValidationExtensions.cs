using FluentValidation;
using FluentValidation.AspNetCore;

namespace StudentCourseManagement.Extensions
{
    public static class ValidationExtensions
    {
        public static void ConfigureValidation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
        }
    }
}