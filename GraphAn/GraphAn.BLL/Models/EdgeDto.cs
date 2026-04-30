// <copyright file="EdgeDto.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Models
{
    /// <summary>
    /// Модель ребра графа.
    /// </summary>
    public class EdgeDto
    {
        /// <summary>
        /// Gets or sets ідентифікатор ребра.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets ідентифікатор початкової вершини.
        /// </summary>
        public string From { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets ідентифікатор кінцевої вершини.
        /// </summary>
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets вагу ребра.
        /// </summary>
        public double Weight { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether чи має ребро вагу.
        /// </summary>
        public bool HasWeight { get; set; }
    }
}