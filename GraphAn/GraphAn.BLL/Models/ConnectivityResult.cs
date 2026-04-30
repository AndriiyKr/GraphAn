// <copyright file="ConnectivityResult.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Models
{
    /// <summary>
    /// Числа зв'язності графа.
    /// </summary>
    public class ConnectivityResult
    {
        /// <summary>
        /// Отримує або задає кількість компонент зв'язності.
        /// </summary>
        public int ComponentsCount { get; set; }

        /// <summary>
        /// Отримує або задає число вершинної зв'язності.
        /// </summary>
        public int VertexConnectivity { get; set; }

        /// <summary>
        /// Отримує або задає число реберної зв'язності.
        /// </summary>
        public int EdgeConnectivity { get; set; }
    }
}