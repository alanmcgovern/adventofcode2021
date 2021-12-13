using System.Drawing;

using Instructions = System.ReadOnlyMemory<(Axis axis, int position)>;

var input = File.ReadAllLines("input.txt");


ReadOnlyMemory<Point> points = input
    .TakeWhile(t => !string.IsNullOrEmpty(t))
    .Select(t => t.Split(','))
    .Select(t => new Point(int.Parse(t[0]), int.Parse(t[1])))
    .ToArray();

Instructions instructions = input.SkipWhile(t => !string.IsNullOrEmpty(t)).Skip (1)
    .Select(t => t.Split(' ').Last())
    .Select(t => t.Split('='))
    .Select<string[], (Axis axis, int position)>(t => (Enum.Parse<Axis>(t[0], true), int.Parse(t[1])))
    .ToArray();

// Q1
var result = points;
foreach (var instruction in instructions.Span.ToArray ().Take (1))
    result = Fold(instruction.axis, instruction.position, result);
Console.WriteLine($"Total visible: {new HashSet<Point>(result.Span.ToArray()).Count}");


// Q2


static ReadOnlyMemory<Point> Fold (Axis axis, int position, ReadOnlyMemory<Point> points)
{
    var result = new List<Point> (points.Length);
    foreach (var point in points.Span)
    {
        switch (axis)
        {
            case Axis.X:
                if (point.X == position)
                    continue;
                if (point.X < position)
                    result.Add(point);
                else
                    result.Add(new Point(point.X - 2 * (point.X - position), point.Y));
                break;

            case Axis.Y:
                if (point.Y == position)
                    continue;
                if (point.Y < position)
                    result.Add(point);
                else
                    result.Add(new Point(point.X, point.Y - 2 * (point.Y - position)));
                break;
        }
    }
    return result.ToArray();
}

enum Axis
{
    X,
    Y
}