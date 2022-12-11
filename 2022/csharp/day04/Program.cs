
var pairs = File.ReadAllLines("input.txt")
   .Select(t => t.Split (','))
   .Select (t => (ToPoint(t[0]), ToPoint(t[1])))
   .ToArray();

Console.WriteLine($"Q1: {pairs.Count(t => t.Item1.Contains(t.Item2) || t.Item2.Contains(t.Item1))}");
Console.WriteLine($"Q2: {pairs.Count(t => t.Item1.Intersects(t.Item2))}");


Range ToPoint(string value)
    => new Range(int.Parse(value.AsSpan(0, value.IndexOf('-'))), int.Parse(value.AsSpan(value.IndexOf('-') + 1)));

readonly struct Range
{
    public int Start { get; }
    public int End { get; }
    public Range(int start, int end)
        => (Start, End) = (start, end);

    public bool Contains(Range other)
        => other.Start >= Start && other.End <= End;

    public bool Intersects(Range other)
        => other.Start <= Start ? other.End >= Start : other.Start <= End;
}