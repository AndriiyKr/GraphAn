// <copyright file="GraphMetricsService.cs" company="GraphAn">
// Copyright (c) GraphAn. All rights reserved.
// </copyright>

namespace GraphAn.BLL.Services
{
    using GraphAn.BLL.Interfaces;
    using GraphAn.BLL.Models;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Сервіс для обчислення метрик графа.
    /// </summary>
    public class GraphMetricsService : IGraphMetricsService
    {
        private readonly ILogger<GraphMetricsService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphMetricsService"/> class.
        /// </summary>
        /// <param name="logger">Об'єкт логера.</param>
        public GraphMetricsService(ILogger<GraphMetricsService> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public GraphInfoResult GetGraphInfo(GraphDto graph)
        {
            var type = graph.Directed ? "Орієнтований" : "Неорієнтований";

            this.logger.LogInformation("Отримано базову інформацію про граф");

            return new GraphInfoResult
            {
                GraphType = type,
                NodeCount = graph.Nodes.Count,
                EdgeCount = graph.Edges.Count,
            };
        }

        /// <inheritdoc/>
        public List<List<int>> GetAdjacencyMatrix(GraphDto graph)
        {
            var nodes = graph.Nodes;
            int n = nodes.Count;

            var matrix = new List<List<int>>();
            for (int i = 0; i < n; i++)
            {
                matrix.Add(new List<int>(new int[n]));
            }

            var nodeIndex = new Dictionary<string, int>();
            for (int i = 0; i < n; i++)
            {
                nodeIndex[nodes[i].Id] = i;
            }

            foreach (var edge in graph.Edges)
            {
                if (!nodeIndex.TryGetValue(edge.From, out int u) ||
                    !nodeIndex.TryGetValue(edge.To, out int v))
                {
                    continue;
                }

                matrix[u][v]++;

                if (!graph.Directed && edge.From != edge.To)
                {
                    matrix[v][u]++;
                }
            }

            this.logger.LogInformation("Обчислено матрицю суміжності графа");
            return matrix;
        }

        /// <inheritdoc/>
        public List<List<int>> GetIncidenceMatrix(GraphDto graph)
        {
            var nodes = graph.Nodes;
            var edges = graph.Edges;
            int n = nodes.Count;
            int m = edges.Count;

            if (n == 0 || m == 0)
            {
                return new List<List<int>>();
            }

            var matrix = new List<List<int>>();
            for (int i = 0; i < n; i++)
            {
                matrix.Add(new List<int>(new int[m]));
            }

            var nodeIndex = new Dictionary<string, int>();
            for (int i = 0; i < n; i++)
            {
                nodeIndex[nodes[i].Id] = i;
            }

            for (int j = 0; j < m; j++)
            {
                var edge = edges[j];
                if (!nodeIndex.TryGetValue(edge.From, out int u) ||
                    !nodeIndex.TryGetValue(edge.To, out int v))
                {
                    continue;
                }

                if (edge.From == edge.To)
                {
                    matrix[u][j] = 2;
                }
                else if (graph.Directed)
                {
                    matrix[u][j] = -1;
                    matrix[v][j] = 1;
                }
                else
                {
                    matrix[u][j] = 1;
                    matrix[v][j] = 1;
                }
            }

            this.logger.LogInformation("Обчислено матрицю інцидентності графа");
            return matrix;
        }

        /// <inheritdoc/>
        public AdjacencyListResult GetAdjacencyList(GraphDto graph)
        {
            var nodes = graph.Nodes;
            var result = new List<AdjacencyEntry>();

            var nodeLabels = nodes.ToDictionary(n => n.Id, n => n.Label);

            var inDegree = nodes.ToDictionary(n => n.Id, _ => 0);
            var outDegree = nodes.ToDictionary(n => n.Id, _ => 0);
            var neighbors = nodes.ToDictionary(n => n.Id, _ => new List<string>());

            foreach (var edge in graph.Edges)
            {
                if (!nodeLabels.ContainsKey(edge.From) || !nodeLabels.ContainsKey(edge.To))
                {
                    continue;
                }

                outDegree[edge.From]++;
                inDegree[edge.To]++;

                if (!neighbors[edge.From].Contains(nodeLabels[edge.To]))
                {
                    neighbors[edge.From].Add(nodeLabels[edge.To]);
                }

                if (!graph.Directed && !neighbors[edge.To].Contains(nodeLabels[edge.From]))
                {
                    neighbors[edge.To].Add(nodeLabels[edge.From]);
                }
            }

            var degrees = new List<int>();

            foreach (var node in nodes.OrderBy(n => n.Id))
            {
                var entry = new AdjacencyEntry
                {
                    Vertex = nodeLabels[node.Id],
                    Neighbors = neighbors[node.Id].Any()
                        ? string.Join(", ", neighbors[node.Id])
                        : "—",
                };

                if (graph.Directed)
                {
                    entry.InDegree = inDegree[node.Id];
                    entry.OutDegree = outDegree[node.Id];
                    entry.Degree = inDegree[node.Id] + outDegree[node.Id];
                }
                else
                {
                    entry.Degree = inDegree[node.Id] + outDegree[node.Id];
                }

                degrees.Add(entry.Degree);
                result.Add(entry);
            }

            bool isRegular = degrees.Count > 0 && degrees.All(d => d == degrees[0]);

            this.logger.LogInformation("Отримано список суміжності графа");
            return new AdjacencyListResult
            {
                AdjacencyList = result,
                IsRegular = isRegular,
            };
        }

        /// <inheritdoc/>
        public CycleAndChromaticResult GetCycleAndChromatic(GraphDto graph)
        {
            var result = new CycleAndChromaticResult();

            // --- Хроматичне число (жадібний алгоритм) ---
            var coloring = new Dictionary<string, int>();
            foreach (var node in graph.Nodes)
            {
                var neighborColors = graph.Edges
                    .Where(e => e.From == node.Id || (!graph.Directed && e.To == node.Id))
                    .Select(e => e.From == node.Id ? e.To : e.From)
                    .Where(coloring.ContainsKey)
                    .Select(n => coloring[n])
                    .ToHashSet();

                int color = 0;
                while (neighborColors.Contains(color))
                {
                    color++;
                }

                coloring[node.Id] = color;
            }

            result.ChromaticNumber = coloring.Values.DefaultIfEmpty(-1).Max() + 1;
            result.Coloring = coloring;

            // --- Найкоротший цикл (BFS з кожної вершини) ---
            var nodeIds = graph.Nodes.Select(n => n.Id).ToList();
            var nodeLabels = graph.Nodes.ToDictionary(n => n.Id, n => n.Label);

            var adjList = nodeIds.ToDictionary(id => id, _ => new List<string>());
            foreach (var edge in graph.Edges)
            {
                if (adjList.ContainsKey(edge.From) && adjList.ContainsKey(edge.To))
                {
                    adjList[edge.From].Add(edge.To);
                    if (!graph.Directed)
                    {
                        adjList[edge.To].Add(edge.From);
                    }
                }
            }

            List<string>? shortestCycle = null;

            foreach (var start in nodeIds)
            {
                var parent = new Dictionary<string, string?> { [start] = null };
                var queue = new Queue<string>();
                queue.Enqueue(start);

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    foreach (var neighbor in adjList[current])
                    {
                        if (!parent.ContainsKey(neighbor))
                        {
                            parent[neighbor] = current;
                            queue.Enqueue(neighbor);
                        }
                        else if (parent[current] != neighbor)
                        {
                            // знайшли цикл — відновлюємо шлях
                            var cycle = new List<string> { current, neighbor };
                            var node = current;
                            while (parent[node] != null && parent[node] != neighbor)
                            {
                                node = parent[node] !;
                                cycle.Insert(0, node);
                            }

                            cycle.Insert(0, neighbor);

                            if (shortestCycle == null || cycle.Count < shortestCycle.Count)
                            {
                                shortestCycle = cycle;
                            }
                        }
                    }
                }
            }

            if (shortestCycle != null)
            {
                result.HasCycle = true;
                result.Girth = shortestCycle.Count - 1;
                result.CyclePath = shortestCycle
                    .Select(id => nodeLabels.TryGetValue(id, out var label) ? label : id)
                    .ToList();
            }

            this.logger.LogInformation("Обчислено найкоротший цикл та хроматичне число графа");
            return result;
        }

