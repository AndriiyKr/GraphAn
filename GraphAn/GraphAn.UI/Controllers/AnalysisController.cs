// <copyright file="AnalysisController.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.Controllers
{
    using System.Security.Claims;
    using System.Text.Json;
    using GraphAn.BLL.Interfaces;
    using GraphAn.BLL.Models;
    using GraphAn.UI.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Контролер для аналізу графів.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly IGraphMetricsService graphMetricsService;
        private readonly IAlgorithmService algorithmService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisController"/> class.
        /// </summary>
        /// <param name="graphMetricsService">Сервіс для обчислення метрик графа.</param>
        /// <param name="algorithmService">Сервіс для виконання алгоритмів на графі.</param>
        public AnalysisController(
            IGraphMetricsService graphMetricsService,
            IAlgorithmService algorithmService)
        {
            this.graphMetricsService = graphMetricsService;
            this.algorithmService = algorithmService;
        }

        /// <summary>
        /// Отримує базову інформацію про граф.
        /// </summary>
        /// <param name="request">Дані графа.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з базовою інформацією про граф,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("info")]
        public IActionResult GetGraphInfo([FromBody] GraphRequest request)
        {
            var graph = this.ParseGraph(request.GraphData);
            if (graph == null)
            {
                return this.BadRequest(new ErrorResponse { Message = "Некоректний формат даних графа" });
            }

            var result = this.graphMetricsService.GetGraphInfo(graph);
            return this.Ok(result);
        }

        /// <summary>
        /// Обчислює матрицю суміжності графа.
        /// </summary>
        /// <param name="request">Дані графа.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з матрицею суміжності,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("adjacency-matrix")]
        public IActionResult GetAdjacencyMatrix([FromBody] GraphRequest request)
        {
            var graph = this.ParseGraph(request.GraphData);
            if (graph == null)
            {
                return this.BadRequest(new ErrorResponse { Message = "Некоректний формат даних графа" });
            }

            var result = this.graphMetricsService.GetAdjacencyMatrix(graph);
            return this.Ok(result);
        }

        /// <summary>
        /// Обчислює матрицю інцидентності графа.
        /// </summary>
        /// <param name="request">Дані графа.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з матрицею інцидентності,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("incidence-matrix")]
        public IActionResult GetIncidenceMatrix([FromBody] GraphRequest request)
        {
            var graph = this.ParseGraph(request.GraphData);
            if (graph == null)
            {
                return this.BadRequest(new ErrorResponse { Message = "Некоректний формат даних графа" });
            }

            var result = this.graphMetricsService.GetIncidenceMatrix(graph);
            return this.Ok(result);
        }

        /// <summary>
        /// Отримує список суміжності та степені вершин графа.
        /// </summary>
        /// <param name="request">Дані графа.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> зі списком суміжності,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("adjacency-list")]
        public IActionResult GetAdjacencyList([FromBody] GraphRequest request)
        {
            var graph = this.ParseGraph(request.GraphData);
            if (graph == null)
            {
                return this.BadRequest(new ErrorResponse { Message = "Некоректний формат даних графа" });
            }

            var result = this.graphMetricsService.GetAdjacencyList(graph);
            return this.Ok(result);
        }

        /// <summary>
        /// Знаходить найкоротший цикл та хроматичне число графа.
        /// </summary>
        /// <param name="request">Дані графа.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з інформацією про цикл та хроматичне число,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("cycle-chromatic")]
        public IActionResult GetCycleAndChromatic([FromBody] GraphRequest request)
        {
            var graph = this.ParseGraph(request.GraphData);
            if (graph == null)
            {
                return this.BadRequest(new ErrorResponse { Message = "Некоректний формат даних графа" });
            }

            var result = this.graphMetricsService.GetCycleAndChromatic(graph);
            return this.Ok(result);
        }

        /// <summary>
        /// Обчислює числа реберної та вершинної зв'язності графа.
        /// </summary>
        /// <param name="request">Дані графа.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з числами зв'язності,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("connectivity")]
        public IActionResult GetConnectivity([FromBody] GraphRequest request)
        {
            var graph = this.ParseGraph(request.GraphData);
            if (graph == null)
            {
                return this.BadRequest(new ErrorResponse { Message = "Некоректний формат даних графа" });
            }

            var result = this.graphMetricsService.GetConnectivity(graph);
            return this.Ok(result);
        }

        /// <summary>
        /// Обчислює число незалежності та клікове число графа.
        /// </summary>
        /// <param name="request">Дані графа.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з числом незалежності та кліковим числом,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("independence-clique")]
        public IActionResult GetIndependenceAndClique([FromBody] GraphRequest request)
        {
            var graph = this.ParseGraph(request.GraphData);
            if (graph == null)
            {
                return this.BadRequest(new ErrorResponse { Message = "Некоректний формат даних графа" });
            }

            var result = this.graphMetricsService.GetIndependenceAndClique(graph);
            return this.Ok(result);
        }

        /// <summary>
        /// Обчислює всі метрики графа.
        /// </summary>
        /// <param name="request">Дані графа.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з усіма метриками графа,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("metrics")]
        public IActionResult GetAllMetrics([FromBody] GraphRequest request)
        {
            var graph = this.ParseGraph(request.GraphData);
            if (graph == null)
            {
                return this.BadRequest(new ErrorResponse { Message = "Некоректний формат даних графа" });
            }

            var result = this.graphMetricsService.GetAllMetrics(graph);
            return this.Ok(result);
        }

        /// <summary>
        /// Виконує алгоритм Дейкстри.
        /// </summary>
        /// <param name="request">Дані графа та параметри алгоритму.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з результатом алгоритму,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("dijkstra")]
        public async Task<IActionResult> RunDijkstra([FromBody] PathRequest request)
        {
            var graph = this.ParseGraph(request.GraphData);
            if (graph == null)
            {
                return this.BadRequest(new ErrorResponse { Message = "Некоректний формат даних графа" });
            }

            var result = await this.algorithmService.RunDijkstraAsync(graph, request.StartId, request.EndId);

            if (!result.Success)
            {
                return this.BadRequest(new ErrorResponse { Message = result.Message });
            }

            return this.Ok(result.Result);
        }

        /// <summary>
        /// Виконує алгоритм Флойда-Воршела.
        /// </summary>
        /// <param name="request">Дані графа.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з результатом алгоритму,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("floyd-warshall")]
        public async Task<IActionResult> RunFloydWarshall([FromBody] GraphRequest request)
        {
            var graph = this.ParseGraph(request.GraphData);
            if (graph == null)
            {
                return this.BadRequest(new ErrorResponse { Message = "Некоректний формат даних графа" });
            }

            var result = await this.algorithmService.RunFloydWarshallAsync(graph);

            if (!result.Success)
            {
                return this.BadRequest(new ErrorResponse { Message = result.Message });
            }

            return this.Ok(result.Result);
        }

        /// <summary>
        /// Виконує алгоритм пошуку вглиб (DFS).
        /// </summary>
        /// <param name="request">Дані графа та параметри алгоритму.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з результатом алгоритму,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("dfs")]
        public async Task<IActionResult> RunDfs([FromBody] TraversalRequest request)
        {
            var graph = this.ParseGraph(request.GraphData);
            if (graph == null)
            {
                return this.BadRequest(new ErrorResponse { Message = "Некоректний формат даних графа" });
            }

            var result = await this.algorithmService.RunDfsAsync(graph, request.StartId);

            if (!result.Success)
            {
                return this.BadRequest(new ErrorResponse { Message = result.Message });
            }

            return this.Ok(result.Result);
        }

        /// <summary>
        /// Виконує алгоритм пошуку вшир (BFS).
        /// </summary>
        /// <param name="request">Дані графа та параметри алгоритму.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> з результатом алгоритму,
        /// або <see cref="BadRequestObjectResult"/> з описом помилки.
        /// </returns>
        [HttpPost("bfs")]
        public async Task<IActionResult> RunBfs([FromBody] TraversalRequest request)
        {
            var graph = this.ParseGraph(request.GraphData);
            if (graph == null)
            {
                return this.BadRequest(new ErrorResponse { Message = "Некоректний формат даних графа" });
            }

            var result = await this.algorithmService.RunBfsAsync(graph, request.StartId);

            if (!result.Success)
            {
                return this.BadRequest(new ErrorResponse { Message = result.Message });
            }

            return this.Ok(result.Result);
        }

        /// <summary>
        /// Десеріалізує JSON рядок у модель графа.
        /// </summary>
        /// <param name="graphData">JSON рядок з даними графа.</param>
        /// <returns><see cref="GraphDto"/> якщо успішно; інакше <see langword="null"/>.</returns>
        private GraphDto? ParseGraph(string graphData)
        {
            try
            {
                return JsonSerializer.Deserialize<GraphDto>(graphData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}