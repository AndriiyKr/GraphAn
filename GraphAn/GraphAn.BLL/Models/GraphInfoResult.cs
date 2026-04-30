// <copyright file="GraphInfoResult.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Models
{
    /// <summary>
    /// Базова інформація про граф.
    /// </summary>
    public class GraphInfoResult
    {
        /// <summary>
        /// Отримує або задає тип графа.
        /// </summary>
        public string GraphType { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає кількість вершин.
        /// </summary>
        public int NodeCount { get; set; }

        /// <summary>
        /// Отримує або задає кількість ребер.
        /// </summary>
        public int EdgeCount { get; set; }
    }
}