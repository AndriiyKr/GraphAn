// <copyright file="LoginResponse.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    /// <summary>
    /// Відповідь після успішного входу у акаунт.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Отримує або задає JWT токен автентифікації.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає унікальний ідентифікатор користувача.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Отримує або задає ім'я користувача.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає електронну пошту користувача.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}
