var results = File.ReadAllLines("input.txt")
    .Select(t => t.Split(' '))
    .Select(t => (t[0], int.Parse(t[1])))
    .GroupBy(t => t.Item1, v => v.Item2)
    .ToDictionary(group => group.Key, group => group.Sum());
var depth = results["down"] - results["up"];
var forward = results["forward"];
Console.WriteLine($"Depth: {depth}. Forward: {forward}. Product: {forward * depth}");
