// <copyright file="TraversalRequest.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    /// <summary>
    /// Запит для алгоритмів обходу графа.
    /// </summary>
    public class TraversalRequest
    {
        /// <summary>
        /// Отримує або задає дані графа у форматі JSON.
        /// </summary>
        public string GraphData { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає ідентифікатор початкової вершини.
        /// </summary>
        public string StartId { get; set; } = string.Empty;
    }
}