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

var results = input.Select(line =>
{
    var stack = new Stack<char>();
    char result = line.FirstOrDefault(c =>
    {
        if (segments.Any(t => t.Opening == c))
        {
            stack.Push(c);
            return false;
        }
        return segments.Single(t => t.Closing == c).Opening != stack.Pop();
    });
    return result == 0
        ? ((long)0, stack)
        : (segments.SingleOrDefault(segment => segment.Closing == result).Points, null);
});
Console.WriteLine($"Parser points: {results.Sum (t => t.Item1)}");

// Question 2
segments = new List<Segment>
{
    new Segment ('(', ')', 1),
    new Segment ('[', ']', 2),
    new Segment ('{', '}', 3),
    new Segment ('<', '>', 4),
};

var autocompletePoints = results
    .Where (t => t.Item2 != null)
    .Select (t => t.Item2)
    .Select(line =>
        line!.Select(opener => segments.Single(t => t.Opening == opener).Points)
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
