var rawCosts = File.ReadAllLines("input.txt")
    .Select(t => t.Select(t => t - '0').ToArray())
    .ToArray();

int Traverse (Graph graph, (int x, int y) source, (int x, int y) destination)
{
    var vertices = Enumerable.Range(0, graph.Length).SelectMany(v => Enumerable.Range(0, graph.Length).Select(y => (v, y))).ToHashSet();
    var distances = new Dictionary<(int x, int y), int>();
    var priorityQueue = new PriorityQueue<(int x, int y), int>();

    distances[(0, 0)] = 0;
    priorityQueue.Enqueue((0, 0), 0);
    while (priorityQueue.TryDequeue(out (int x, int y) currentNode, out int currentCost))
    {
        if (!vertices.Remove(currentNode))
            continue;

        var neighbours = new[] { (1, 0), (-1, 0), (0, 1), (0, -1) }
            .Select(t => (t.Item1 + currentNode.x, t.Item2 + currentNode.y))
            .Where(vertices.Contains)
            .ToArray();

        foreach (var neighbour in neighbours)
        {
            var newCost = currentCost + graph[neighbour];
            if (newCost < distances.GetValueOrDefault (neighbour, int.MaxValue))
            {
                distances[neighbour] = newCost;
                priorityQueue.Enqueue(neighbour, newCost);
            }
        }
    }
    return distances[destination];
}

var graphQ1 = new Graph(rawCosts, 1);
var graphQ2 = new Graph(rawCosts, 5);

Console.WriteLine($"Q1 cost: {Traverse(graphQ1, (0, 0), (graphQ1.Length - 1, graphQ1.Length - 1))}");
Console.WriteLine($"Q2 cost: {Traverse(graphQ2, (0, 0), (graphQ2.Length - 1, graphQ2.Length - 1))}");

public readonly struct Graph
{
    readonly int[][] graph;
    readonly int tiles;

    public int Length => graph.Length * tiles;

    public int this[(int x, int y) index]
    {
        get
        {
            int additionX = index.x / graph.Length;
            int additionY = index.y / graph.Length;

            var cost = graph[index.x % graph.Length][index.y % graph.Length] + (additionX + additionY);
            while (cost > 9)
                cost -= 9;
            return cost;
        }
    }

    public Graph(int[][] graph, int tiles)
        => (this.graph, this.tiles) = (graph, tiles);
}