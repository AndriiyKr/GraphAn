// <copyright file="IAlgorithmService.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Interfaces
{
    using GraphAn.BLL.Models;

    /// <summary>
    /// Інтерфейс сервісу для виконання алгоритмів на графі.
    /// </summary>
    public interface IAlgorithmService
    {
        /// <summary>
        /// Виконує алгоритм Дейкстри для знаходження найкоротшого шляху.
        /// </summary>
        /// <param name="graph">Модель графа.</param>
        /// <param name="startId">Ідентифікатор початкової вершини.</param>
        /// <param name="endId">Ідентифікатор кінцевої вершини.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки,
        /// <c>Result</c> — результат алгоритму.
        /// </returns>
        Task<(bool Success, string Message, DijkstraResult? Result)> RunDijkstraAsync(GraphDto graph, string startId, string endId);

        /// <summary>
        /// Виконує алгоритм Флойда-Воршела для знаходження найкоротших шляхів між усіма парами вершин.
        /// </summary>
        /// <param name="graph">Модель графа.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки,
        /// <c>Result</c> — результат алгоритму.
        /// </returns>
        Task<(bool Success, string Message, FloydWarshallResult? Result)> RunFloydWarshallAsync(GraphDto graph);

        /// <summary>
        /// Виконує алгоритм пошуку вглиб (DFS).
        /// </summary>
        /// <param name="graph">Модель графа.</param>
        /// <param name="startId">Ідентифікатор початкової вершини.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки,
        /// <c>Result</c> — результат алгоритму.
        /// </returns>
        Task<(bool Success, string Message, TraversalResult? Result)> RunDfsAsync(GraphDto graph, string startId);

        /// <summary>
        /// Виконує алгоритм пошуку вшир (BFS).
        /// </summary>
        /// <param name="graph">Модель графа.</param>
        /// <param name="startId">Ідентифікатор початкової вершини.</param>
        /// <returns>
        /// Кортеж, де <c>Success</c> — результат операції,
        /// <c>Message</c> — опис результату або помилки,
        /// <c>Result</c> — результат алгоритму.
        /// </returns>
        Task<(bool Success, string Message, TraversalResult? Result)> RunBfsAsync(GraphDto graph, string startId);
    }
}