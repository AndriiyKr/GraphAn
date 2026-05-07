// <copyright file="AlgorithmServiceTests.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.Tests.BLL.Services
{
    using System.Collections.Generic;
    using GraphAn.BLL.Models;
    using GraphAn.BLL.Services;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;
    using FluentAssertions;

    /// <summary>
    /// Тести для сервісу алгоритмів на графах.
    /// </summary>
    public class AlgorithmServiceTests
    {
        private readonly Mock<ILogger<AlgorithmService>> _loggerMock;
        private readonly AlgorithmService _service;

        public AlgorithmServiceTests()
        {
            _loggerMock = new Mock<ILogger<AlgorithmService>>();
            _service = new AlgorithmService(_loggerMock.Object);
        }

        #region Допоміжні методи

        /// <summary>
        /// Створює простий неорієнтований граф з трьома вершинами та вагами.
        /// </summary>
        private static GraphDto CreateSimpleUndirectedGraph()
        {
            return new GraphDto
            {
                Nodes = new List<NodeDto>
                {
                    new() { Id = "1", Label = "A" },
                    new() { Id = "2", Label = "B" },
                    new() { Id = "3", Label = "C" },
                },
                Edges = new List<EdgeDto>
                {
                    new() { Id = "e12", From = "1", To = "2", Weight = 1, HasWeight = true },
                    new() { Id = "e23", From = "2", To = "3", Weight = 2, HasWeight = true },
                    new() { Id = "e13", From = "1", To = "3", Weight = 4, HasWeight = true },
                },
                Directed = false,
            };
        }

        /// <summary>
        /// Створює орієнтований граф.
        /// </summary>
        private static GraphDto CreateDirectedGraph()
        {
            return new GraphDto
            {
                Nodes = new List<NodeDto>
                {
                    new() { Id = "1", Label = "A" },
                    new() { Id = "2", Label = "B" },
                    new() { Id = "3", Label = "C" },
                },
                Edges = new List<EdgeDto>
                {
                    new() { Id = "e12", From = "1", To = "2", Weight = 1, HasWeight = true },
                    new() { Id = "e23", From = "2", To = "3", Weight = 2, HasWeight = true },
                },
                Directed = true,
            };
        }

        /// <summary>
        /// Створює граф з від'ємною вагою.
        /// </summary>
        private static GraphDto CreateGraphWithNegativeWeight()
        {
            return new GraphDto
            {
                Nodes = new List<NodeDto>
                {
                    new() { Id = "1", Label = "A" },
                    new() { Id = "2", Label = "B" },
                },
                Edges = new List<EdgeDto>
                {
                    new() { Id = "e12", From = "1", To = "2", Weight = -5, HasWeight = true },
                },
                Directed = false,
            };
        }

        /// <summary>
        /// Створює граф без ваг на ребрах.
        /// </summary>
        private static GraphDto CreateGraphWithoutWeights()
        {
            return new GraphDto
            {
                Nodes = new List<NodeDto>
                {
                    new() { Id = "1", Label = "A" },
                    new() { Id = "2", Label = "B" },
                },
                Edges = new List<EdgeDto>
                {
                    new() { Id = "e12", From = "1", To = "2", Weight = 0, HasWeight = false },
                },
                Directed = false,
            };
        }

        /// <summary>
        /// Створює незв'язний граф (дві окремі компоненти).
        /// </summary>
        private static GraphDto CreateDisconnectedGraph()
        {
            return new GraphDto
            {
                Nodes = new List<NodeDto>
                {
                    new() { Id = "1", Label = "A" },
                    new() { Id = "2", Label = "B" },
                    new() { Id = "3", Label = "C" },
                },
                Edges = new List<EdgeDto>
                {
                    new() { Id = "e12", From = "1", To = "2", Weight = 1, HasWeight = true },
                },
                Directed = false,
            };
        }

        #endregion

        #region RunDijkstraAsync

        /// <summary>
        /// Тест: алгоритм Дейкстри повертає найкоротший шлях у простому графі.
        /// Очікується шлях A->B->C з вагою 3.
        /// </summary>
        [Fact]
        public async Task RunDijkstraAsync_ValidGraph_ReturnsShortestPath()
        {
            // Arrange
            var graph = CreateSimpleUndirectedGraph();

            // Act
            var result = await _service.RunDijkstraAsync(graph, "1", "3");

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Шлях знайдено");
            result.Result.Should().NotBeNull();
            result.Result!.PathNodes.Should().Equal("A", "B", "C");
            result.Result.PathEdges.Should().Equal("e12", "e23");
            result.Result.TotalWeight.Should().Be(3);
        }

        /// <summary>
        /// Тест: шлях не існує між вершинами.
        /// Очікується помилка.
        /// </summary>
        [Fact]
        public async Task RunDijkstraAsync_NoPath_ReturnsFailure()
        {
            // Arrange
            var graph = CreateDirectedGraph(); // немає шляху з 1 до 3? Є 1->2->3, але в тесті візьмемо окремі вершини.
            // Створимо граф без шляху
            var disconnected = new GraphDto
            {
                Nodes = new List<NodeDto>
                {
                    new() { Id = "1", Label = "A" },
                    new() { Id = "2", Label = "B" },
                },
                Edges = new List<EdgeDto>(),
                Directed = false,
            };
            // Act
            var result = await _service.RunDijkstraAsync(disconnected, "1", "2");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Шлях між вершинами не існує");
            result.Result.Should().BeNull();
        }

        /// <summary>
        /// Тест: початкова або кінцева вершина не існує.
        /// </summary>
        [Fact]
        public async Task RunDijkstraAsync_InvalidVertex_ReturnsFailure()
        {
            // Arrange
            var graph = CreateSimpleUndirectedGraph();

            // Act
            var result = await _service.RunDijkstraAsync(graph, "99", "1");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Вершину не знайдено у графі");
            result.Result.Should().BeNull();
        }

        /// <summary>
        /// Тест: наявність від'ємної ваги.
        /// </summary>
        [Fact]
        public async Task RunDijkstraAsync_NegativeWeight_ReturnsFailure()
        {
            // Arrange
            var graph = CreateGraphWithNegativeWeight();

            // Act
            var result = await _service.RunDijkstraAsync(graph, "1", "2");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Алгоритм Дейкстри не працює з від'ємними вагами");
            result.Result.Should().BeNull();
        }

        /// <summary>
        /// Тест: ребра без ваги.
        /// </summary>
        [Fact]
        public async Task RunDijkstraAsync_MissingWeights_ReturnsFailure()
        {
            // Arrange
            var graph = CreateGraphWithoutWeights();

            // Act
            var result = await _service.RunDijkstraAsync(graph, "1", "2");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Не всі ребра мають вагу");
            result.Result.Should().BeNull();
        }

        /// <summary>
        /// Тест: startId == endId.
        /// </summary>
        [Fact]
        public async Task RunDijkstraAsync_StartEqualsEnd_ReturnsZeroWeightPath()
        {
            // Arrange
            var graph = CreateSimpleUndirectedGraph();

            // Act
            var result = await _service.RunDijkstraAsync(graph, "1", "1");

            // Assert
            result.Success.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result!.PathNodes.Should().ContainSingle("A");
            result.Result.PathEdges.Should().BeEmpty();
            result.Result.TotalWeight.Should().Be(0);
        }

        #endregion

        #region RunFloydWarshallAsync

        /// <summary>
        /// Тест: алгоритм Флойда-Воршела для неорієнтованого графа.
        /// Очікується успішне виконання, кількість кроків = кількість вершин + 1.
        /// </summary>
        [Fact]
        public async Task RunFloydWarshallAsync_UndirectedGraph_ReturnsSteps()
        {
            // Arrange
            var graph = CreateSimpleUndirectedGraph();

            // Act
            var result = await _service.RunFloydWarshallAsync(graph);

            // Assert
            result.Success.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result!.Steps.Should().HaveCount(graph.Nodes.Count + 1);
            result.Result.Labels.Should().Equal("A", "B", "C");
        }

        /// <summary>
        /// Тест: алгоритм для порожнього графа (без вершин).
        /// </summary>
        [Fact]
        public async Task RunFloydWarshallAsync_EmptyGraph_ReturnsSuccessWithEmptySteps()
        {
            // Arrange
            var graph = new GraphDto { Nodes = new List<NodeDto>(), Edges = new List<EdgeDto>(), Directed = false };

            // Act
            var result = await _service.RunFloydWarshallAsync(graph);

            // Assert
            result.Success.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result!.Steps.Should().ContainSingle(); // один крок (ініціалізація)
            result.Result.Labels.Should().BeEmpty();
        }

        /// <summary>
        /// Тест: ребра без ваги – помилка.
        /// </summary>
        [Fact]
        public async Task RunFloydWarshallAsync_WithoutWeights_ReturnsFailure()
        {
            // Arrange
            var graph = CreateGraphWithoutWeights();

            // Act
            var result = await _service.RunFloydWarshallAsync(graph);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Не всі ребра мають вагу");
            result.Result.Should().BeNull();
        }

        #endregion

        #region RunDfsAsync

        /// <summary>
        /// Тест: DFS на зв'язному неорієнтованому графі.
        /// Очікується протокол обходу та дерево ребер.
        /// </summary>
        [Fact]
        public async Task RunDfsAsync_ConnectedGraph_ReturnsTraversalResult()
        {
            // Arrange
            var graph = CreateSimpleUndirectedGraph();

            // Act
            var result = await _service.RunDfsAsync(graph, "1");

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Алгоритм виконано");
            result.Result.Should().NotBeNull();
            result.Result!.Protocol.Should().NotBeEmpty();
            result.Result.TreeEdges.Should().HaveCount(graph.Nodes.Count - 1);
        }

        /// <summary>
        /// Тест: DFS на порожньому графі.
        /// </summary>
        [Fact]
        public async Task RunDfsAsync_EmptyGraph_ReturnsFailure()
        {
            // Arrange
            var graph = new GraphDto { Nodes = new List<NodeDto>(), Edges = new List<EdgeDto>(), Directed = false };

            // Act
            var result = await _service.RunDfsAsync(graph, "1");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Граф порожній");
            result.Result.Should().BeNull();
        }

        /// <summary>
        /// Тест: початкова вершина не існує.
        /// </summary>
        [Fact]
        public async Task RunDfsAsync_StartVertexNotFound_ReturnsFailure()
        {
            // Arrange
            var graph = CreateSimpleUndirectedGraph();

            // Act
            var result = await _service.RunDfsAsync(graph, "99");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Початкову вершину не знайдено");
            result.Result.Should().BeNull();
        }

        /// <summary>
        /// Тест: незв'язний граф.
        /// </summary>
        [Fact]
        public async Task RunDfsAsync_DisconnectedGraph_ReturnsFailure()
        {
            // Arrange
            var graph = CreateDisconnectedGraph();

            // Act
            var result = await _service.RunDfsAsync(graph, "1");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Граф незв'язний");
            result.Result.Should().BeNull();
        }

        #endregion

        #region RunBfsAsync

        /// <summary>
        /// Тест: BFS на зв'язному неорієнтованому графі.
        /// </summary>
        [Fact]
        public async Task RunBfsAsync_ConnectedGraph_ReturnsTraversalResult()
        {
            // Arrange
            var graph = CreateSimpleUndirectedGraph();

            // Act
            var result = await _service.RunBfsAsync(graph, "1");

            // Assert
            result.Success.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result!.Protocol.Should().NotBeEmpty();
            result.Result.TreeEdges.Should().HaveCount(graph.Nodes.Count - 1);
        }

        /// <summary>
        /// Тест: BFS на порожньому графі.
        /// </summary>
        [Fact]
        public async Task RunBfsAsync_EmptyGraph_ReturnsFailure()
        {
            // Arrange
            var graph = new GraphDto { Nodes = new List<NodeDto>(), Edges = new List<EdgeDto>(), Directed = false };

            // Act
            var result = await _service.RunBfsAsync(graph, "1");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Граф порожній");
            result.Result.Should().BeNull();
        }

        /// <summary>
        /// Тест: початкова вершина не існує.
        /// </summary>
        [Fact]
        public async Task RunBfsAsync_StartVertexNotFound_ReturnsFailure()
        {
            // Arrange
            var graph = CreateSimpleUndirectedGraph();

            // Act
            var result = await _service.RunBfsAsync(graph, "99");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Початкову вершину не знайдено");
            result.Result.Should().BeNull();
        }

        /// <summary>
        /// Тест: незв'язний граф.
        /// </summary>
        [Fact]
        public async Task RunBfsAsync_DisconnectedGraph_ReturnsFailure()
        {
            // Arrange
            var graph = CreateDisconnectedGraph();

            // Act
            var result = await _service.RunBfsAsync(graph, "1");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Граф незв'язний");
            result.Result.Should().BeNull();
        }

        #endregion
    }
}