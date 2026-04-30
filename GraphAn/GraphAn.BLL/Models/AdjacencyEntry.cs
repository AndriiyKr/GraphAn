// <copyright file="AdjacencyEntry.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Models
{
    /// <summary>
    /// Запис списку суміжності для однієї вершини.
    /// </summary>
    public class AdjacencyEntry
    {
        /// <summary>
        /// Отримує або задає мітку вершини.
        /// </summary>
        public string Vertex { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає список сусідів.
        /// </summary>
        public string Neighbors { get; set; } = string.Empty;

        /// <summary>
        /// Отримує або задає степінь вершини.
        /// </summary>
        public int Degree { get; set; }

        /// <summary>
        /// Отримує або задає вхідний степінь вершини (для орієнтованих графів).
        /// </summary>
        public int? InDegree { get; set; }

        /// <summary>
        /// Отримує або задає вихідний степінь вершини (для орієнтованих графів).
        /// </summary>
        public int? OutDegree { get; set; }
    }
}