// <copyright file="TraversalStep.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Models
{
    /// <summary>
    /// Крок обходу графа.
    /// </summary>
    public class TraversalStep
    {
        /// <summary>
        /// Отримує або задає мітку поточної вершини.
        /// </summary>
        public string Vertex { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає номер кроку обходу.
        /// </summary>
        public int? StepNumber { get; set; }

        /// <summary>
        /// Отримує або задає поточний стан черги або стека.
        /// </summary>
        public List<string> Collection { get; set; } = new List<string>();

        /// <summary>
        /// Отримує або задає ребро дерева обходу.
        /// </summary>
        public string TreeEdge { get; set; } = "—";

        /// <summary>
        /// Отримує або задає ідентифікатор ребра.
        /// </summary>
        public string? EdgeId { get; set; }
    }
}
