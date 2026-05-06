// <copyright file="UserLoginRequest.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    /// <summary>
    /// Запит для входу користувача в систему.
    /// </summary>
    public class UserLoginRequest
    {
        /// <summary>
        /// Отримує або задає логін (може бути email або ім'я користувача).
        /// </summary>
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає пароль користувача.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає значення, чи потрібно запам'ятати користувача.
        /// </summary>
        public bool RememberMe { get; set; }
    }
}