// <copyright file="CodeConfirmRequest.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    /// <summary>
    /// Запит для підтвердження реєстрації користувача.
    /// </summary>
    public class CodeConfirmRequest
    {
        /// <summary>
        /// Gets or sets електронна пошта користувача.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets код підтвердження.
        /// </summary>
        public string Code { get; set; } = string.Empty;
    }
}
