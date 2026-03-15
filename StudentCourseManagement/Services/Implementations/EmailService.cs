using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using StudentCourseManagement.Configurations;
using StudentCourseManagement.CORE;
using StudentCourseManagement.Services.Interfaces;

namespace StudentCourseManagement.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtp;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtp = smtpSettings.Value;
        }

        public async Task<ApiResponse<string>> SendPasswordResetLinkAsync(string toEmail, string userName, string resetLink)
        {
            try
            {
                var subject = "Reset Your Password";
                var body = GeneratePasswordResetEmailTemplate(userName, resetLink);

                await SendEmailAsync(toEmail, subject, body);

                return ApiResponseFactory.CreateSuccessResponse("Password reset email sent.");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.CreateErrorResponse<string>("Email sending failed");
            }
        }

        public async Task<ApiResponse<string>> SendVerificationCodeAsync(string toEmail, string userName)
        {
            try
            {
                string verificationCode = GenerateVerificationCode();

                var subject = "Your Verification Code";
                var body = GenerateVerificationEmailTemplate(userName, verificationCode);

                await SendEmailAsync(toEmail, subject, body);

                return ApiResponseFactory.CreateSuccessResponse(verificationCode);
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.CreateErrorResponse<string>("Email sending failed");
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            using var mail = new MailMessage();

            mail.From = new MailAddress(_smtp.SenderEmail, _smtp.SenderName);
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = htmlBody;
            mail.IsBodyHtml = true;

            using var smtpClient = new SmtpClient(_smtp.Host)
            {
                Port = _smtp.Port,
                EnableSsl = _smtp.EnableSsl,
                Credentials = new NetworkCredential(_smtp.SenderEmail, _smtp.AppPassword)
            };

            await smtpClient.SendMailAsync(mail);
        }

        private static string GenerateVerificationCode()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];

            rng.GetBytes(bytes);

            var randomNumber = BitConverter.ToUInt32(bytes, 0);

            return (randomNumber % 900000 + 100000).ToString();
        }

        private static string GeneratePasswordResetEmailTemplate(string userName, string resetLink)
        {
            return $@"
<h2>Password Reset</h2>
<p>Hello {userName},</p>
<p>You requested to reset your password.</p>
<p>
<a href='{resetLink}'>Click here to reset your password</a>
</p>
<p>This link will expire in 1 hour.</p>";
        }

        private static string GenerateVerificationEmailTemplate(string userName, string code)
        {
            return $@"
<h2>Email Verification</h2>
<p>Hello {userName},</p>
<p>Your verification code is:</p>
<h1>{code}</h1>
<p>This code will expire shortly.</p>";
        }
    }
}