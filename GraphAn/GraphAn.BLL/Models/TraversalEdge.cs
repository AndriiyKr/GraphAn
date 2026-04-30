// <copyright file="TraversalEdge.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Models
{
    /// <summary>
    /// Ребро дерева обходу графа.
    /// </summary>
    public class TraversalEdge
    {
        /// <summary>
        /// Отримує або задає ідентифікатор початкової вершини.
        /// </summary>
        public string From { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає ідентифікатор кінцевої вершини.
        /// </summary>
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає ідентифікатор ребра.
        /// </summary>
        public string? EdgeId { get; set; }
    }
}