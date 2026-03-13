using StudentCourseManagement.Middleware;

namespace StudentCourseManagement.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void ConfigureMiddleware(this WebApplication app)
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.MapControllers();
        }
    }
}