using System.Drawing;

var input = File.ReadAllLines("input.txt");


var points = input
    .TakeWhile(t => !string.IsNullOrEmpty(t))
    .Select(t => t.Split(','))
    .Select(t => new Point(int.Parse(t[0]), int.Parse(t[1])))
    .ToArray();

var instructions = input.SkipWhile(t => !string.IsNullOrEmpty(t)).Skip (1)
    .Select(t => t.Split(' ').Last())
    .Select(t => t.Split('='))
    .Select<string[], (Axis axis, int position)>(t => (Enum.Parse<Axis>(t[0], true), int.Parse(t[1])))
    .ToArray();

// Q1
IEnumerable<Point> result = points;
foreach (var instruction in instructions.Take(1))
    result = instruction.axis == Axis.X ? FoldX(result, instruction.position) : FoldY(result, instruction.position);
Console.WriteLine($"Total visible: {new HashSet<Point>(result).Count}");

// Q2
result = points;
foreach (var instruction in instructions)
    result = instruction.axis == Axis.X ? FoldX(result, instruction.position) : FoldY(result, instruction.position);

// Lets make a guess that i need to print this out in order to understand it...
var lines = new List<string>();
foreach (var pointSet in result.Distinct ().GroupBy(p => p.Y).OrderBy (p => p.Key))
{
    while (pointSet.Key < lines.Count)
        lines.Add("");

    var currentLine = "";
    foreach (var point in pointSet.OrderBy(p => p.X))
        currentLine += new string(' ', point.X - currentLine.Length) + '#';
    lines.Add(currentLine);
}

Console.WriteLine("Password:");
Console.WriteLine(string.Join (Environment.NewLine,lines));

static IEnumerable<Point> FoldX(IEnumerable<Point> points, int position)
    => Fold(points, position, p => p.X, (point, offset) => new Point(offset, point.Y));

static IEnumerable<Point> FoldY(IEnumerable<Point> points, int position)
    => Fold(points, position, p => p.Y, (point, offset) => new Point(point.X, offset));

static IEnumerable<Point> Fold(IEnumerable<Point> points, int position, Func<Point, int> selector, Func<Point, int, Point> updater)
    => points.Where(t => position != selector(t))
             .Select(t => selector(t) < position ? t : updater(t, selector (t) - 2 * (selector(t) - position)));

enum Axis { X, Y }