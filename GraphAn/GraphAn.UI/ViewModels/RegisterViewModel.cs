// <copyright file="RegisterViewModel.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    /// <summary>
    /// Модель представлення для сторінки реєстрації.
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Отримує або задає ім'я користувача (логін).
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Отримує або задає електронну пошту користувача.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає пароль.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}