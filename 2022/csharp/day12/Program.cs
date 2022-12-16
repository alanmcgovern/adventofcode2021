using System;

var rawCosts = File.ReadAllLines("input.txt")
    .Select(t => t.Select(t => (int)t).ToArray())
    .ToArray();

static (int, int)[] GetPositions(int[][] graph, Predicate<int> canStartFrom, int replacement)
{
    var startingPositions = new List<(int, int)>();
    for (int row = 0; row < graph.Length; row++)
    {
        for (int col = 0; col < graph[row].Length; col++)
        {
            if (canStartFrom (graph[row][col]))
            {
                startingPositions.Add ((col, row));
                graph[row][col] = replacement;
            }
        }
    }

    return startingPositions.ToArray();
}

int Traverse(int[][] graph, (int x, int y) source, (int x, int y) destination)
{
    var vertices = Enumerable.Range(0, rawCosts[0].Length).SelectMany(v => Enumerable.Range(0, rawCosts.Length).Select(y => (v, y))).ToHashSet();
    var distances = new Dictionary<(int x, int y), int>();
    var priorityQueue = new PriorityQueue<(int x, int y), int>();

    distances[source] = 0;
    priorityQueue.Enqueue(source, 0);
    while (priorityQueue.TryDequeue(out (int x, int y) currentNode, out int currentCost))
    {
        if (!vertices.Remove(currentNode))
            continue;

        var neighbours = new[] { (1, 0), (-1, 0), (0, 1), (0, -1) }
            .Select(t => (t.Item1 + currentNode.x, t.Item2 + currentNode.y))
            .Where(vertices.Contains)
            .Where(neighbour => graph[currentNode.y][currentNode.x] >= graph[neighbour.Item2][neighbour.Item1] - 1)
            .ToArray();

        foreach (var neighbour in neighbours)
        {
            var newCost = distances.GetValueOrDefault(currentNode) + 1;
            if (newCost < distances.GetValueOrDefault(neighbour, int.MaxValue))
            {
                distances[neighbour] = newCost;
                priorityQueue.Enqueue(neighbour, newCost);
            }
        }
    }
    // No guarantee that we can actually make it to our destination from this starting point.
    return distances.TryGetValue(destination, out int cost) ? cost : int.MaxValue;
}

// The destination is the same for both questions.
var destPosition = GetPositions(rawCosts, val => val == 'E', 'z').Single();

// Q1 has a single possible starting position.
var startPosition = GetPositions(rawCosts, val => val == 'S', 'a').Single();
Console.WriteLine($"Q1 cost: {Traverse(rawCosts, startPosition, destPosition)}");

// Q2 has many possible starting positions. Choose the cheapest
var startPositions = GetPositions(rawCosts, val => val == 'S' || val == 'a', 'a');
var allCosts = startPositions.Select(t => Traverse(rawCosts, t, destPosition));
Console.WriteLine($"Q2 cost: {allCosts.Min()}");
