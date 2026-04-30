// <copyright file="PathRequest.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    /// <summary>
    /// Запит для алгоритмів пошуку шляху.
    /// </summary>
    public class PathRequest
    {
        /// <summary>
        /// Отримує або задає дані графа у форматі JSON.
        /// </summary>
        public string GraphData { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає ідентифікатор початкової вершини.
        /// </summary>
        public string StartId { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає ідентифікатор кінцевої вершини.
        /// </summary>
        public string EndId { get; set; } = string.Empty;
    }
}