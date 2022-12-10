using System.Runtime.InteropServices;

Point headPosition = default;
Point tailOffset = default;



Dictionary<Direction, Dictionary<Point, Point>> TailTransitions = new Dictionary<Direction, Dictionary<Point, Point>>
{
    { Direction.U, new Dictionary<Point, Point>
        {
            { new Point(-1, -1), new Point (0, -1) },
            { new Point(0, -1), new Point (0, -1) },
            { new Point(1, -1), new Point (0, -1) },
            { new Point(-1, 0), new Point (-1, -1) },
            { new Point(0, 0), new Point (0, -1) },
            { new Point(1, 0), new Point (1, -1) },
            { new Point(-1, 1), new Point (-1, 0) },
            { new Point(0, 1), new Point (0, 0) },
            { new Point(1, 1), new Point (1, 0) },
        }
    },
    {Direction.D, new Dictionary<Point, Point>
        {
            { new Point(-1, -1), new Point (-1, 0) },
            { new Point(0, -1), new Point (0, 0) },
            { new Point(1, -1), new Point (1, 0) },
            { new Point(-1, 0), new Point (-1, 1) },
            { new Point(0, 0), new Point (0, 1) },
            { new Point(1, 0), new Point (1, 1) },
            { new Point(-1, 1), new Point (0, 1) },
            { new Point(0, 1), new Point (0, 1) },
            { new Point(1, 1), new Point (0, 1) },
        }
    },
    { Direction.L, new Dictionary<Point, Point>
        {
            { new Point(1, 1), new Point (1, 0) },
            { new Point(1, 0), new Point (1, 0) },
            { new Point(1, -1), new Point (1, 0) },
            { new Point(0, 1), new Point (1, 1) },
            { new Point(0, 0), new Point (1, 0) },
            { new Point(0, -1), new Point (1, -1) },
            { new Point(-1, 1), new Point (0, 1) },
            { new Point(-1, 0), new Point (0, 0) },
            { new Point(-1, -1), new Point (0, -1) },
        }
    },
    { Direction.R, new Dictionary<Point, Point>
        {
            { new Point(1, 1), new Point (0, 1) },
            { new Point(1, 0), new Point (0, 0) },
            { new Point(1, -1), new Point (0, -1) },
            { new Point(0, 1), new Point (-1, 1) },
            { new Point(0, 0), new Point (-1, 0) },
            { new Point(0, -1), new Point (-1, -1) },
            { new Point(-1, 1), new Point (-1, 0) },
            { new Point(-1, 0), new Point (-1, 0) },
            { new Point(-1, -1), new Point (-1, 0) },
      }
    },
};

Dictionary<Direction, Point> HeadTransitions = new Dictionary<Direction, Point>
{
    { Direction.U, new Point(0, 1) },
    { Direction.D, new Point(0, -1) },
    { Direction.L, new Point(-1, 0) },
    { Direction.R, new Point(1, 0) },
};

HashSet<Point> tailVisits = new HashSet<Point>();

// Q1 - easy!
foreach (var line in File.ReadLines("input.txt"))
{
    var parts = line.Split(' ');
    (var direction, var count) = (Enum.Parse<Direction>(parts[0]), int.Parse(parts[1]));
    while (count-- > 0) {
        headPosition = headPosition.Offset(HeadTransitions[direction]);
        tailOffset = TailTransitions[direction][tailOffset];
        tailVisits.Add(headPosition.Offset(tailOffset));
    }
}
Console.WriteLine($"Q1: {tailVisits.Count}");

readonly struct Point
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
        => (X, Y) = (x, y);

    public Point Offset(Point other)
        => new Point(X + other.X, Y + other.Y);

    public override string ToString()
        => $"{X}, {Y}";
}

enum Direction
{
    U, D, L, R
}
