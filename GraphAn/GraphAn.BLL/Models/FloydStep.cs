// <copyright file="FloydStep.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Models
{
    /// <summary>
    /// Крок виконання алгоритму Флойда-Воршела.
    /// </summary>
    public class FloydStep
    {
        /// <summary>
        /// Отримує або задає матрицю відстаней на поточному кроці.
        /// </summary>
        public List<List<string>> DistanceMatrix { get; set; } = new List<List<string>>();

        /// <summary>
        /// Отримує або задає матрицю попередників на поточному кроці.
        /// </summary>
        public List<List<int>> PredecessorMatrix { get; set; } = new List<List<int>>();
    }
}
