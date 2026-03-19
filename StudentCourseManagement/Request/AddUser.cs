using StudentCourseManagement.Enum;

namespace StudentCourseManagement.Requests
{
    public class AddUser
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public USER_ROLE Role { get; internal set; }
    }
}
