// <copyright file="GraphRequest.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.UI.ViewModels
{
    /// <summary>
    /// Запит з даними графа.
    /// </summary>
    public class GraphRequest
    {
        /// <summary>
        /// Отримує або задає дані графа у форматі JSON.
        /// </summary>
        public string GraphData { get; set; } = string.Empty;
    }
}