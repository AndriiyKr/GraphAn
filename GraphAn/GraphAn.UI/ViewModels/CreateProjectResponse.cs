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
        /// Gets or sets ідентифікатор створеного проекту.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Gets or sets повідомлення про результат операції.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}