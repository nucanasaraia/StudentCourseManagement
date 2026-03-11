using Microsoft.EntityFrameworkCore;
using StudentCourseManagement.Models;

namespace StudentCourseManagement.Data
{
    public class DataContext : DbContext
    {
       
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"");
        }
    }
}
