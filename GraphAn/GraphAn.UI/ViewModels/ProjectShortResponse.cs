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
        /// Отримує або задає ідентифікатор проекту.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Отримує або задає назву проекту.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає час створення проекту.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Отримує або задає час останнього оновлення проекту.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}