// <copyright file="EmailServiceTests.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.Tests.BLL.Services
{
    using System;
    using System.Threading.Tasks;
    using GraphAn.BLL.Services;
    using GraphAn.DAL.Models;
    using GraphAn.DAL.Repositories;
    using GraphAn.Tests.Helpers;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;
    using FluentAssertions;

    /// <summary>
    /// Тести для сервісу роботи з електронною поштою.
    /// </summary>
    public class EmailServiceTests
    {
        private readonly Mock<ILogger<EmailService>> _loggerMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRegistrationRepository> _registrationRepositoryMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly TestEmailService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailServiceTests"/> class.
        /// </summary>
        public EmailServiceTests()
        {
            _loggerMock = new Mock<ILogger<EmailService>>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _registrationRepositoryMock = new Mock<IRegistrationRepository>();
            _userManagerMock = MockHelpers.MockUserManager();
            _service = new TestEmailService(
                _loggerMock.Object,
                _userRepositoryMock.Object,
                _registrationRepositoryMock.Object,
                _userManagerMock.Object);
        }

        /// <summary>
        /// Тестовий підклас для підміни методу відправки email.
        /// </summary>
        private class TestEmailService : EmailService
        {
            public TestEmailService(
                ILogger<EmailService> logger,
                IUserRepository userRepository,
                IRegistrationRepository registrationRepository,
                UserManager<User> userManager)
                : base(logger, userRepository, registrationRepository, userManager)
            {
            }

            public bool EmailSendingShouldSucceed { get; set; } = true;

            protected override async Task<bool> SendVerificationCodeAsync(string email, string code)
            {
                return await Task.FromResult(EmailSendingShouldSucceed);
            }

            protected override async Task<bool> SendEmailAsync(string toEmail, string subject, string bodyHtml)
            {
                return await Task.FromResult(EmailSendingShouldSucceed);
            }
        }

        #region StartRegistrationAsync

        /// <summary>
        /// Тест: успішна реєстрація з коректними даними.
        /// </summary>
        [Fact]
        public async Task StartRegistrationAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var email = "test@example.com";
            var password = "Password123";
            var username = "testuser";
            _userRepositoryMock.Setup(r => r.IfEmailExistsAsync(email)).ReturnsAsync(false);
            _userManagerMock.Setup(m => m.FindByNameAsync(username)).ReturnsAsync(null as User);
            _registrationRepositoryMock.Setup(r => r.IfEmailExistsAsync(email)).ReturnsAsync(false);
            Registration? savedRegistration = null;
            _registrationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Registration>()))
                .Callback<Registration>(reg => savedRegistration = reg)
                .ReturnsAsync(1);
            _service.EmailSendingShouldSucceed = true;

            // Act
            var result = await _service.StartRegistrationAsync(email, password, username);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Contain("Код було відправлено на вашу пошту");
            savedRegistration.Should().NotBeNull();
            savedRegistration!.Email.Should().Be(email);
            savedRegistration.TempUsername.Should().Be(username);
            _registrationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Registration>()), Times.Once);
        }

        /// <summary>
        /// Тест: некоректний email.
        /// </summary>
        [Fact]
        public async Task StartRegistrationAsync_InvalidEmail_ReturnsFailure()
        {
            // Arrange
            var email = "invalid-email";
            var password = "Password123";

            // Act
            var result = await _service.StartRegistrationAsync(email, password, null);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Некоректний email");
            _registrationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Registration>()), Times.Never);
        }

        /// <summary>
        /// Тест: некоректний пароль.
        /// </summary>
        [Fact]
        public async Task StartRegistrationAsync_InvalidPassword_ReturnsFailure()
        {
            // Arrange
            var email = "test@example.com";
            var password = "123";

            // Act
            var result = await _service.StartRegistrationAsync(email, password, null);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Некоректний пароль");
        }

        /// <summary>
        /// Тест: email вже існує.
        /// </summary>
        [Fact]
        public async Task StartRegistrationAsync_EmailAlreadyExists_ReturnsFailure()
        {
            // Arrange
            var email = "existing@example.com";
            _userRepositoryMock.Setup(r => r.IfEmailExistsAsync(email)).ReturnsAsync(true);

            // Act
            var result = await _service.StartRegistrationAsync(email, "Password123", null);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Користувач з таким email уже існує");
        }

        /// <summary>
        /// Тест: username вже існує.
        /// </summary>
        [Fact]
        public async Task StartRegistrationAsync_UsernameAlreadyExists_ReturnsFailure()
        {
            // Arrange
            var email = "test@example.com";
            var username = "existinguser";
            _userRepositoryMock.Setup(r => r.IfEmailExistsAsync(email)).ReturnsAsync(false);
            _userManagerMock.Setup(m => m.FindByNameAsync(username)).ReturnsAsync(new User());

            // Act
            var result = await _service.StartRegistrationAsync(email, "Password123", username);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Користувач з таким іменем вже існує");
        }

        /// <summary>
        /// Тест: вже є активна реєстрація (тимчасовий запис).
        /// </summary>
        [Fact]
        public async Task StartRegistrationAsync_ActiveRegistrationExists_ReturnsFailure()
        {
            // Arrange
            var email = "test@example.com";
            _userRepositoryMock.Setup(r => r.IfEmailExistsAsync(email)).ReturnsAsync(false);
            _userManagerMock.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(null as User);
            _registrationRepositoryMock.Setup(r => r.IfEmailExistsAsync(email)).ReturnsAsync(true);

            // Act
            var result = await _service.StartRegistrationAsync(email, "Password123", null);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Користувач з таким email уже створив запит на реєстрацію");
        }

        /// <summary>
        /// Тест: помилка при відправці коду.
        /// </summary>
        [Fact]
        public async Task StartRegistrationAsync_SendCodeFails_ReturnsFailure()
        {
            // Arrange
            var email = "test@example.com";
            _userRepositoryMock.Setup(r => r.IfEmailExistsAsync(email)).ReturnsAsync(false);
            _registrationRepositoryMock.Setup(r => r.IfEmailExistsAsync(email)).ReturnsAsync(false);
            _service.EmailSendingShouldSucceed = false;

            // Act
            var result = await _service.StartRegistrationAsync(email, "Password123", "user");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Помилка надсилання коду.");
            _registrationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Registration>()), Times.Never);
        }

        #endregion

        #region ConfirmRegistrationAsync

        /// <summary>
        /// Тест: успішне підтвердження реєстрації.
        /// </summary>
        [Fact]
        public async Task ConfirmRegistrationAsync_ValidCode_ReturnsSuccess()
        {
            // Arrange
            var email = "test@example.com";
            var code = "123456";
            var registration = new Registration
            {
                Email = email,
                VerificationCode = code,
                CodeExpiryTime = DateTime.UtcNow.AddMinutes(5),
                TempUsername = "testuser",
                TempPasswordHash = "hashed"
            };
            _userRepositoryMock.Setup(r => r.IfEmailExistsAsync(email)).ReturnsAsync(false);
            _registrationRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(registration);
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(1);
            _registrationRepositoryMock.Setup(r => r.DeleteAsync(registration)).ReturnsAsync(1);

            // Act
            var result = await _service.ConfirmRegistrationAsync(email, code);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Успішно зареєстровано");
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.DeleteAsync(registration), Times.Once);
        }

        /// <summary>
        /// Тест: некоректний email.
        /// </summary>
        [Fact]
        public async Task ConfirmRegistrationAsync_InvalidEmail_ReturnsFailure()
        {
            // Act
            var result = await _service.ConfirmRegistrationAsync("invalid", "123456");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Некоректний email");
        }

        /// <summary>
        /// Тест: некоректний код.
        /// </summary>
        [Fact]
        public async Task ConfirmRegistrationAsync_InvalidCode_ReturnsFailure()
        {
            // Act
            var result = await _service.ConfirmRegistrationAsync("test@example.com", "123");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Некоректний код");
        }

        /// <summary>
        /// Тест: користувач вже існує.
        /// </summary>
        [Fact]
        public async Task ConfirmRegistrationAsync_UserAlreadyExists_ReturnsFailure()
        {
            // Arrange
            var email = "test@example.com";
            _userRepositoryMock.Setup(r => r.IfEmailExistsAsync(email)).ReturnsAsync(true);

            // Act
            var result = await _service.ConfirmRegistrationAsync(email, "123456");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Користувач з таким email уже існує");
        }

        /// <summary>
        /// Тест: реєстрацію не знайдено.
        /// </summary>
        [Fact]
        public async Task ConfirmRegistrationAsync_RegistrationNotFound_ReturnsFailure()
        {
            // Arrange
            var email = "test@example.com";
            _userRepositoryMock.Setup(r => r.IfEmailExistsAsync(email)).ReturnsAsync(false);
            _registrationRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(null as Registration);

            // Act
            var result = await _service.ConfirmRegistrationAsync(email, "123456");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Не знайдено реєстрації для підтвердження");
        }

        /// <summary>
        /// Тест: невірний код.
        /// </summary>
        [Fact]
        public async Task ConfirmRegistrationAsync_WrongCode_ReturnsFailure()
        {
            // Arrange
            var email = "test@example.com";
            var registration = new Registration
            {
                Email = "test@example.com",
                VerificationCode = "111111",
                CodeExpiryTime = DateTime.UtcNow.AddMinutes(5),
                TempPasswordHash = "dummy_hash",
                TempUsername = "dummy_user"
            };
            _userRepositoryMock.Setup(r => r.IfEmailExistsAsync(email)).ReturnsAsync(false);
            _registrationRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(registration);

            // Act
            var result = await _service.ConfirmRegistrationAsync(email, "123456");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Не правильний код для підтвердження реєстрації");
        }

        /// <summary>
        /// Тест: термін дії коду вичерпано.
        /// </summary>
        [Fact]
        public async Task ConfirmRegistrationAsync_CodeExpired_ReturnsFailure()
        {
            // Arrange
            var email = "test@example.com";
            var registration = new Registration
            {
                Email = "test@example.com",
                VerificationCode = "123456",
                CodeExpiryTime = DateTime.UtcNow.AddMinutes(-1),
                TempPasswordHash = "dummy_hash",
                TempUsername = "dummy_user"
            };
            _userRepositoryMock.Setup(r => r.IfEmailExistsAsync(email)).ReturnsAsync(false);
            _registrationRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(registration);

            // Act
            var result = await _service.ConfirmRegistrationAsync(email, "123456");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Час для підтвердження реєстрації було вичерпано");
        }

        #endregion

        #region ForgotPasswordAsync

        /// <summary>
        /// Тест: успішне надсилання листа для скидання пароля.
        /// </summary>
        [Fact]
        public async Task ForgotPasswordAsync_ValidEmail_ReturnsSuccess()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { Email = email, EmailConfirmed = true };
            _userManagerMock.SetupFindByEmailAsync(email, user);
            _userManagerMock.SetupGeneratePasswordResetTokenAsync();
            _service.EmailSendingShouldSucceed = true;

            // Act
            var result = await _service.ForgotPasswordAsync(email, (e, t) => "http://link");

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Інструкції зі скидання пароля надіслано на вашу пошту.");
            _userManagerMock.Verify(m => m.GeneratePasswordResetTokenAsync(user), Times.Once);
        }

        /// <summary>
        /// Тест: некоректний email.
        /// </summary>
        [Fact]
        public async Task ForgotPasswordAsync_InvalidEmail_ReturnsFailure()
        {
            // Act
            var result = await _service.ForgotPasswordAsync("invalid", (e, t) => "link");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Некоректна електронна адреса.");
        }

        /// <summary>
        /// Тест: користувача не існує або email не підтверджено.
        /// </summary>
        [Fact]
        public async Task ForgotPasswordAsync_UserNotFoundOrNotConfirmed_ReturnsSuccessWithInfo()
        {
            // Arrange
            var email = "test@example.com";
            _userManagerMock.SetupFindByEmailAsync(email, null);

            // Act
            var result = await _service.ForgotPasswordAsync(email, (e, t) => "link");

            // Assert
            result.Success.Should().BeTrue(); // для безпеки не повідомляємо про відсутність
            result.Message.Should().Be("Якщо обліковий запис існує та email підтверджено, на нього надіслано лист з інструкціями.");
        }

        /// <summary>
        /// Тест: помилка при відправці листа.
        /// </summary>
        [Fact]
        public async Task ForgotPasswordAsync_SendEmailFails_ReturnsFailure()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { Email = email, EmailConfirmed = true };
            _userManagerMock.SetupFindByEmailAsync(email, user);
            _userManagerMock.SetupGeneratePasswordResetTokenAsync();
            _service.EmailSendingShouldSucceed = false;

            // Act
            var result = await _service.ForgotPasswordAsync(email, (e, t) => "link");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Помилка відправки листа. Спробуйте пізніше.");
        }

        #endregion

        #region ResetPasswordAsync

        /// <summary>
        /// Тест: успішне скидання пароля.
        /// </summary>
        [Fact]
        public async Task ResetPasswordAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var email = "test@example.com";
            var token = "valid-token";
            var newPassword = "NewPassword123";
            var user = new User { Email = email };
            _userManagerMock.SetupFindByEmailAsync(email, user);
            _userManagerMock.SetupResetPasswordAsyncSuccess();

            // Act
            var result = await _service.ResetPasswordAsync(email, token, newPassword);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Пароль успішно змінено. Тепер ви можете увійти з новим паролем.");
            _userManagerMock.Verify(m => m.ResetPasswordAsync(user, token, newPassword), Times.Once);
            _userManagerMock.Verify(m => m.UpdateSecurityStampAsync(user), Times.Once);
        }

        /// <summary>
        /// Тест: некоректний email.
        /// </summary>
        [Fact]
        public async Task ResetPasswordAsync_InvalidEmail_ReturnsFailure()
        {
            // Act
            var result = await _service.ResetPasswordAsync("invalid", "token", "Pass123");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Некоректні дані для скидання пароля.");
        }

        /// <summary>
        /// Тест: некоректний пароль (занадто короткий).
        /// </summary>
        [Fact]
        public async Task ResetPasswordAsync_InvalidPassword_ReturnsFailure()
        {
            // Act
            var result = await _service.ResetPasswordAsync("test@example.com", "token", "123");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Некоректні дані для скидання пароля.");
        }

        /// <summary>
        /// Тест: користувача не знайдено.
        /// </summary>
        [Fact]
        public async Task ResetPasswordAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            _userManagerMock.SetupFindByEmailAsync("test@example.com", null);

            // Act
            var result = await _service.ResetPasswordAsync("test@example.com", "token", "NewPass123");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Користувача з такою електронною адресою не знайдено.");
        }

        /// <summary>
        /// Тест: помилка при скиданні пароля (токен невірний тощо).
        /// </summary>
        [Fact]
        public async Task ResetPasswordAsync_ResetFails_ReturnsFailure()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { Email = email };
            _userManagerMock.SetupFindByEmailAsync(email, user);
            _userManagerMock.SetupResetPasswordAsyncFailure("Помилка токена");

            // Act
            var result = await _service.ResetPasswordAsync(email, "bad-token", "NewPass123");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Не вдалося скинути пароль: Помилка токена");
        }

        #endregion
    }
}