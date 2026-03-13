using Microsoft.EntityFrameworkCore;
using StudentCourseManagement.Data;

namespace StudentCourseManagement.Extensions
{
    public static class DatabaseExtensions
    {
        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}