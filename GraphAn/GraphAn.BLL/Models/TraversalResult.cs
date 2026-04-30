// <copyright file="TraversalResult.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Models
{
    /// <summary>
    /// Результат виконання алгоритму обходу графа.
    /// </summary>
    public class TraversalResult
    {
        /// <summary>
        /// Отримує або задає протокол обходу. (таблиця кроків з вершиною, номером кроку, стеком/чергою, ребром).
        /// </summary>
        public List<TraversalStep> Protocol { get; set; } = new List<TraversalStep>();

        /// <summary>
        /// Отримує або задає список ребер дерева обходу. (ребра дерева обходу).
        /// </summary>
        public List<TraversalEdge> TreeEdges { get; set; } = new List<TraversalEdge>();
    }
}