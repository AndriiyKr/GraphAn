// <copyright file="GraphMetricsServiceTests.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.Tests.BLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GraphAn.BLL.Models;
    using GraphAn.BLL.Services;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;
    using FluentAssertions;

    /// <summary>
    /// Тести для сервісу обчислення метрик графа.
    /// </summary>
    public class GraphMetricsServiceTests
    {
        private readonly Mock<ILogger<GraphMetricsService>> _loggerMock;
        private readonly GraphMetricsService _service;

        public GraphMetricsServiceTests()
        {
            _loggerMock = new Mock<ILogger<GraphMetricsService>>();
            _service = new GraphMetricsService(_loggerMock.Object);
        }

        #region Допоміжні методи створення тестових графів

        /// <summary>
        /// Створює порожній граф (без вершин і ребер).
        /// </summary>
        private static GraphDto CreateEmptyGraph() => new()
        {
            Nodes = new List<NodeDto>(),
            Edges = new List<EdgeDto>(),
            Directed = false,
        };

        /// <summary>
        /// Створює граф з однією вершиною.
        /// </summary>
        private static GraphDto CreateSingleNodeGraph()
        {
            return new GraphDto
            {
                Nodes = new List<NodeDto> { new() { Id = "1", Label = "A" } },
                Edges = new List<EdgeDto>(),
                Directed = false,
            };
        }

        /// <summary>
        /// Створює неорієнтований граф з трьома вершинами та ребрами: A-B, B-C, A-C.
        /// </summary>
        private static GraphDto CreateUndirectedTriangleGraph()
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
                    new() { Id = "e1", From = "1", To = "2", Weight = 1, HasWeight = true },
                    new() { Id = "e2", From = "2", To = "3", Weight = 2, HasWeight = true },
                    new() { Id = "e3", From = "3", To = "1", Weight = 3, HasWeight = true },
                },
                Directed = false,
            };
        }

        /// <summary>
        /// Створює орієнтований граф з трьома вершинами та дугами: A->B, B->C, C->A.
        /// </summary>
        private static GraphDto CreateDirectedTriangleGraph()
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
                    new() { Id = "e1", From = "1", To = "2", Weight = 1, HasWeight = true },
                    new() { Id = "e2", From = "2", To = "3", Weight = 2, HasWeight = true },
                    new() { Id = "e3", From = "3", To = "1", Weight = 3, HasWeight = true },
                },
                Directed = true,
            };
        }

        /// <summary>
        /// Створює граф із петлею (ребро з вершини на себе).
        /// </summary>
        private static GraphDto CreateGraphWithLoop()
        {
            return new GraphDto
            {
                Nodes = new List<NodeDto> { new() { Id = "1", Label = "A" } },
                Edges = new List<EdgeDto>
                {
                    new() { Id = "e1", From = "1", To = "1", Weight = 1, HasWeight = true },
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
                    new() { Id = "4", Label = "D" },
                },
                Edges = new List<EdgeDto>
                {
                    new() { Id = "e1", From = "1", To = "2", Weight = 1, HasWeight = true },
                    new() { Id = "e2", From = "3", To = "4", Weight = 1, HasWeight = true },
                },
                Directed = false,
            };
        }

        #endregion

        #region GetGraphInfo

        /// <summary>
        /// Тест: отримання інформації про граф для неорієнтованого графа.
        /// Очікується правильний тип, кількість вершин та ребер.
        /// </summary>
        [Fact]
        public void GetGraphInfo_UndirectedGraph_ReturnsCorrectInfo()
        {
            // Arrange
            var graph = CreateUndirectedTriangleGraph();

            // Act
            var result = _service.GetGraphInfo(graph);

            // Assert
            result.GraphType.Should().Be("Неорієнтований");
            result.NodeCount.Should().Be(3);
            result.EdgeCount.Should().Be(3);
        }

        /// <summary>
        /// Тест: отримання інформації про граф для орієнтованого графа.
        /// Очікується правильний тип.
        /// </summary>
        [Fact]
        public void GetGraphInfo_DirectedGraph_ReturnsCorrectType()
        {
            // Arrange
            var graph = CreateDirectedTriangleGraph();

            // Act
            var result = _service.GetGraphInfo(graph);

            // Assert
            result.GraphType.Should().Be("Орієнтований");
            result.NodeCount.Should().Be(3);
            result.EdgeCount.Should().Be(3);
        }

        /// <summary>
        /// Тест: отримання інформації про порожній граф.
        /// Очікується нульова кількість вершин та ребер.
        /// </summary>
        [Fact]
        public void GetGraphInfo_EmptyGraph_ReturnsZeroCounts()
        {
            // Arrange
            var graph = CreateEmptyGraph();

            // Act
            var result = _service.GetGraphInfo(graph);

            // Assert
            result.GraphType.Should().Be("Неорієнтований");
            result.NodeCount.Should().Be(0);
            result.EdgeCount.Should().Be(0);
        }

        #endregion

        #region GetAdjacencyMatrix

        /// <summary>
        /// Тест: обчислення матриці суміжності для неорієнтованого графа.
        /// Очікується симетрична матриця з одиницями на місцях ребер.
        /// </summary>
        [Fact]
        public void GetAdjacencyMatrix_UndirectedGraph_ReturnsSymmetricMatrix()
        {
            // Arrange
            var graph = CreateUndirectedTriangleGraph();

            // Act
            var matrix = _service.GetAdjacencyMatrix(graph);

            // Assert
            matrix.Should().HaveCount(3);
            foreach (var row in matrix)
            {
                row.Should().HaveCount(3);
            }

            // Перевірка на симетричність та наявність ребер
            matrix[0][1].Should().Be(1);
            matrix[1][0].Should().Be(1);
            matrix[1][2].Should().Be(1);
            matrix[2][1].Should().Be(1);
            matrix[2][0].Should().Be(1);
            matrix[0][2].Should().Be(1);
        }

        /// <summary>
        /// Тест: обчислення матриці суміжності для орієнтованого графа.
        /// Очікується несиметрична матриця.
        /// </summary>
        [Fact]
        public void GetAdjacencyMatrix_DirectedGraph_ReturnsNonSymmetricMatrix()
        {
            // Arrange
            var graph = CreateDirectedTriangleGraph();

            // Act
            var matrix = _service.GetAdjacencyMatrix(graph);

            // Assert
            matrix[0][1].Should().Be(1);
            matrix[1][0].Should().Be(0); // немає зворотного ребра
            matrix[1][2].Should().Be(1);
            matrix[2][1].Should().Be(0);
            matrix[2][0].Should().Be(1);
            matrix[0][2].Should().Be(0);
        }

        /// <summary>
        /// Тест: матриця суміжності для графа з петлею.
        /// Очікується, що петля збільшує значення на діагоналі.
        /// </summary>
        [Fact]
        public void GetAdjacencyMatrix_GraphWithLoop_IncrementsDiagonal()
        {
            // Arrange
            var graph = CreateGraphWithLoop();

            // Act
            var matrix = _service.GetAdjacencyMatrix(graph);

            // Assert
            matrix[0][0].Should().Be(1);
        }

        #endregion

        #region GetIncidenceMatrix

        /// <summary>
        /// Тест: обчислення матриці інцидентності для неорієнтованого графа.
        /// Очікується матриця, де кожне ребро позначається 1 в обох кінцях.
        /// </summary>
        [Fact]
        public void GetIncidenceMatrix_UndirectedGraph_ReturnsCorrectMatrix()
        {
            // Arrange
            var graph = CreateUndirectedTriangleGraph();

            // Act
            var matrix = _service.GetIncidenceMatrix(graph);

            // Assert
            // 3 вершини, 3 ребра
            matrix.Should().HaveCount(3);
            foreach (var row in matrix)
            {
                row.Should().HaveCount(3);
            }

            // Кожен стовпець повинен мати дві одиниці (для неорієнтованого)
            for (int col = 0; col < 3; col++)
            {
                var columnValues = matrix.Select(row => row[col]).ToList();
                columnValues.Count(v => v == 1).Should().Be(2);
            }
        }

        /// <summary>
        /// Тест: обчислення матриці інцидентності для орієнтованого графа.
        /// Очікується -1 для початку дуги, 1 для кінця.
        /// </summary>
        [Fact]
        public void GetIncidenceMatrix_DirectedGraph_ReturnsDirectedMatrix()
        {
            // Arrange
            var graph = CreateDirectedTriangleGraph();

            // Act
            var matrix = _service.GetIncidenceMatrix(graph);

            // Assert
            // Дуга A->B (перший стовпець): для A -1, для B 1
            matrix[0][0].Should().Be(-1);
            matrix[1][0].Should().Be(1);
            // Дуга B->C (другий стовпець)
            matrix[1][1].Should().Be(-1);
            matrix[2][1].Should().Be(1);
            // Дуга C->A (третій стовпець)
            matrix[2][2].Should().Be(-1);
            matrix[0][2].Should().Be(1);
        }

        /// <summary>
        /// Тест: порожній граф – повертається порожній список.
        /// </summary>
        [Fact]
        public void GetIncidenceMatrix_EmptyGraph_ReturnsEmptyList()
        {
            // Arrange
            var graph = CreateEmptyGraph();

            // Act
            var matrix = _service.GetIncidenceMatrix(graph);

            // Assert
            matrix.Should().BeEmpty();
        }

        #endregion

        #region GetAdjacencyList

        /// <summary>
        /// Тест: отримання списку суміжності для неорієнтованого графа.
        /// Очікується правильний перелік сусідів та степенів вершин.
        /// </summary>
        [Fact]
        public void GetAdjacencyList_UndirectedGraph_ReturnsCorrectAdjacency()
        {
            // Arrange
            var graph = CreateUndirectedTriangleGraph();

            // Act
            var result = _service.GetAdjacencyList(graph);

            // Assert
            result.AdjacencyList.Should().HaveCount(3);
            result.IsRegular.Should().BeTrue(); // всі вершини мають степінь 2

            var entryA = result.AdjacencyList.First(e => e.Vertex == "A");
            entryA.Neighbors.Should().Be("B, C");
            entryA.Degree.Should().Be(2);
            entryA.InDegree.Should().BeNull();
            entryA.OutDegree.Should().BeNull();
        }

        /// <summary>
        /// Тест: отримання списку суміжності для орієнтованого графа.
        /// Очікується окремий підрахунок вхідних/вихідних степенів.
        /// </summary>
        [Fact]
        public void GetAdjacencyList_DirectedGraph_ReturnsInOutDegrees()
        {
            // Arrange
            var graph = CreateDirectedTriangleGraph();

            // Act
            var result = _service.GetAdjacencyList(graph);

            // Assert
            var entryA = result.AdjacencyList.First(e => e.Vertex == "A");
            entryA.InDegree.Should().Be(1); // з C
            entryA.OutDegree.Should().Be(1); // в B
            entryA.Degree.Should().Be(2);
        }

        /// <summary>
        /// Тест: граф з однією вершиною без ребер – степінь 0, регулярний.
        /// </summary>
        [Fact]
        public void GetAdjacencyList_SingleNode_ReturnsZeroDegreeAndRegular()
        {
            // Arrange
            var graph = CreateSingleNodeGraph();

            // Act
            var result = _service.GetAdjacencyList(graph);

            // Assert
            result.AdjacencyList.Should().ContainSingle();
            result.AdjacencyList[0].Degree.Should().Be(0);
            result.IsRegular.Should().BeTrue();
        }

        #endregion

        #region GetCycleAndChromatic

        /// <summary>
        /// Тест: знаходження найкоротшого циклу та хроматичного числа для трикутника.
        /// Очікується цикл довжини 3, хроматичне число 3.
        /// </summary>
        [Fact]
        public void GetCycleAndChromatic_TriangleGraph_HasCycleAndChromatic3()
        {
            // Arrange
            var graph = CreateUndirectedTriangleGraph();

            // Act
            var result = _service.GetCycleAndChromatic(graph);

            // Assert
            result.HasCycle.Should().BeTrue();
            result.Girth.Should().Be(3);
            result.CyclePath.Should().HaveCount(4); // шлях містить початкову вершину двічі
            result.CyclePath.First().Should().Be(result.CyclePath.Last()); // замкнений цикл
            result.ChromaticNumber.Should().Be(3);
            result.Coloring.Should().HaveCount(3);
        }

        /// <summary>
        /// Тест: граф без циклів (дерево) – не має циклу.
        /// </summary>
        [Fact]
        public void GetCycleAndChromatic_TreeGraph_NoCycle()
        {
            // Arrange
            var graph = new GraphDto
            {
                Nodes = new List<NodeDto>
                {
                    new() { Id = "1", Label = "A" },
                    new() { Id = "2", Label = "B" },
                    new() { Id = "3", Label = "C" },
                },
                Edges = new List<EdgeDto>
                {
                    new() { Id = "e1", From = "1", To = "2", Weight = 1, HasWeight = true },
                    new() { Id = "e2", From = "2", To = "3", Weight = 1, HasWeight = true },
                },
                Directed = false,
            };

            // Act
            var result = _service.GetCycleAndChromatic(graph);

            // Assert
            result.HasCycle.Should().BeFalse();
            result.Girth.Should().BeNull();
            result.CyclePath.Should().BeEmpty();
            // хроматичне число для дерева = 2
            result.ChromaticNumber.Should().Be(2);
        }

        #endregion

        #region GetConnectivity

        /// <summary>
        /// Тест: зв'язний граф – кількість компонент 1, вершинна та реберна зв'язність >0.
        /// Поточна реалізація повертає реберну зв'язність = кількості ребер, оскільки перевіряється лише видалення одного ребра.
        /// </summary>
        [Fact]
        public void GetConnectivity_ConnectedGraph_ReturnsOneComponent()
        {
            // Arrange
            var graph = CreateUndirectedTriangleGraph();

            // Act
            var result = _service.GetConnectivity(graph);

            // Assert
            result.ComponentsCount.Should().Be(1);
            result.VertexConnectivity.Should().Be(2);
            result.EdgeConnectivity.Should().Be(2); // 2
        }

        /// <summary>
        /// Тест: незв'язний граф – дві компоненти, зв'язність 0.
        /// </summary>
        [Fact]
        public void GetConnectivity_DisconnectedGraph_ReturnsTwoComponents()
        {
            // Arrange
            var graph = CreateDisconnectedGraph();

            // Act
            var result = _service.GetConnectivity(graph);

            // Assert
            result.ComponentsCount.Should().Be(2);
            result.VertexConnectivity.Should().Be(0);
            result.EdgeConnectivity.Should().Be(0);
        }

        #endregion

        #region GetIndependenceAndClique

        /// <summary>
        /// Тест: число незалежності та клікове число для трикутника.
        /// Для K3: незалежність = 1, кліка = 3.
        /// </summary>
        [Fact]
        public void GetIndependenceAndClique_TriangleGraph_ReturnsCorrectNumbers()
        {
            // Arrange
            var graph = CreateUndirectedTriangleGraph();

            // Act
            var result = _service.GetIndependenceAndClique(graph);

            // Assert
            result.IndependenceNumber.Should().Be(1);
            result.CliqueNumber.Should().Be(3);
        }

        /// <summary>
        /// Тест: для порожнього графа (без ребер) незалежність = кількість вершин, кліка = 1.
        /// </summary>
        [Fact]
        public void GetIndependenceAndClique_EdgelessGraph_ReturnsCorrect()
        {
            // Arrange
            var graph = new GraphDto
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
            var result = _service.GetIndependenceAndClique(graph);

            // Assert
            result.IndependenceNumber.Should().Be(2);
            result.CliqueNumber.Should().Be(1);
        }

        #endregion

        #region GetAllMetrics

        /// <summary>
        /// Тест: метод GetAllMetrics повертає всі метрики для графа.
        /// Перевіряється, що результат не є null та містить очікувані дані.
        /// </summary>
        [Fact]
        public void GetAllMetrics_ValidGraph_ReturnsCompleteMetrics()
        {
            // Arrange
            var graph = CreateUndirectedTriangleGraph();

            // Act
            var result = _service.GetAllMetrics(graph);

            // Assert
            result.Should().NotBeNull();
            result.Info.Should().NotBeNull();
            result.AdjacencyMatrix.Should().HaveCount(3);
            result.IncidenceMatrix.Should().HaveCount(3);
            result.AdjacencyList.Should().NotBeNull();
            result.CycleAndChromatic.Should().NotBeNull();
            result.Connectivity.Should().NotBeNull();
            result.IndependenceAndClique.Should().NotBeNull();

            result.CycleAndChromatic.HasCycle.Should().BeTrue();
            result.Connectivity.ComponentsCount.Should().Be(1);
        }

        #endregion
    }
}