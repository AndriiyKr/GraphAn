// <copyright file="GraphMetricsResult.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Models
{
    /// <summary>
    /// Всі метрики графа.
    /// </summary>
    public class GraphMetricsResult
    {
        /// <summary>
        /// Отримує або задає базову інформацію про граф.
        /// </summary>
        public GraphInfoResult Info { get; set; } = new GraphInfoResult();

        /// <summary>
        /// Отримує або задає матрицю суміжності.
        /// </summary>
        public List<List<int>> AdjacencyMatrix { get; set; } = new List<List<int>>();

        /// <summary>
        /// Отримує або задає матрицю інцидентності.
        /// </summary>
        public List<List<int>> IncidenceMatrix { get; set; } = new List<List<int>>();

        /// <summary>
        /// Отримує або задає список суміжності та степені вершин.
        /// </summary>
        public AdjacencyListResult AdjacencyList { get; set; } = new AdjacencyListResult();

        /// <summary>
        /// Отримує або задає інформацію про цикл та хроматичне число.
        /// </summary>
        public CycleAndChromaticResult CycleAndChromatic { get; set; } = new CycleAndChromaticResult();

        /// <summary>
        /// Отримує або задає числа зв'язності графа.
        /// </summary>
        public ConnectivityResult Connectivity { get; set; } = new ConnectivityResult();

        /// <summary>
        /// Отримує або задає число незалежності та клікове число.
        /// </summary>
        public IndependenceAndCliqueResult IndependenceAndClique { get; set; } = new IndependenceAndCliqueResult();
    }
}