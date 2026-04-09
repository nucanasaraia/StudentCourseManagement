using StudentCourseManagement.Middleware;

namespace StudentCourseManagement.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void ConfigureMiddleware(this WebApplication app)
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();

                app.UseSwagger();
                app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.MapControllers();
        }
    }
}