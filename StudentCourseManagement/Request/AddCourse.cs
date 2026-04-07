namespace StudentCourseManagement.Requests
{
    public class AddCourse
    {
        public required string Title { get; set; }
        public int Credits { get; set; }
    }
}
