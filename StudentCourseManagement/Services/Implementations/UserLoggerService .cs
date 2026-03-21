using Serilog.Context;
using StudentCourseManagement.Models;
using StudentCourseManagement.Services.Interfaces;

namespace StudentCourseManagement.Services.Implementations
{
    public class UserLoggerService : IUserLogger
    {
        private readonly ILogger<UserLoggerService> _logger;

        public UserLoggerService(ILogger<UserLoggerService> logger)
        {
            _logger = logger;
        }

        public void LogInfo(User? user, string message, params object[] args)
        {
            if (user != null)
            {
                using (LogContext.PushProperty("UserId", user.Id))
                using (LogContext.PushProperty("Email", user.Email))
                using (LogContext.PushProperty("Role", user.Role))
                {
                    _logger.LogInformation(message, args);
                }
            }
            else
            {
                _logger.LogInformation(message, args);
            }
        }

        public void LogWarning(User? user, string message, params object[] args)
        {
            if (user != null)
            {
                using (LogContext.PushProperty("UserId", user.Id))
                using (LogContext.PushProperty("Email", user.Email))
                using (LogContext.PushProperty("Role", user.Role))
                {
                    _logger.LogWarning(message, args);
                }
            }
            else
            {
                _logger.LogWarning(message, args);
            }
        }

        public void LogError(User? user, Exception ex, string message, params object[] args)
        {
            if (user != null)
            {
                using (LogContext.PushProperty("UserId", user.Id))
                using (LogContext.PushProperty("Email", user.Email))
                using (LogContext.PushProperty("Role", user.Role))
                {
                    _logger.LogError(ex, message, args);
                }
            }
            else
            {
                _logger.LogError(ex, message, args);
            }
        }
    }
}
