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
            string? email,
            string password,
            string? username);

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
        /// Надсилає на електронну пошту користувача посилання для скидання пароля.
        /// </summary>
        /// <param name="email">Електронна адреса користувача.</param>
        /// <param name="resetLinkGenerator">Функція для генерації посилання скидання пароля, приймає email та токен.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки.
        /// </returns>
        Task<(bool Success, string Message)> ForgotPasswordAsync(string email, Func<string, string, string> resetLinkGenerator);

        /// <summary>
        /// Скидає пароль користувача за допомогою токена.
        /// </summary>
        /// <param name="email">Електронна адреса користувача.</param>
        /// <param name="token">Токен скидання пароля.</param>
        /// <param name="newPassword">Новий пароль.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки.
        /// </returns>
        Task<(bool Success, string Message)> ResetPasswordAsync(string email, string token, string newPassword);
    }
}