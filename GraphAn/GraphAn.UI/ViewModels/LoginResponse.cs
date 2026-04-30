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
        /// Gets or sets JWT токен автентифікації.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets унікальний ідентифікатор користувача.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets ім'я користувача.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets електронну пошту користувача.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}