        /// <inheritdoc/>
        public ConnectivityResult GetConnectivity(GraphDto graph)
        {
            var nodeIds = graph.Nodes.Select(n => n.Id).ToHashSet();

            // --- Компоненти зв'язності (BFS) ---
            var visited = new HashSet<string>();
            var adjList = nodeIds.ToDictionary(id => id, _ => new List<string>());

            foreach (var edge in graph.Edges)
            {
                if (!nodeIds.Contains(edge.From) || !nodeIds.Contains(edge.To))
                {
                    continue;
                }

                adjList[edge.From].Add(edge.To);
                adjList[edge.To].Add(edge.From);
            }

            int components = 0;
            foreach (var node in nodeIds)
            {
                if (!visited.Contains(node))
                {
                    components++;
                    var queue = new Queue<string>();
                    queue.Enqueue(node);
                    visited.Add(node);
                    while (queue.Count > 0)
                    {
                        var current = queue.Dequeue();
                        foreach (var neighbor in adjList[current])
                        {
                            if (!visited.Contains(neighbor))
                            {
                                visited.Add(neighbor);
                                queue.Enqueue(neighbor);
                            }
                        }
                    }
                }
            }

            var result = new ConnectivityResult { ComponentsCount = components };

            if (components != 1)
            {
                this.logger.LogInformation("Граф незв'язний — зв'язність рівна 0");
                return result;
            }

            // --- Вершинна зв'язність (видаляємо по одній вершині) ---
            int vertexConnectivity = nodeIds.Count;
            foreach (var removed in nodeIds)
            {
                var remaining = nodeIds.Where(n => n != removed).ToHashSet();
                if (!this.IsConnected(remaining, graph.Edges))
                {
                    vertexConnectivity = 1;
                    break;
                }
            }

            result.VertexConnectivity = vertexConnectivity == nodeIds.Count ? nodeIds.Count - 1 : 1;

            // --- Реберна зв'язність (видаляємо по одному ребру) ---
            int edgeConnectivity = graph.Edges.Count;
            for (int i = 0; i < graph.Edges.Count; i++)
            {
                var edgesWithout = graph.Edges.Where((_, idx) => idx != i).ToList();
                if (!this.IsConnected(nodeIds, edgesWithout))
                {
                    edgeConnectivity = 1;
                    break;
                }
            }

            result.EdgeConnectivity = edgeConnectivity == graph.Edges.Count ? graph.Edges.Count : 1;

            this.logger.LogInformation("Обчислено числа зв'язності графа");
            return result;
        }

