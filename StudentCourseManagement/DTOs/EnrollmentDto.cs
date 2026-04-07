namespace StudentCourseManagement.DTOs
{
    public class EnrollmentDto
    {
        public int StudentId { get; set; }
        public required string StudentName { get; set; }

        public int CourseId { get; set; }
        public required string CourseTitle { get; set; }
    }
}
