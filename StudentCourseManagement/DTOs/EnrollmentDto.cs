namespace StudentCourseManagement.DTOs
{
    public class EnrollmentDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }

        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
    }
}
