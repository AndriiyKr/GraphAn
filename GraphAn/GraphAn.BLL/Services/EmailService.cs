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
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки.
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

        /// <summary>
        /// Підтвердити реєстрацію з кодом від користувача.
        /// </summary>
        /// <param name="email">Електронна пошта користувача.</param>
        /// <param name="code">Код.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки.
        /// </returns>
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

            User user = new User
            {
                Id = Guid.NewGuid(),
                Email = email!,
                Username = registration.TempUsername ?? "Anonymous user",
                PasswordHash = registration.TempPasswordHash,
            };

            await this.userRepository.AddAsync(user);
            await this.registrationRepository.DeleteAsync(registration);

            this.logger.LogInformation("Користувача: {Email} було успішно зареєстровано", email);
            return (true, "Успішно зареєстровано");
        }

        /// <summary>
        /// Перевірка і знаходження користувача для входу у акаунт.</summary>
        /// <param name="password">Пароль.</param>
        /// <param name="emailOrUsername">Електронна пошта або нікнейм користувача.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки.
        /// <c>User</c> — об'єкт користувача при успіху.
        /// </returns>
        public async Task<(bool Success, string Message, User? User)> UserLoginAsync(string password, string? emailOrUsername)
        {
            if (this.IsPasswordInvalid(password))
            {
                this.logger.LogWarning("Передано некоректний пароль при вході у акаунт");
                return (false, "Некоректний пароль", null);
            }

            if (emailOrUsername == null)
            {
                this.logger.LogWarning("Передано некоректний логін при вході у акаунт");
                return (false, "Некоректний логін", null);
            }

            User? user = await this.userRepository.GetByEmailAsync(emailOrUsername);

            if (user == null)
            {
                user = await this.userRepository.GetByUsernameAsync(emailOrUsername);
            }

            if (user == null)
            {
                this.logger.LogWarning("Користувача з таким логіном не існує: {EmailOrUsername}", emailOrUsername);
                return (false, "Користувача з таким логіном не існує", null);
            }

            if (!this.CheckIfPasswordCorrect(user, password!))
            {
                this.logger.LogWarning("Передано невірний пароль при вході у акаунт: {EmailOrUsername}", emailOrUsername);
                return (false, "Невірний пароль", null);
            }

            this.logger.LogInformation("Користувача успішно перевірено при входжені у акаунт: {EmailOrUsername}", emailOrUsername);
            return (true, "Успішно знайдено", user);
        }

        /// <summary>
        /// Відправляє на електронну пошту користувача код підтвердження для верифікації.
        /// </summary>
        /// <param name="email">Електронна адреса користувача, на яку буде надіслано код.</param>
        /// <param name="code">Код підтвердження, який потрібно надіслати користувачу.</param>
        /// <returns>Асинхронна операція відправлення email-повідомлення.</returns>
        private async Task<bool> SendVerificationCodeAsync(string email, string code)
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

        private string GetPasswordHash(string password)
        {
            return new PasswordHasher<User>().HashPassword(new User(), password);
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

        private bool IsCodeInvalid(string? code)
        {
            if (code == null)
            {
                return true;
            }

            if (code.Length != 6)
            {
                return true;
            }

            if (!code.All(char.IsDigit))
            {
                return true;
            }

            return false;
        }
    }
}
