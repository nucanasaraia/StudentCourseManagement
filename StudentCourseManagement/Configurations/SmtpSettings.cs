namespace StudentCourseManagement.Configurations
{
    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public required string SenderEmail { get; set; }
        public required string SenderName { get; set; }
        public required string AppPassword { get; set; }
        public bool EnableSSl { get; set; }
    }
}