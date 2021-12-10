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

var incompleteLines = new List<Stack<char>>();

long value = 0;
foreach (var line in input)
{
    var stack = new Stack<char>();
    bool corrupt = false;
    foreach (var c in line)
    {
        if (segments.Any(t => t.Opening == c))
        {
            stack.Push(c);
        }
        else if (segments.Any(t => t.Closing == c))
        {
            if (segments.Single(t => t.Closing == c).Opening != stack.Pop())
            {
                corrupt = true;
                value += segments.Single(segment => segment.Closing == c).Points;
            }
        }
    }
    if (!corrupt)
        incompleteLines.Add(stack);
}
Console.WriteLine($"Parser points: {value}");


// Question 2
segments = new List<Segment>
{
    new Segment ('(', ')', 1),
    new Segment ('[', ']', 2),
    new Segment ('{', '}', 3),
    new Segment ('<', '>', 4),
};

var autocompletePoints = incompleteLines
    .Select(line =>
        line.Select(opener => segments.Single(t => t.Opening == opener).Points)
       .Aggregate((a, b) => a * 5 + b)
    ).OrderBy(t => t)
    .ToArray();

Console.WriteLine($"Autocomplete points: {autocompletePoints[autocompletePoints.Length / 2]}");

readonly struct Segment
{
    public readonly char Opening;
    public readonly char Closing;
    public readonly long Points;

    public Segment(char opening, char closing, int points)
        => (Opening, Closing, Points) = (opening, closing, points);
}
