using AutoMapper;

namespace StudentCourseManagement.Extensions
{
    public static class MappingExtensions
    {
        public static void ConfigureMapping(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Program));
            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); for bigger projects with multiple assemblies
        }
    }
}