// <copyright file="FloydWarshallResult.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Models
{
    /// <summary>
    /// Результат виконання алгоритму Флойда-Воршела.
    /// </summary>
    public class FloydWarshallResult
    {
        /// <summary>
        /// Отримує або задає кроки виконання алгоритму.
        /// </summary>
        public List<FloydStep> Steps { get; set; } = new List<FloydStep>();

        /// <summary>
        /// Отримує або задає мітки вершин.
        /// </summary>
        public List<string> Labels { get; set; } = new List<string>();
    }
}