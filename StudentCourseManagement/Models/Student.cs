namespace StudentCourseManagement.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } 

        public List<Enrollment> Enrollments { get; set; } = new();
    }
}
