var input = File.ReadLines("input.txt")
    .Select(t => t.ToArray())
    .ToArray();

var segments = new List<Segment>
{
    new Segment ('(', ')', 3),
    new Segment ('{', '}', 1197),
    new Segment ('[', ']', 57),
    new Segment ('<', '>', 25137),
};

var value = 0;
foreach (var line in input)
{
    var stack = new Stack<char>();
    foreach (var c in line)
    {
        if (segments.Any (t => t.Opening == c))
            stack.Push(c);
        if (segments.Any(t => t.Closing == c))
        {
            if (stack.Count == 0)
                break;
            else if (segments.Single (t => t.Closing == c).Opening != stack.Pop())
                value += segments.Single (segment => segment.Closing == c).Points;
        }
    }
}
Console.WriteLine($"Parser points: {value}");


readonly struct Segment
{
    public readonly char Opening;
    public readonly char Closing;
    public readonly int Points;

    public Segment(char opening, char closing, int points)
        => (Opening, Closing, Points) = (opening, closing, points);
}