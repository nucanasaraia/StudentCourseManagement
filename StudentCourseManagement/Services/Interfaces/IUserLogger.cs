using StudentCourseManagement.Models;

namespace StudentCourseManagement.Services.Interfaces
{
    public interface IUserLogger
    {
        void LogInfo(User? user, string message, params object[] args);
        void LogWarning(User? user, string message, params object[] args);
        void LogError(User? user, Exception? ex, string message, params object[] args);
    }
}
