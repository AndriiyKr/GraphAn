// <copyright file="UserResetPasswordRequest.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    /// <summary>
    /// Запит для скидання пароля після отримання токена.
    /// </summary>
    public class UserResetPasswordRequest
    {
        /// <summary>
        /// Отримує або задає електронну пошту користувача.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає токен скидання пароля, отриманий з листа.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає новий пароль.
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;
    }
}