namespace StudentCourseManagement.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public List<Enrollment> Enrollments { get; set; } = new();
    }
}
