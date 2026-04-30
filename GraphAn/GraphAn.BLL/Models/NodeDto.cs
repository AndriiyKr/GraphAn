// <copyright file="NodeDto.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Models
{
    /// <summary>
    /// Модель вершини графа.
    /// </summary>
    public class NodeDto
    {
        /// <summary>
        /// Отримує або задає ідентифікатор вершини.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає мітку вершини.
        /// </summary>
        public string Label { get; set; } = string.Empty;
    }
}