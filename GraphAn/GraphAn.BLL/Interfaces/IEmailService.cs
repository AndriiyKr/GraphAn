// <copyright file="IEmailService.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Interfaces
{
    using GraphAn.DAL.Models;

    /// <summary>
    /// Контракт для сервісу роботи з електронною поштою.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Починає процес реєстрації користувача та відправляє код підтвердження на email.
        /// </summary>
        /// <param name="email">Електронна пошта користувача.</param>
        /// <param name="password">Пароль.</param>
        /// <param name="username">Назва користувача.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки.
        /// </returns>
        Task<(bool Success, string Message)> StartRegistrationAsync(
            string? email, string password, string? username);

        /// <summary>
        /// Підтвердити реєстрацію з кодом від користувача.
        /// </summary>
        /// <param name="email">Електронна пошта користувача.</param>
        /// <param name="code">Код.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки.
        /// </returns>
        Task<(bool Success, string Message)> ConfirmRegistrationAsync(string? email, string? code);

        /// <summary>
        /// Перевірка і знаходження користувача для входу у акаунт.</summary>
        /// <param name="email">Електронна адреса користувача.</param>
        /// <param name="password">Пароль.</param>
        /// <param name="username">Ім'я користувача (необов'язково).</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки.
        /// <c>User</c> — об'єкт користувача при успіху.
        /// </returns>
        Task<(bool Success, string Message, User? User)> UserLoginAsync(string? email, string password, string? username);
    }
}
