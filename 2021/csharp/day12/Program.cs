var caveConnections = File.ReadLines("input.txt")
    .Select(t => t.Split('-'))
    .ToArray();

// Connect the caves (just like the previous question)
var caves = new Dictionary<string, Cave>();
foreach (var connection in caveConnections)
{
    var currentPair = connection.Select(t => caves.TryGetValue(t, out Cave? cave) ? cave : (caves[t] = new Cave(t))).ToArray();
    currentPair[0].Connections.Add(currentPair[1]);
    if (!currentPair[0].IsStart)
        currentPair[1].Connections.Add(currentPair[0]);
}

// Walk through the caves
var completePaths = new HashSet<string>();
foreach (var cave in caves.Values.Single (t => t.IsStart).Connections)
    TryTravel(cave, new List<Cave> (), completePaths);
Console.WriteLine($"Total Paths: {completePaths.Count}");

static void TryTravel (Cave cave, List<Cave> currentPath, HashSet<string> completePaths, bool canUseSmallTwice)
{
    if (cave.Travel ())
    {
        currentPath.Add(cave);
        if (cave.IsEnd)
        {
            completePaths.Add(string.Join(" -> ", currentPath.Select(t => t.Name)));
        }
        else
        {
            foreach (var connection in cave.Connections)
                TryTravel(connection, currentPath, completePaths);
        }
        cave.UnTravel();
        currentPath.RemoveAt(currentPath.Count - 1);
    }
}

class Cave
{
    int taken = 0;

    public List<Cave> Connections { get; } = new List<Cave>();
    public string Name { get; }

    public bool IsStart
        => Name == "start";

    public bool IsEnd
        => Name == "end";

    public Cave(string name)
        => Name = name;

    public bool Travel()
    {
        if (taken > 0 && char.IsLower(Name[0]))
            return false;
        taken++;
        return true;
    }

    public void UnTravel()
        => taken--;

    public override string ToString()
        => Name;
}
