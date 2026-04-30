// <copyright file="DijkstraResult.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Models
{
    /// <summary>
    /// Результат виконання алгоритму Дейкстри.
    /// </summary>
    public class DijkstraResult
    {
        /// <summary>
        /// Отримує або задає список вершин шляху.
        /// </summary>
        public List<string> PathNodes { get; set; } = new List<string>();

        /// <summary>
        /// Отримує або задає список ідентифікаторів ребер шляху.
        /// </summary>
        public List<string> PathEdges { get; set; } = new List<string>();

        /// <summary>
        /// Отримує або задає загальну вагу шляху.
        /// </summary>
        public double TotalWeight { get; set; }
    }
}