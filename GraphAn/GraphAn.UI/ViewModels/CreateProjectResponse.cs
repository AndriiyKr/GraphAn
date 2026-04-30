// <copyright file="CreateProjectResponse.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    /// <summary>
    /// Відповідь після успішного створення проекту.
    /// </summary>
    public class CreateProjectResponse
    {
        /// <summary>
        /// Отримує або задає ідентифікатор створеного проекту.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Отримує або задає повідомлення про результат операції.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}