// <copyright file="ErrorViewModel.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.ViewModels
{
    /// <summary>
    /// Модель для відображення помилок.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Отримує або задає ідентифікатор запиту.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Вказує, чи потрібно відображати ідентифікатор запиту.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
    }
}