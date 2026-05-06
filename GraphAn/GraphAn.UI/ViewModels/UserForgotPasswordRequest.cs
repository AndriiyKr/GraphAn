// <copyright file="UserForgotPasswordRequest.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    /// <summary>
    /// Запит для відновлення пароля (надсилання листа з посиланням).
    /// </summary>
    public class UserForgotPasswordRequest
    {
        /// <summary>
        /// Отримує або задає електронну пошту користувача.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}