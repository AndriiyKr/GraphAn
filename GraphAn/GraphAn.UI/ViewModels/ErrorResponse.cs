// <copyright file="ErrorResponse.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    /// <summary>
    /// Відповідь з описом помилки.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Отримує або задає опис помилки.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
