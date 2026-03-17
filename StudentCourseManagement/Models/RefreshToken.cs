namespace StudentCourseManagement.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string TokenHash { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; }

        public int StudentId { get; set; }

        public Student Student { get; set; }
    }
}
