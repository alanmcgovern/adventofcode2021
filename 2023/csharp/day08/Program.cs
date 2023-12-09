(var path, var connections) = Parse(File.ReadAllLines("input.txt"));
var mapper = connections.ToDictionary(t => t.Id);

Console.WriteLine($"Q1: {CountStepsQ1()}");
Console.WriteLine($"Q2: {CountStepsQ2()}");

long CountStepsQ1 ()
{
    long steps = 0;
    var position = mapper["AAA"];
    while (position.Id != "ZZZ")
    {
        position = mapper[path[(int)(steps % path.Length)] == 'R' ? position.Right : position.Left];
        steps++;
    }
    return steps;
}

long CountStepsQ2()
{
    var positions = mapper.Where(t => t.Key.EndsWith('A')).Select(t => t.Value).ToArray();

    var loops = positions.Select(LoopLength).ToArray();
    foreach (var loop in loops)
        Console.WriteLine($"TimeToStart: {loop.loopStart}. Time to loop: {loop.loopLength}");

    //Convienently loopStart == loopLength. So we just need the LCM.
    return loops.Select(t => t.loopLength).Aggregate(lcm);
}

long gcd(long a, long b)
{
    while (b != 0)
        (a, b) = (b, a % b);
    return a;
}

long lcm(long a, long b)
    => a * b / gcd(a, b);

(long loopStart, long loopLength) LoopLength(Connection position)
{
    var firstZ = 0;
    int pathIndex = 0;
    var positions = new Dictionary<(int, string), int>();
    while (!positions.ContainsKey((pathIndex % path.Length, position.Id)))
    {
        if (position.Id.EndsWith('Z'))
        {
            firstZ = pathIndex;
            Console.WriteLine($"{position.Id} - {pathIndex}");
        }
        positions[((pathIndex % path.Length, position.Id))] = pathIndex;
        position = mapper[path[pathIndex % path.Length] == 'R' ? position.Right : position.Left];
        pathIndex++;
    }
    var originalStart = positions[(pathIndex % path.Length, position.Id)];
    return (firstZ, pathIndex - originalStart);
}

static (string path, IEnumerable<Connection> connections) Parse(string[] input)
{
    var path = input[0];
    var connections = new List<Connection>();
    foreach (var line in input.Skip(2))
    {
        var parts = line.Split('=');
        var source = parts[0].Trim();
        var dests = parts[1]
            .Replace("(", "")
            .Replace(")", "")
            .Split(',');
        connections.Add(new Connection(source, dests[0].Trim(), dests[1].Trim()));
    }
    return (path, connections);
}


class Connection(string id, string left, string right)
{
    public string Id { get; } = id;
    public string Left { get; } = left;
    public string Right { get; } = right;
}