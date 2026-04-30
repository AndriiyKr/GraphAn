// <copyright file="CycleAndChromaticResult.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Models
{
    /// <summary>
    /// Інформація про найкоротший цикл та хроматичне число графа.
    /// </summary>
    public class CycleAndChromaticResult
    {
        /// <summary>
        /// Отримує або задає значення, що вказує чи має граф цикл.
        /// </summary>
        public bool HasCycle { get; set; }

        /// <summary>
        /// Отримує або задає обхват графа (довжина найкоротшого циклу).
        /// </summary>
        public int? Girth { get; set; }

        /// <summary>
        /// Отримує або задає шлях найкоротшого циклу.
        /// </summary>
        public List<string> CyclePath { get; set; } = new List<string>();

        /// <summary>
        /// Отримує або задає хроматичне число графа.
        /// </summary>
        public int ChromaticNumber { get; set; }

        /// <summary>
        /// Отримує або задає розфарбування вершин.
        /// </summary>
        public Dictionary<string, int> Coloring { get; set; } = new Dictionary<string, int>();
    }
}