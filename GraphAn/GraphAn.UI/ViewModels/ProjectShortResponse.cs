// <copyright file="ProjectShortResponse.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    /// <summary>
    /// Коротка інформація про проект без даних графа.
    /// </summary>
    public class ProjectShortResponse
    {
        /// <summary>
        /// Gets or sets ідентифікатор проекту.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Gets or sets назву проекту.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets час створення проекту.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets час останнього оновлення проекту.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}