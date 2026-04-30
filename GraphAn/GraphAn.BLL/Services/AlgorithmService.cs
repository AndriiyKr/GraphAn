// <copyright file="AlgorithmService.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Services
{
    using GraphAn.BLL.Interfaces;
    using GraphAn.BLL.Models;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Сервіс для виконання алгоритмів на графі.
    /// </summary>
    public class AlgorithmService : IAlgorithmService
    {
        private readonly ILogger<AlgorithmService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgorithmService"/> class.
        /// </summary>
        /// <param name="logger">Об'єкт логера.</param>
        public AlgorithmService(ILogger<AlgorithmService> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public Task<(bool Success, string Message, DijkstraResult? Result)> RunDijkstraAsync(
            GraphDto graph, string startId, string endId)
        {
            var nodeIds = graph.Nodes.Select(n => n.Id).ToHashSet();
            var nodeLabels = graph.Nodes.ToDictionary(n => n.Id, n => n.Label);

            if (!nodeIds.Contains(startId) || !nodeIds.Contains(endId))
            {
                this.logger.LogWarning("Вершину не знайдено у графі");
                return Task.FromResult<(bool, string, DijkstraResult?)>(
                    (false, "Вершину не знайдено у графі", null));
            }

            foreach (var edge in graph.Edges)
            {
                if (edge.Weight < 0)
                {
                    this.logger.LogWarning("Алгоритм Дейкстри не працює з від'ємними вагами");
                    return Task.FromResult<(bool, string, DijkstraResult?)>(
                        (false, "Алгоритм Дейкстри не працює з від'ємними вагами", null));
                }

                if (!edge.HasWeight)
                {
                    this.logger.LogWarning("Не всі ребра мають вагу");
                    return Task.FromResult<(bool, string, DijkstraResult?)>(
                        (false, "Не всі ребра мають вагу", null));
                }
            }

            if (startId == endId)
            {
                return Task.FromResult<(bool, string, DijkstraResult?)>((true, "Шлях знайдено", new DijkstraResult
                {
                    PathNodes = new List<string> { nodeLabels[startId] },
                    PathEdges = new List<string>(),
                    TotalWeight = 0,
                }));
            }

            // ініціалізація
            var dist = nodeIds.ToDictionary(id => id, _ => double.PositiveInfinity);
            var prev = new Dictionary<string, string?>(nodeIds.ToDictionary(id => id, _ => (string?)null));
            var prevEdge = new Dictionary<string, string?>(nodeIds.ToDictionary(id => id, _ => (string?)null));
            var unvisited = new HashSet<string>(nodeIds);
            dist[startId] = 0;

            // будуємо список суміжності з вагами
            var adjList = nodeIds.ToDictionary(id => id, _ => new List<(string To, double Weight, string EdgeId)>());
            foreach (var edge in graph.Edges)
            {
                if (!nodeIds.Contains(edge.From) || !nodeIds.Contains(edge.To))
                {
                    continue;
                }

                adjList[edge.From].Add((edge.To, edge.Weight, edge.Id));
                if (!graph.Directed)
                {
                    adjList[edge.To].Add((edge.From, edge.Weight, edge.Id));
                }
            }

            while (unvisited.Count > 0)
            {
                var current = unvisited
                    .Where(id => !double.IsPositiveInfinity(dist[id]))
                    .OrderBy(id => dist[id])
                    .FirstOrDefault();

                if (current == null)
                {
                    break;
                }

                if (current == endId)
                {
                    break;
                }

                unvisited.Remove(current);

                foreach (var (to, weight, edgeId) in adjList[current])
                {
                    if (!unvisited.Contains(to))
                    {
                        continue;
                    }

                    double newDist = dist[current] + weight;
                    if (newDist < dist[to])
                    {
                        dist[to] = newDist;
                        prev[to] = current;
                        prevEdge[to] = edgeId;
                    }
                }
            }

            if (double.IsPositiveInfinity(dist[endId]))
            {
                this.logger.LogWarning("Шлях між вершинами не існує");
                return Task.FromResult<(bool, string, DijkstraResult?)>(
                    (false, "Шлях між вершинами не існує", null));
            }

            // відновлення шляху
            var pathNodes = new List<string>();
            var pathEdges = new List<string>();
            var node = endId;

            while (node != null)
            {
                pathNodes.Insert(0, nodeLabels[node]);
                if (prevEdge[node] != null)
                {
                    pathEdges.Insert(0, prevEdge[node] !);
                }

                node = prev[node];
            }

            this.logger.LogInformation("Алгоритм Дейкстри виконано успішно");
            return Task.FromResult<(bool, string, DijkstraResult?)>((true, "Шлях знайдено", new DijkstraResult
            {
                PathNodes = pathNodes,
                PathEdges = pathEdges,
                TotalWeight = dist[endId],
            }));
        }

        /// <inheritdoc/>
        public Task<(bool Success, string Message, FloydWarshallResult? Result)> RunFloydWarshallAsync(GraphDto graph)
        {
            foreach (var edge in graph.Edges)
            {
                if (!edge.HasWeight)
                {
                    this.logger.LogWarning("Не всі ребра мають вагу");
                    return Task.FromResult<(bool, string, FloydWarshallResult?)>(
                        (false, "Не всі ребра мають вагу", null));
                }
            }

            var nodes = graph.Nodes.OrderBy(n => n.Id).ToList();
            int n = nodes.Count;
            var nodeIndex = new Dictionary<string, int>();
            var labels = new List<string>();

            for (int i = 0; i < n; i++)
            {
                nodeIndex[nodes[i].Id] = i;
                labels.Add(nodes[i].Label);
            }

            // ініціалізація матриць
            var dist = new double[n, n];
            var pred = new int[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    dist[i, j] = i == j ? 0 : double.PositiveInfinity;
                    pred[i, j] = i == j ? 0 : i + 1;
                }
            }

            foreach (var edge in graph.Edges)
            {
                if (!nodeIndex.TryGetValue(edge.From, out int u) ||
                    !nodeIndex.TryGetValue(edge.To, out int v))
                {
                    continue;
                }

                if (edge.Weight < dist[u, v])
                {
                    dist[u, v] = edge.Weight;
                    pred[u, v] = u + 1;
                }

                if (!graph.Directed && edge.Weight < dist[v, u])
                {
                    dist[v, u] = edge.Weight;
                    pred[v, u] = v + 1;
                }
            }

            var steps = new List<FloydStep> { this.GetFloydSnapshot(dist, pred, n) };

            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (!double.IsPositiveInfinity(dist[i, k]) &&
                            !double.IsPositiveInfinity(dist[k, j]) &&
                            dist[i, k] + dist[k, j] < dist[i, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            pred[i, j] = pred[k, j];
                        }
                    }
                }

                steps.Add(this.GetFloydSnapshot(dist, pred, n));
            }

            this.logger.LogInformation("Алгоритм Флойда-Воршела виконано успішно");
            return Task.FromResult<(bool, string, FloydWarshallResult?)>((true, "Алгоритм виконано", new FloydWarshallResult
            {
                Steps = steps,
                Labels = labels,
            }));
        }

        /// <inheritdoc/>
        public Task<(bool Success, string Message, TraversalResult? Result)> RunDfsAsync(
            GraphDto graph, string startId)
        {
            var validationError = this.ValidateTraversal(graph, startId);
            if (validationError != null)
            {
                return Task.FromResult<(bool, string, TraversalResult?)>(
                    (false, validationError, null));
            }

            var nodeLabels = graph.Nodes.ToDictionary(n => n.Id, n => n.Label);
            var adjList = this.BuildAdjList(graph);

            var protocol = new List<TraversalStep>();
            var treeEdges = new List<TraversalEdge>();
            var visited = new HashSet<string>();
            var stack = new Stack<string>();
            int counter = 1;

            visited.Add(startId);
            stack.Push(startId);

            protocol.Add(new TraversalStep
            {
                Vertex = nodeLabels[startId],
                StepNumber = counter,
                Collection = stack.Select(n => nodeLabels[n]).ToList(),
                TreeEdge = "—",
                EdgeId = null,
            });

            while (stack.Count > 0)
            {
                var current = stack.Peek();
                var unvisitedNeighbor = adjList[current]
                    .OrderBy(t => nodeLabels[t.To])
                    .FirstOrDefault(t => !visited.Contains(t.To));

                if (unvisitedNeighbor != default)
                {
                    counter++;
                    var neighbor = unvisitedNeighbor.To;
                    visited.Add(neighbor);
                    stack.Push(neighbor);

                    var edgeLabel = graph.Directed
                        ? $"({nodeLabels[current]}, {nodeLabels[neighbor]})"
                        : $"{{{nodeLabels[current]}, {nodeLabels[neighbor]}}}";

                    treeEdges.Add(new TraversalEdge
                    {
                        From = current,
                        To = neighbor,
                        EdgeId = unvisitedNeighbor.EdgeId,
                    });

                    protocol.Add(new TraversalStep
                    {
                        Vertex = nodeLabels[neighbor],
                        StepNumber = counter,
                        Collection = stack.Select(n => nodeLabels[n]).ToList(),
                        TreeEdge = edgeLabel,
                        EdgeId = unvisitedNeighbor.EdgeId,
                    });
                }
                else
                {
                    stack.Pop();
                    protocol.Add(new TraversalStep
                    {
                        Vertex = "—",
                        StepNumber = null,
                        Collection = stack.Count > 0
                            ? stack.Select(n => nodeLabels[n]).ToList()
                            : new List<string> { "∅" },
                        TreeEdge = stack.Count > 0 ? "backtrack" : "—",
                        EdgeId = null,
                    });
                }
            }

            this.logger.LogInformation("Алгоритм DFS виконано успішно");
            return Task.FromResult<(bool, string, TraversalResult?)>((true, "Алгоритм виконано", new TraversalResult
            {
                Protocol = protocol,
                TreeEdges = treeEdges,
            }));
        }

        /// <inheritdoc/>
        public Task<(bool Success, string Message, TraversalResult? Result)> RunBfsAsync(
            GraphDto graph, string startId)
        {
            var validationError = this.ValidateTraversal(graph, startId);
            if (validationError != null)
            {
                return Task.FromResult<(bool, string, TraversalResult?)>(
                    (false, validationError, null));
            }

            var nodeLabels = graph.Nodes.ToDictionary(n => n.Id, n => n.Label);
            var adjList = this.BuildAdjList(graph);

            var protocol = new List<TraversalStep>();
            var treeEdges = new List<TraversalEdge>();
            var visited = new HashSet<string> { startId };
            var queue = new List<string> { startId };
            int headIdx = 0;
            int counter = 1;

            protocol.Add(new TraversalStep
            {
                Vertex = nodeLabels[startId],
                StepNumber = counter,
                Collection = new List<string> { nodeLabels[startId] },
                TreeEdge = "—",
                EdgeId = null,
            });

            while (headIdx < queue.Count)
            {
                var current = queue[headIdx];

                foreach (var (to, edgeId) in adjList[current]
                    .OrderBy(t => nodeLabels[t.To])
                    .Select(t => (t.To, t.EdgeId)))
                {
                    if (visited.Contains(to))
                    {
                        continue;
                    }

                    counter++;
                    visited.Add(to);
                    queue.Add(to);

                    var edgeLabel = graph.Directed
                        ? $"({nodeLabels[current]}, {nodeLabels[to]})"
                        : $"{{{nodeLabels[current]}, {nodeLabels[to]}}}";

                    treeEdges.Add(new TraversalEdge
                    {
                        From = current,
                        To = to,
                        EdgeId = edgeId,
                    });

                    protocol.Add(new TraversalStep
                    {
                        Vertex = nodeLabels[to],
                        StepNumber = counter,
                        Collection = queue.Skip(headIdx).Select(n => nodeLabels[n]).ToList(),
                        TreeEdge = edgeLabel,
                        EdgeId = edgeId,
                    });
                }

                headIdx++;
                var remaining = queue.Skip(headIdx).Select(n => nodeLabels[n]).ToList();
                protocol.Add(new TraversalStep
                {
                    Vertex = "—",
                    StepNumber = null,
                    Collection = remaining.Count > 0 ? remaining : new List<string> { "∅" },
                    TreeEdge = "—",
                    EdgeId = null,
                });
            }

            this.logger.LogInformation("Алгоритм BFS виконано успішно");
            return Task.FromResult<(bool, string, TraversalResult?)>((true, "Алгоритм виконано", new TraversalResult
            {
                Protocol = protocol,
                TreeEdges = treeEdges,
            }));
        }

        /// <summary>
        /// Перевіряє граф та початкову вершину перед обходом.
        /// </summary>
        /// <param name="graph">Модель графа.</param>
        /// <param name="startId">Ідентифікатор початкової вершини.</param>
        /// <returns>Повідомлення про помилку або <see langword="null"/> якщо все коректно.</returns>
        private string? ValidateTraversal(GraphDto graph, string startId)
        {
            if (!graph.Nodes.Any())
            {
                return "Граф порожній";
            }

            if (!graph.Nodes.Any(n => n.Id == startId))
            {
                return "Початкову вершину не знайдено";
            }

            // перевірка зв'язності через BFS
            var nodeIds = graph.Nodes.Select(n => n.Id).ToHashSet();
            var adjList = this.BuildAdjList(graph);
            var visited = new HashSet<string>();
            var queue = new Queue<string>();
            queue.Enqueue(startId);
            visited.Add(startId);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var (to, _) in adjList[current])
                {
                    if (!visited.Contains(to))
                    {
                        visited.Add(to);
                        queue.Enqueue(to);
                    }
                }
            }

            if (visited.Count != nodeIds.Count)
            {
                return $"Граф незв'язний. Для обходу граф має бути зв'язним.";
            }

            return null;
        }

        /// <summary>
        /// Будує список суміжності з ребрами.
        /// </summary>
        /// <param name="graph">Модель графа.</param>
        /// <returns>Список суміжності.</returns>
        private Dictionary<string, List<(string To, string? EdgeId)>> BuildAdjList(GraphDto graph)
        {
            var nodeIds = graph.Nodes.Select(n => n.Id).ToHashSet();
            var adjList = nodeIds.ToDictionary(
                id => id,
                _ => new List<(string To, string? EdgeId)>());

            foreach (var edge in graph.Edges)
            {
                if (!nodeIds.Contains(edge.From) || !nodeIds.Contains(edge.To))
                {
                    continue;
                }

                adjList[edge.From].Add((edge.To, edge.Id));
                if (!graph.Directed)
                {
                    adjList[edge.To].Add((edge.From, edge.Id));
                }
            }

            return adjList;
        }

        /// <summary>
        /// Створює знімок матриць на поточному кроці алгоритму Флойда-Воршела.
        /// </summary>
        /// <param name="dist">Матриця відстаней.</param>
        /// <param name="pred">Матриця попередників.</param>
        /// <param name="n">Розмір матриці.</param>
        /// <returns>Знімок поточного кроку.</returns>
        private FloydStep GetFloydSnapshot(double[,] dist, int[,] pred, int n)
        {
            var distMatrix = new List<List<string>>();
            var predMatrix = new List<List<int>>();

            for (int i = 0; i < n; i++)
            {
                var distRow = new List<string>();
                var predRow = new List<int>();

                for (int j = 0; j < n; j++)
                {
                    distRow.Add(double.IsPositiveInfinity(dist[i, j])
                        ? "∞"
                        : (dist[i, j] == (int)dist[i, j]
                            ? ((int)dist[i, j]).ToString()
                            : dist[i, j].ToString()));

                    predRow.Add(pred[i, j]);
                }

                distMatrix.Add(distRow);
                predMatrix.Add(predRow);
            }

            return new FloydStep
            {
                DistanceMatrix = distMatrix,
                PredecessorMatrix = predMatrix,
            };
        }
    }
}