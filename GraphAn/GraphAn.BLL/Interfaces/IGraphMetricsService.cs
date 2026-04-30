// <copyright file="IGraphMetricsService.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Interfaces
{
    using GraphAn.BLL.Models;

    /// <summary>
    /// Інтерфейс сервісу для обчислення метрик графа.
    /// </summary>
    public interface IGraphMetricsService
    {
        /// <summary>
        /// Отримує базову інформацію про граф.
        /// </summary>
        /// <param name="graph">Модель графа.</param>
        /// <returns>Базова інформація про граф.</returns>
        GraphInfoResult GetGraphInfo(GraphDto graph);

        /// <summary>
        /// Обчислює матрицю суміжності графа.
        /// </summary>
        /// <param name="graph">Модель графа.</param>
        /// <returns>Матриця суміжності у вигляді двовимірного списку.</returns>
        List<List<int>> GetAdjacencyMatrix(GraphDto graph);

        /// <summary>
        /// Обчислює матрицю інцидентності графа.
        /// </summary>
        /// <param name="graph">Модель графа.</param>
        /// <returns>Матриця інцидентності у вигляді двовимірного списку.</returns>
        List<List<int>> GetIncidenceMatrix(GraphDto graph);

        /// <summary>
        /// Отримує список суміжності та степені вершин графа.
        /// </summary>
        /// <param name="graph">Модель графа.</param>
        /// <returns>Список суміжності та степені вершин.</returns>
        AdjacencyListResult GetAdjacencyList(GraphDto graph);

        /// <summary>
        /// Знаходить найкоротший цикл та хроматичне число графа.
        /// </summary>
        /// <param name="graph">Модель графа.</param>
        /// <returns>Інформація про цикл та хроматичне число.</returns>
        CycleAndChromaticResult GetCycleAndChromatic(GraphDto graph);

        /// <summary>
        /// Обчислює числа реберної та вершинної зв'язності графа.
        /// </summary>
        /// <param name="graph">Модель графа.</param>
        /// <returns>Числа зв'язності графа.</returns>
        ConnectivityResult GetConnectivity(GraphDto graph);

        /// <summary>
        /// Обчислює число незалежності та клікове число графа.
        /// </summary>
        /// <param name="graph">Модель графа.</param>
        /// <returns>Число незалежності та клікове число.</returns>
        IndependenceAndCliqueResult GetIndependenceAndClique(GraphDto graph);
    }
}