using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using StudentCourseManagement.CORE;
using StudentCourseManagement.Data;
using StudentCourseManagement.DTOs;
using StudentCourseManagement.Models;
using StudentCourseManagement.Services.Interfaces;
using System.Net;

namespace StudentCourseManagement.Tests
{
    public class AuthServiceTests
    {
        // ─────────────────────────────────────────────
        // HELPERS — shared by every test
        // ─────────────────────────────────────────────

        // Creates a fresh empty in-memory database for each test.
        // We use a random Guid as the name so tests never share data.
        private DataContext CreateDb()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new DataContext(options);
        }

        // Builds a ready-to-use AuthService with fake dependencies.
        // The fakes do the minimum needed: return something sensible.
        private AuthService BuildService(DataContext db)
        {
            // Fake token service — just returns dummy strings
            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(t => t.GenerateAccessToken(It.IsAny<User>()))
                        .Returns("fake-access-token");
            tokenService.Setup(t => t.GenerateRefreshToken())
                        .Returns("fake-refresh-token");
            // HashToken just prepends "hash-" so we can predict what gets stored
            tokenService.Setup(t => t.HashToken(It.IsAny<string>()))
                        .Returns<string>(token => "hash-" + token);

            // Fake email service — always says it succeeded, never sends anything
            var emailService = new Mock<IEmailService>();
            emailService
                .Setup(e => e.SendVerificationCodeAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ApiResponse<bool> { Status = HttpStatusCode.OK });
            emailService
                .Setup(e => e.SendPasswordResetLinkAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ApiResponse<string> { Status = HttpStatusCode.OK });

            // Fake logger — does nothing, we just need it to exist
            var logger = new Mock<IUserLogger>();

            return new AuthService(db, tokenService.Object, emailService.Object, logger.Object);
        }

        // Quickly creates a verified user with a hashed password.
        // Saves us repeating this setup in every login/password test.
        private User MakeVerifiedUser(DataContext db, string username, string email, string password)
        {
            var hasher = new PasswordHasher<User>();
            var user = new User
            {
                Username = username,
                Email = email,
                EmailVerified = true,
                PasswordHash = string.Empty  // set properly on the next line
            };
            user.PasswordHash = hasher.HashPassword(user, password);
            db.Users.Add(user);
            db.SaveChanges();
            return user;
        }


        // ─────────────────────────────────────────────
        // REGISTER TESTS
        // ─────────────────────────────────────────────

        [Fact]
        public async Task Register_WithNewEmail_Succeeds()
        {
            // Arrange
            var db = CreateDb();
            var svc = BuildService(db);
            var dto = new RegisterDto { Username = "alice", Email = "alice@test.com", Password = "Pass1!" };

            // Act
            var result = await svc.Register(dto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.Status);
            // The user should now exist in the database
            Assert.True(await db.Users.AnyAsync(u => u.Email == "alice@test.com"));
        }

        [Fact]
        public async Task Register_WithExistingEmail_Fails()
        {
            // Arrange — add a user with that email first
            var db = CreateDb();
            var svc = BuildService(db);
            db.Users.Add(new User { Username = "alice", Email = "alice@test.com", PasswordHash = "x" });
            await db.SaveChangesAsync();

            // Act — try to register again with the same email
            var result = await svc.Register(
                new RegisterDto { Username = "alice2", Email = "alice@test.com", Password = "Pass1!" });

            // Assert
            Assert.NotEqual(HttpStatusCode.OK, result.Status);
            Assert.Contains("already exists", result.Message);
        }


        // ─────────────────────────────────────────────
        // VERIFY EMAIL TESTS
        // ─────────────────────────────────────────────

