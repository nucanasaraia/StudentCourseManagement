namespace StudentCourseManagement.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public bool EmailVerified { get; set; }

        // Email verification
        public string? VerificationCode { get; set; }
        public int VerificationAttempts { get; set; }
        public DateTime? VerificationCodeExpires { get; set; }

        // Password reset
        public string? PasswordResetTokenHash { get; set; }
        public DateTime? PasswordResetTokenExpires { get; set; }

        public List<Enrollment> Enrollments { get; set; } = new();
        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }
}
