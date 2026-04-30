// <copyright file="IndependenceAndCliqueResult.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Models
{
    /// <summary>
    /// Число незалежності та клікове число графа.
    /// </summary>
    public class IndependenceAndCliqueResult
    {
        /// <summary>
        /// Отримує або задає число незалежності графа.
        /// </summary>
        public int IndependenceNumber { get; set; }

        /// <summary>
        /// Отримує або задає клікове число графа.
        /// </summary>
        public int CliqueNumber { get; set; }
    }
}