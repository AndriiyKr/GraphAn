// <copyright file="GraphDto.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Модель графа для десеріалізації з JSON.
    /// </summary>
    public class GraphDto
    {
        /// <summary>
        /// Отримує або задає список вершин графа.
        /// </summary>
        public List<NodeDto> Nodes { get; set; } = new List<NodeDto>();

        /// <summary>
        /// Отримує або задає список ребер графа.
        /// </summary>
        public List<EdgeDto> Edges { get; set; } = new List<EdgeDto>();

        /// <summary>
        /// Отримує або задає чи є граф орієнтованим.
        /// </summary>
        public bool Directed { get; set; }
    }
}