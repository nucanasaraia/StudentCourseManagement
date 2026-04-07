using StudentCourseManagement.Enum;

namespace StudentCourseManagement.Requests
{
    public class AddUser
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public USER_ROLE Role { get; set; }
    }
}
