using System.Linq;
using System.Text.RegularExpressions;

var rawInput = File.ReadAllLines("input.txt");
var setup = rawInput.TakeWhile(t => t.Trim().Length != 0).ToArray();
var steps = rawInput.Skip(setup.Length + 1).ToArray();

Console.WriteLine($"Q1: {RunSteps(false)}");
Console.WriteLine($"Q2: {RunSteps(true)}");


string RunSteps(bool useStacker9001)
{
    var towers = new Dictionary<int, Stack<char>>();
    foreach (var contents in setup.Reverse().Skip(1))
    {
        int tower = 1;
        var span = contents.AsSpan();
        while (span.Length > 0)
        {
            if (!towers.TryGetValue(tower, out var stack))
                towers[tower] = stack = new Stack<char>();

            ref readonly char content = ref span[1];
            if (content >= 'A' && content <= 'Z')
                stack.Push(content);

            tower++;
            span = span.Slice(Math.Min(span.Length, 4));
        }
    }

    var regex = new Regex(@"move (\d*) from (\d*) to (\d*)", RegexOptions.NonBacktracking | RegexOptions.CultureInvariant);
    foreach (var step in steps)
    {
        var result = regex.Match(step);
        var count = int.Parse(result.Groups[1].Value);
        var source = int.Parse(result.Groups[2].Value);
        var dest = int.Parse(result.Groups[3].Value);

        var crates = Enumerable.Range(0, count)
            .Select(t => towers[source].Pop());
        if (useStacker9001)
            crates = crates.Reverse();
        foreach (var crate in crates)
            towers[dest].Push(crate);
    }

    return string.Concat(towers.Select(t => t.Value.Pop()));
}