        /// <inheritdoc/>
        public IndependenceAndCliqueResult GetIndependenceAndClique(GraphDto graph)
        {
            var nodes = graph.Nodes.Select(n => n.Id).ToList();
            var edgeSet = new HashSet<string>(
                graph.Edges.Select(e => $"{e.From}-{e.To}"));

            bool HasEdge(string u, string v) =>
                edgeSet.Contains($"{u}-{v}") || edgeSet.Contains($"{v}-{u}");

            // --- Число незалежності (жадібний алгоритм) ---
            var independentSet = new List<string>();
            foreach (var node in nodes)
            {
                if (independentSet.All(n => !HasEdge(n, node)))
                {
                    independentSet.Add(node);
                }
            }

            // --- Клікове число (жадібний алгоритм) ---
            var clique = new List<string>();
            foreach (var node in nodes)
            {
                if (clique.All(n => HasEdge(n, node)))
                {
                    clique.Add(node);
                }
            }

            this.logger.LogInformation("Обчислено число незалежності та клікове число графа");
            return new IndependenceAndCliqueResult
            {
                IndependenceNumber = independentSet.Count,
                CliqueNumber = clique.Count,
            };
        }

        /// <inheritdoc/>
        public GraphMetricsResult GetAllMetrics(GraphDto graph)
        {
            this.logger.LogInformation("Обчислення всіх метрик графа");

            return new GraphMetricsResult
            {
                Info = this.GetGraphInfo(graph),
                AdjacencyMatrix = this.GetAdjacencyMatrix(graph),
                IncidenceMatrix = this.GetIncidenceMatrix(graph),
                AdjacencyList = this.GetAdjacencyList(graph),
                CycleAndChromatic = this.GetCycleAndChromatic(graph),
                Connectivity = this.GetConnectivity(graph),
                IndependenceAndClique = this.GetIndependenceAndClique(graph),
            };
        }

        /// <summary>
        /// Перевіряє чи є граф зв'язним після видалення вершин або ребер.
        /// </summary>
        /// <param name="nodes">Множина вершин.</param>
        /// <param name="edges">Список ребер.</param>
        /// <returns><see langword="true"/> якщо граф зв'язний; інакше <see langword="false"/>.</returns>
        private bool IsConnected(HashSet<string> nodes, List<EdgeDto> edges)
        {
            if (nodes.Count == 0)
            {
                return true;
            }

            var adjList = nodes.ToDictionary(id => id, _ => new List<string>());
            foreach (var edge in edges)
            {
                if (!nodes.Contains(edge.From) || !nodes.Contains(edge.To))
                {
                    continue;
                }

                adjList[edge.From].Add(edge.To);
                adjList[edge.To].Add(edge.From);
            }

            var visited = new HashSet<string>();
            var queue = new Queue<string>();
            var start = nodes.First();
            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var neighbor in adjList[current])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return visited.Count == nodes.Count;
        }
    }
}