using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using StudentCourseManagement.Configurations;
using StudentCourseManagement.CORE;
using StudentCourseManagement.Services.Interfaces;

namespace StudentCourseManagement.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtp;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
        {
            _smtp = smtpSettings.Value;
            _logger = logger;
        }

        public async Task<ApiResponse<string>> SendPasswordResetLinkAsync(string toEmail, string userName, string resetLink)
        {
            _logger.LogInformation("Sending password reset email to {Email} for user {UserName}", toEmail, userName);
            try
            {
                var subject = "Reset Your Password";
                var body = GeneratePasswordResetEmailTemplate(userName, resetLink);

                await SendEmailAsync(toEmail, subject, body);

                _logger.LogInformation("Password reset email sent successfully to {Email}", toEmail);
                return ApiResponseFactory.Success("Password reset email sent.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", toEmail);
                throw;
            }
        }

        public async Task<ApiResponse<bool>> SendVerificationCodeAsync(string toEmail, string userName, string code)
        {
            _logger.LogInformation("Sending verification email to {Email} for user {UserName}", toEmail, userName);
            try
            {
                var subject = "Your Verification Code";
                var body = GenerateVerificationEmailTemplate(userName, code);

                await SendEmailAsync(toEmail, subject, body);

                _logger.LogInformation("Verification code email sent successfully to {Email}", toEmail);
                return ApiResponseFactory.Success(true, "Verification email sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send verification email to {Email}", toEmail);
                throw;
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            using var mail = new MailMessage
            {
                From = new MailAddress(_smtp.SenderEmail, _smtp.SenderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            using var smtpClient = new SmtpClient(_smtp.Host)
            {
                Port = _smtp.Port,
                EnableSsl = _smtp.EnableSSl,
                Credentials = new NetworkCredential(_smtp.SenderEmail, _smtp.AppPassword)
            };

            await smtpClient.SendMailAsync(mail);
        }

        private static string GeneratePasswordResetEmailTemplate(string userName, string resetLink) =>
            $@"
<h2>Password Reset</h2>
<p>Hello {userName},</p>
<p>You requested to reset your password.</p>
<p>
<a href='{resetLink}'>Click here to reset your password</a>
</p>
<p>This link will expire in 1 hour.</p>";

        private static string GenerateVerificationEmailTemplate(string userName, string code) =>
            $@"
<h2>Email Verification</h2>
<p>Hello {userName},</p>
<p>Your verification code is:</p>
<h1>{code}</h1>
<p>This code will expire shortly.</p>";
    }
}