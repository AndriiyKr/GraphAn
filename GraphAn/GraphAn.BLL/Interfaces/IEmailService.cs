// <copyright file="IEmailService.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Interfaces
{
    /// <summary>
    /// Контракт для сервісу роботи з електронною поштою.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Починає процес реєстрації користувача та відправляє код підтвердження на email.
        /// </summary>
        /// <param name="email">Електронна адреса користувача.</param>
        /// <param name="password">Пароль користувача.</param>
        /// <param name="username">Ім'я користувача (необов'язково).</param>
        /// <returns>Результат операції (успіх/помилка та повідомлення).</returns>
        Task<(bool Success, string Message)> StartRegistrationAsync(
            string? email,
            string? password,
            string? username = null);

        /// <summary>
        /// Відправляє код підтвердження на email користувача.
        /// </summary>
        /// <param name="email">Електронна адреса отримувача.</param>
        /// <param name="code">Код підтвердження.</param>
        /// <returns>True якщо email успішно відправлено, інакше false.</returns>
        Task<bool> SendVerificationCodeAsync(string email, string code);
    }
}
