using StudentCourseManagement.Models;

namespace StudentCourseManagement.CORE
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string Token { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; }

        public int StudentId { get; set; }

        public Student Student { get; set; }
    }
}
