// <copyright file="AdjacencyListResult.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Models
{
    /// <summary>
    /// Список суміжності та степені вершин графа.
    /// </summary>
    public class AdjacencyListResult
    {
        /// <summary>
        /// Отримує або задає список суміжності.
        /// </summary>
        public List<AdjacencyEntry> AdjacencyList { get; set; } = new List<AdjacencyEntry>();

        /// <summary>
        /// Отримує або задає значення, що вказує чи є граф регулярним.
        /// </summary>
        public bool IsRegular { get; set; }
    }
}
