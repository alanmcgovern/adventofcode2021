using System.Diagnostics;

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

var stopwatch = Stopwatch.StartNew();
// Walk through the caves
var completePaths = 0;
foreach (var cave in caves.Values.Single (t => t.IsStart).Connections)
    TryTravel(cave, ref completePaths, false);
Console.WriteLine($"Total Paths: {completePaths}. Took {stopwatch.Elapsed.TotalSeconds:0.00} seconds");

// Walk through the caves, allowing double visits to small caves.
completePaths = 0;
stopwatch.Restart();
foreach (var cave in caves.Values.Single(t => t.IsStart).Connections)
    TryTravel(cave, ref completePaths, true);
Console.WriteLine($"Total Paths (with doubles): {completePaths}. Took {stopwatch.Elapsed.TotalSeconds:0.00} seconds");


void TryTravel (Cave cave, ref int completePaths, bool allowSmallTwice)
{
    // If we're allowed visit a cave twice, we should try to visit it 1 and 2 times.
    // If we're not allowed visit it twice, we'll just visit once because 'false' will
    // be in the array twice.
    if (cave.Travel(allowSmallTwice && caves.Values.All (c => !c.IsSmall || c.Visitations < 2)))
    {
        if (cave.IsEnd)
        {
            completePaths++;
        }
        else
        {
            foreach (var connection in cave.Connections)
                TryTravel(connection, ref completePaths, allowSmallTwice);
        }
        cave.UnTravel();
    }
}

class Cave
{
    public List<Cave> Connections { get; } = new List<Cave>();
    public string Name { get; }

    public bool IsEnd { get; }
    public bool IsSmall { get; }
    public bool IsStart { get; }
    public int Visitations { get; private set; }

    public Cave(string name)
        => (Name, IsStart, IsEnd, IsSmall) = (name, name == "start", name == "end", char.IsLower(name[0]));

    public bool Travel(bool allowSmallTwice)
    {
        if (IsSmall && Visitations > 0)
        {
            if (!allowSmallTwice || Visitations > 1)
                return false;
        }
        Visitations++;
        return true;
    }

    public void UnTravel()
        => Visitations--;

    public override string ToString()
        => $"{Name}: {Visitations}";
}
