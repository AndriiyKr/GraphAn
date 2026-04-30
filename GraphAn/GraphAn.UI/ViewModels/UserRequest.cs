// <copyright file="UserRequest.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    /// <summary>
    /// Запит з даними користувача для реєстрації або входу.
    /// </summary>
    public class UserRequest
    {
        /// <summary>
        /// Gets or sets електронну пошту користувача.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets ім'я користувача.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets пароль користувача.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