        [Fact]
        public async Task VerifyEmail_WithCorrectCode_MarksUserAsVerified()
        {
            // Arrange — a user who registered but hasn't verified yet
            var db = CreateDb();
            var svc = BuildService(db);
            db.Users.Add(new User
            {
                Username = "bob",
                Email = "bob@test.com",
                PasswordHash = "x",
                VerificationCode = "123456",
                VerificationCodeExpires = DateTime.UtcNow.AddMinutes(5)  // not expired
            });
            await db.SaveChangesAsync();

            // Act
            var result = await svc.VerifyEmail(
                new VerifyEmailDto { Email = "bob@test.com", Code = "123456" });

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.Status);
            var user = await db.Users.FirstAsync(u => u.Email == "bob@test.com");
            Assert.True(user.EmailVerified);
            Assert.Null(user.VerificationCode);  // code should be cleared after use
        }

        [Fact]
        public async Task VerifyEmail_WithExpiredCode_Fails()
        {
            // Arrange — code expired 1 minute ago
            var db = CreateDb();
            var svc = BuildService(db);
            db.Users.Add(new User
            {
                Username = "bob",
                Email = "bob@test.com",
                PasswordHash = "x",
                VerificationCode = "123456",
                VerificationCodeExpires = DateTime.UtcNow.AddMinutes(-1)  // already expired
            });
            await db.SaveChangesAsync();

            // Act
            var result = await svc.VerifyEmail(
                new VerifyEmailDto { Email = "bob@test.com", Code = "123456" });

            // Assert
            Assert.NotEqual(HttpStatusCode.OK, result.Status);
            Assert.Contains("expired", result.Message.ToLower());
        }

        [Fact]
        public async Task VerifyEmail_WithWrongCode_IncrementsAttemptCount()
        {
            // Arrange
            var db = CreateDb();
            var svc = BuildService(db);
            db.Users.Add(new User
            {
                Username = "bob",
                Email = "bob@test.com",
                PasswordHash = "x",
                VerificationCode = "999999",
                VerificationCodeExpires = DateTime.UtcNow.AddMinutes(5),
                VerificationAttempts = 0
            });
            await db.SaveChangesAsync();

            // Act — submit the wrong code
            await svc.VerifyEmail(new VerifyEmailDto { Email = "bob@test.com", Code = "000000" });

            // Assert — attempts counter went up by 1
            var user = await db.Users.FirstAsync(u => u.Email == "bob@test.com");
            Assert.Equal(1, user.VerificationAttempts);
        }


        // ─────────────────────────────────────────────
        // LOGIN TESTS
        // ─────────────────────────────────────────────

        [Fact]
        public async Task Login_WithCorrectCredentials_ReturnsToken()
        {
            // Arrange — a verified user with a known password
            var db = CreateDb();
            var svc = BuildService(db);
            MakeVerifiedUser(db, "carol", "carol@test.com", "Pass1!");

            // Act
            var result = await svc.Login(new LoginDto { Username = "carol", Password = "Pass1!" });

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.Status);
            Assert.Equal("fake-access-token", result.Data?.Token);  // matches our fake token service
        }

        [Fact]
        public async Task Login_WithWrongPassword_Fails()
        {
            // Arrange
            var db = CreateDb();
            var svc = BuildService(db);
            MakeVerifiedUser(db, "carol", "carol@test.com", "Pass1!");

            // Act
            var result = await svc.Login(new LoginDto { Username = "carol", Password = "WrongPass!" });

            // Assert
            Assert.NotEqual(HttpStatusCode.OK, result.Status);
        }

        [Fact]
        public async Task Login_WhenEmailNotVerified_Fails()
        {
            // Arrange — user exists but never verified their email
            var db = CreateDb();
            var svc = BuildService(db);
            db.Users.Add(new User
            {
                Username = "dave",
                Email = "dave@test.com",
                PasswordHash = "x",
                EmailVerified = false
            });
            await db.SaveChangesAsync();

            // Act
            var result = await svc.Login(new LoginDto { Username = "dave", Password = "Pass1!" });

            // Assert
            Assert.NotEqual(HttpStatusCode.OK, result.Status);
        }


        // ─────────────────────────────────────────────
        // FORGOT PASSWORD TESTS
        // ─────────────────────────────────────────────

        [Fact]
        public async Task ForgotPassword_WithUnknownEmail_StillReturnsSuccess()
        {
            // Arrange — empty database, email doesn't exist
            var db = CreateDb();
            var svc = BuildService(db);

            // Act
            var result = await svc.ForgotPassword("ghost@test.com");

            // Assert — must return success to avoid leaking whether the email exists
            Assert.Equal(HttpStatusCode.OK, result.Status);
        }

        [Fact]
        public async Task ForgotPassword_WithKnownEmail_SetsResetToken()
        {
            // Arrange
            var db = CreateDb();
            var svc = BuildService(db);
            db.Users.Add(new User
            { Username = "frank", Email = "frank@test.com", PasswordHash = "x", EmailVerified = true });
            await db.SaveChangesAsync();

            // Act
            await svc.ForgotPassword("frank@test.com");

            // Assert — reset token hash should now be stored on the user
            var user = await db.Users.FirstAsync(u => u.Email == "frank@test.com");
            Assert.NotNull(user.PasswordResetTokenHash);
        }


        // ─────────────────────────────────────────────
        // RESET PASSWORD TESTS
        // ─────────────────────────────────────────────

        [Fact]
        public async Task ResetPassword_WithValidToken_ChangesPassword()
        {
            // Arrange — user already has a reset token stored
            var db = CreateDb();
            var svc = BuildService(db);
            db.Users.Add(new User
            {
                Username = "grace",
                Email = "grace@test.com",
                PasswordHash = "old-hash",
                // "hash-mytoken" is what our fake HashToken returns for "mytoken"
                PasswordResetTokenHash = "hash-mytoken",
                PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1)
            });
            await db.SaveChangesAsync();

            // Act
            var result = await svc.ResetPassword(
                new ResetPasswordDto { Token = "mytoken", NewPassword = "NewPass1!" });

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.Status);
            var user = await db.Users.FirstAsync(u => u.Email == "grace@test.com");
            Assert.NotEqual("old-hash", user.PasswordHash);  // password was changed
            Assert.Null(user.PasswordResetTokenHash);         // token was cleared
        }

        [Fact]
        public async Task ResetPassword_WithExpiredToken_Fails()
        {
            // Arrange — token exists but expired an hour ago
            var db = CreateDb();
            var svc = BuildService(db);
            db.Users.Add(new User
            {
                Username = "grace",
                Email = "grace@test.com",
                PasswordHash = "old-hash",
                PasswordResetTokenHash = "hash-expiredtoken",
                PasswordResetTokenExpires = DateTime.UtcNow.AddHours(-1)  // expired
            });
            await db.SaveChangesAsync();

            // Act
            var result = await svc.ResetPassword(
                new ResetPasswordDto { Token = "expiredtoken", NewPassword = "NewPass1!" });

            // Assert
            Assert.NotEqual(HttpStatusCode.OK, result.Status);
        }
    }
}
