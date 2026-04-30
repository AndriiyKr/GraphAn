// <copyright file="SaveProjectRequest.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    /// <summary>
    /// Запит для збереження існуючого проекту.
    /// </summary>
    public class SaveProjectRequest
    {
        /// <summary>
        /// Gets or sets ідентифікатор проекту.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Gets or sets нову назву проекту.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets дані графа у форматі JSON.
        /// </summary>
        public string? GraphData { get; set; }
    }
}