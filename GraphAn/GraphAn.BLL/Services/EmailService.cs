// <copyright file="EmailService.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Services
{
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
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
        private readonly ILogger<EmailService> logger;
        private readonly UserRepository userRepository;
        private readonly RegistrationRepository registrationRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <param name="logger">Об'єкт логера.</param>
        /// <param name="userRepository">Об'єкт репозиторію користувача.</param>
        /// <param name="registrationRepository">Об'єкт репозиторію реєстрації.</param>
        public EmailService(
            ILogger<EmailService> logger,
            UserRepository userRepository,
            RegistrationRepository registrationRepository)
        {
            this.logger = logger;
            this.userRepository = userRepository;
            this.registrationRepository = registrationRepository;
        }

        /// <summary>
        /// Створити тимчасову реєсрацію з кодом для користувача.
        /// </summary>
        /// <param name="email">Електронна пошта користувача.</param>
        /// <param name="password">Пароль.</param>
        /// <param name="username">Назва користувача.</param>
        /// <returns>
        /// "1" — якщо користувача було зареєстровано;
        /// "опис помилки" — якщо ні.
        /// </returns>
        public async Task<(bool Success, string Message)> StartRegistrationAsync(string? email, string? password, string? username = null)
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

            if (await this.registrationRepository.IfEmailExistsAsync(email!))
            {
                this.logger.LogWarning("Користувач з таким email уже створив запит на реєстрацію: {Email}", email);
                return (false, "Користувач з таким email уже існує уже створив запит на реєстрацію");
            }

            User tempUser = new User
            {
                Id = Guid.NewGuid(),
                Email = email!,
                Username = username ?? "temp",
            };

            string verificationCode = this.GenerateCode();
            var registration = new Registration
            {
                Id = Guid.NewGuid(),
                Email = email!,
                VerificationCode = verificationCode,
                CodeExpiryTime = DateTime.UtcNow.AddMinutes(10),
                TempPasswordHash = this.GetPassworHash(tempUser, password!),
                TempUsername = username ?? "Anonymous user",
            };

            await this.registrationRepository.AddAsync(registration);

            bool sendResult = await this.SendVerificationCodeAsync(email!, verificationCode);
            if (!sendResult)
            {
                this.logger.LogWarning("Помилка надсилання коду користувачу: {Email}", email);

                return (false, "Помилка надсилання коду.");
            }

            this.logger.LogInformation("Тимчасовий запис реєстрації створено для користувача: {Email}", email);

            return (true, "Запис успішно створено. Код було відправлено на вашу пошту.");
        }

        /// <summary>
        /// Відправляє на електронну пошту користувача код підтвердження для верифікації.
        /// </summary>
        /// <param name="email">Електронна адреса користувача, на яку буде надіслано код.</param>
        /// <param name="code">Код підтвердження, який потрібно надіслати користувачу.</param>
        /// <returns>Асинхронна операція відправлення email-повідомлення.</returns>
        public async Task<bool> SendVerificationCodeAsync(string email, string code)
        {
            var emailUser = Environment.GetEnvironmentVariable("EMAIL_USER");
            var emailPass = Environment.GetEnvironmentVariable("EMAIL_PASS");

            if (string.IsNullOrWhiteSpace(emailUser) || string.IsNullOrWhiteSpace(emailPass))
            {
                this.logger.LogError("SMTP credentials are missing in environment variables.");
                return false;
            }

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("GraphAn", emailUser));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Verification code";

            message.Body = new TextPart("plain")
            {
                Text = $"Ваш код підтвердження: {code}",
            };

            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync(emailUser, emailPass);
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Помилка відправки email: {Email}", email);
                return false;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }

            return true;
        }

        private string GetPassworHash(User user, string password)
        {
            var hasher = new PasswordHasher<User>();
            return hasher.HashPassword(user, password);
        }

        private bool CheckIfPasswordCorrect(User user, string password)
        {
            var hasher = new PasswordHasher<User>();
            var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, password!);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return false;
            }

            return true;
        }

        private bool IsEmailInvalid(string? email)
        {
            if (email == null)
            {
                return true;
            }

            if (email.Length < 1)
            {
                return true;
            }

            if (email.Length > 100)
            {
                return true;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return true;
            }

            return false;
        }

        private string GenerateCode()
        {
            return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        }

        private bool IsPasswordInvalid(string? password)
        {
            if (password == null)
            {
                return true;
            }

            if (password.Length < 8)
            {
                return true;
            }

            if (password.Length > 255)
            {
                return true;
            }

            return false;
        }
    }
}
