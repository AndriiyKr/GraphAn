// <copyright file="EmailService.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Services
{
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using GraphAn.BLL.Interfaces;
    using GraphAn.DAL.Models;
    using GraphAn.DAL.Repositories;
    using MailKit.Net.Smtp;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using MimeKit;

    /// <summary>
    /// Сервіс для реалізації методів роботи з Email.
    /// </summary>
    public class EmailService : IEmailService
    {
        private static readonly User SystemUser = new ()
        {
            UserName = "system",
            Email = "system@local",
            PasswordHash = string.Empty,
        };

        private readonly ILogger<EmailService> logger;
        private readonly IUserRepository userRepository;
        private readonly IRegistrationRepository registrationRepository;
        private readonly UserManager<User> userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <param name="logger">Об'єкт логера.</param>
        /// <param name="userRepository">Об'єкт репозиторію користувача.</param>
        /// <param name="registrationRepository">Об'єкт репозиторію реєстрації.</param>
        /// <param name="userManager">Менеджер користувачів Identity.</param>
        public EmailService(
            ILogger<EmailService> logger,
            IUserRepository userRepository,
            IRegistrationRepository registrationRepository,
            UserManager<User> userManager)
        {
            this.logger = logger;
            this.userRepository = userRepository;
            this.registrationRepository = registrationRepository;
            this.userManager = userManager;
        }

        /// <inheritdoc/>
        public async Task<(bool Success, string Message)> StartRegistrationAsync(
            string? email,
            string password,
            string? username = null)
        {
            if (this.IsEmailInvalid(email))
            {
                this.logger.LogWarning("Передано некоректний email: {Email}", email);
                return (false, "Некоректний email");
            }

            if (this.IsPasswordInvalid(password))
            {
                this.logger.LogWarning("Передано некоректний пароль при реєстрації");
                return (false, "Некоректний пароль");
            }

            if (await this.userRepository.IfEmailExistsAsync(email!))
            {
                this.logger.LogWarning("Користувач з таким email уже існує: {Email}", email);
                return (false, "Користувач з таким email уже існує");
            }

            // Перевірка унікальності імені користувача через UserManager
            if (!string.IsNullOrWhiteSpace(username))
            {
                var existingUserByUsername = await this.userManager.FindByNameAsync(username);
                if (existingUserByUsername != null)
                {
                    this.logger.LogWarning("Користувач з таким іменем уже існує: {Username}", username);
                    return (false, "Користувач з таким іменем вже існує");
                }
            }

            if (await this.registrationRepository.IfEmailExistsAsync(email!))
            {
                this.logger.LogWarning("Користувач з таким email уже створив запит на реєстрацію: {Email}", email);
                return (false, "Користувач з таким email уже створив запит на реєстрацію");
            }

            string verificationCode = this.GenerateCode();
            var registration = new Registration
            {
                Id = Guid.NewGuid(),
                Email = email!,
                VerificationCode = verificationCode,
                CodeExpiryTime = DateTime.UtcNow.AddMinutes(10),
                TempPasswordHash = this.GetPasswordHash(password!),
                TempUsername = username ?? "Anonymous user",
            };

            bool sendResult = await this.SendVerificationCodeAsync(email!, verificationCode);
            if (!sendResult)
            {
                this.logger.LogWarning("Помилка надсилання коду користувачу: {Email}", email);
                return (false, "Помилка надсилання коду.");
            }

            await this.registrationRepository.AddAsync(registration);
            this.logger.LogInformation("Тимчасовий запис реєстрації створено для користувача: {Email}", email);

            return (true, "Запис успішно створено. Код було відправлено на вашу пошту.");
        }

        /// <inheritdoc/>
        public async Task<(bool Success, string Message)> ConfirmRegistrationAsync(string? email, string? code)
        {
            if (this.IsEmailInvalid(email))
            {
                this.logger.LogWarning("Передано некоректний email: {Email}", email);
                return (false, "Некоректний email");
            }

            if (this.IsCodeInvalid(code))
            {
                this.logger.LogWarning("Передано некоректний code: {Code}", code);
                return (false, "Некоректний код");
            }

            if (await this.userRepository.IfEmailExistsAsync(email!))
            {
                this.logger.LogWarning("Користувач з таким email уже існує: {Email}", email);
                return (false, "Користувач з таким email уже існує");
            }

            var registration = await this.registrationRepository.GetByEmailAsync(email!);
            if (registration == null)
            {
                this.logger.LogWarning("Не знайдено реєстрації для підтвердження користувача: {Email}", email);
                return (false, "Не знайдено реєстрації для підтвердження");
            }

            if (!string.Equals(registration.VerificationCode, code, StringComparison.Ordinal))
            {
                this.logger.LogInformation("Не правильний код для підтвердження реєстрації користувача: {Email}", email);
                return (false, "Не правильний код для підтвердження реєстрації");
            }

            if (registration.CodeExpiryTime < DateTime.UtcNow)
            {
                this.logger.LogInformation("Час для підтвердження реєстрації користувача: {Email} було вичерпано", email);
                return (false, "Час для підтвердження реєстрації було вичерпано");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email!,
                UserName = registration.TempUsername ?? "Anonymous user",
                PasswordHash = registration.TempPasswordHash,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };

            // Нормалізація для Identity
            user.NormalizedUserName = user.UserName.ToUpperInvariant();
            user.NormalizedEmail = user.Email.ToUpperInvariant();

            await this.userRepository.AddAsync(user);
            await this.registrationRepository.DeleteAsync(registration);

            this.logger.LogInformation("Користувача: {Email} було успішно зареєстровано", email);
            return (true, "Успішно зареєстровано");
        }

        /// <inheritdoc/>
        public async Task<(bool Success, string Message)> ForgotPasswordAsync(string email, Func<string, string, string> resetLinkGenerator)
        {
            if (string.IsNullOrWhiteSpace(email) || !this.IsEmailValid(email))
            {
                this.logger.LogWarning("Передано некоректний email для скидання пароля: {Email}", email);
                return (false, "Некоректна електронна адреса.");
            }

            var user = await this.userManager.FindByEmailAsync(email);
            if (user == null || !user.EmailConfirmed)
            {
                this.logger.LogWarning("Спроба скидання пароля для неіснуючого або непідтвердженого email: {Email}", email);
                return (true, "Якщо обліковий запис існує та email підтверджено, на нього надіслано лист з інструкціями.");
            }

            var token = await this.userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = resetLinkGenerator(user.Email!, token);

            string bodyHtml = $"<p>Для скидання пароля перейдіть за посиланням: <a href='{resetLink}'>скинути пароль</a></p><p>Якщо ви не ініціювали скидання, проігноруйте цей лист.</p>";
            var sendResult = await this.SendEmailAsync(
                email,
                "Відновлення пароля",
                bodyHtml);
            if (!sendResult)
            {
                this.logger.LogError("Не вдалося надіслати лист для скидання пароля {Email}", email);
                return (false, "Помилка відправки листа. Спробуйте пізніше.");
            }

            this.logger.LogInformation("Надіслано лист для скидання пароля для {Email}", email);
            return (true, "Інструкції зі скидання пароля надіслано на вашу пошту.");
        }

        /// <inheritdoc/>
        public async Task<(bool Success, string Message)> ResetPasswordAsync(string email, string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || !this.IsEmailValid(email) ||
                string.IsNullOrWhiteSpace(token) || this.IsPasswordInvalid(newPassword))
            {
                this.logger.LogWarning("Некоректні дані для скидання пароля: {Email}", email);
                return (false, "Некоректні дані для скидання пароля.");
            }

            var user = await this.userManager.FindByEmailAsync(email);
            if (user == null)
            {
                this.logger.LogWarning("Користувача з email {Email} не знайдено для скидання пароля", email);
                return (false, "Користувача з такою електронною адресою не знайдено.");
            }

            var result = await this.userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                await this.userManager.UpdateSecurityStampAsync(user);
                this.logger.LogInformation("Пароль успішно змінено для {Email}", email);
                return (true, "Пароль успішно змінено. Тепер ви можете увійти з новим паролем.");
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            this.logger.LogWarning("Помилка скидання пароля для {Email}: {Errors}", email, errors);
            return (false, $"Не вдалося скинути пароль: {errors}");
        }

        /// <summary>
        /// Надсилає код підтвердження на електронну пошту користувача.
        /// </summary>
        /// <param name="email">Електронна адреса отримувача.</param>
        /// <param name="code">6-значний код підтвердження.</param>
        /// <returns><see langword="true"/> якщо лист успішно відправлено; інакше <see langword="false"/>.</returns>
        protected virtual async Task<bool> SendVerificationCodeAsync(string email, string code)
        {
            return await this.SendEmailAsync(
                email,
                "Verification code",
                $"Ваш код підтвердження: <strong>{code}</strong>");
        }

        /// <summary>
        /// Надсилає електронний лист через SMTP сервер Gmail.
        /// </summary>
        /// <param name="toEmail">Електронна адреса отримувача.</param>
        /// <param name="subject">Тема листа.</param>
        /// <param name="bodyHtml">HTML-вміст листа.</param>
        /// <returns><see langword="true"/> якщо лист успішно відправлено; інакше <see langword="false"/>.</returns>
        protected virtual async Task<bool> SendEmailAsync(string toEmail, string subject, string bodyHtml)
        {
            var emailUser = Environment.GetEnvironmentVariable("EMAIL_USER");
            var emailPass = Environment.GetEnvironmentVariable("EMAIL_PASS");

            if (string.IsNullOrWhiteSpace(emailUser) || string.IsNullOrWhiteSpace(emailPass))
            {
                this.logger.LogError("SMTP credentials (EMAIL_USER/EMAIL_PASS) не встановлено.");
                return false;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("GraphAn", emailUser));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = bodyHtml };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync(emailUser, emailPass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Помилка відправки email на {ToEmail}", toEmail);
                return false;
            }
        }

        private string GetPasswordHash(string password)
        {
            return new PasswordHasher<User>().HashPassword(SystemUser, password);
        }

        private bool CheckIfPasswordCorrect(User user, string password)
        {
            var hasher = new PasswordHasher<User>();
            var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash!, password);
            return verificationResult != PasswordVerificationResult.Failed;
        }

        private bool IsEmailInvalid(string? email)
        {
            return email == null ||
                   email.Length < 1 ||
                   email.Length > 100 ||
                   !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private bool IsEmailValid(string email) => !this.IsEmailInvalid(email);

        private string GenerateCode()
        {
            return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        }

        private bool IsPasswordInvalid(string? password)
        {
            return password == null ||
                   password.Length < 8 ||
                   password.Length > 255;
        }

        private bool IsCodeInvalid(string? code)
        {
            return code == null ||
                   code.Length != 6 ||
                   !code.All(char.IsDigit);
        }
    }
}