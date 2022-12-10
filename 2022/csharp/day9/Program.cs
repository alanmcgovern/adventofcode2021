
using System.Diagnostics.CodeAnalysis;

var moves = File.ReadLines("input.txt")
    .Select(t => t.Split(' '))
    .Select(parts => (ParseDirection(parts[0]), int.Parse(parts[1])))
    .ToArray();

Console.WriteLine($"Q1: {CountTailVisits(2)}");
Console.WriteLine($"Q2: {CountTailVisits(10)}");


int CountTailVisits (int knotCount)
{
    var tailVisits = new HashSet<Point>();

    var knots = new Point[knotCount];
    foreach (var move in moves)
    {
        (var direction, var count) = move;
        while (count-- > 0)
        {
            knots[0] = knots[0].Offset(direction);
            for (int knot = 1; knot < knots.Length; knot++)
                knots[knot] = CalculateNewPosition(knots[knot - 1], knots[knot]);
            tailVisits.Add(knots.Last ());
        }
    }
    return tailVisits.Count;
}

Point CalculateNewPosition(Point headPoint, Point tailPoint)
{
    if (headPoint.X == tailPoint.X)
        if (Math.Abs(headPoint.Y - tailPoint.Y) == 2)
            return tailPoint.Offset(0, headPoint.Y > tailPoint.Y ? 1 : -1);

    if(headPoint.Y == tailPoint.Y)
        if (Math.Abs(headPoint.X - tailPoint.X) == 2)
            return tailPoint.Offset(headPoint.X > tailPoint.X ? 1 : -1, 0);

    if (Math.Abs(headPoint.X - tailPoint.X) <= 1 && Math.Abs(headPoint.Y - tailPoint.Y) <= 1)
        return tailPoint;

    return tailPoint.Offset(headPoint.X > tailPoint.X ? 1 : -1, headPoint.Y > tailPoint.Y ? 1 : -1);
}

Point ParseDirection(string str)
{
    return str switch
    {
        "D" => new Point(0, -1),
        "U" => new Point(0, 1),
        "L" => new Point(-1, 0),
        "R" => new Point(1, 0),
        _ => throw new NotSupportedException(),
    };
}

readonly struct Point
{
    public static Point Zero { get; } = new Point(0, 0);

    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
        => (X, Y) = (x, y);

    public Point Offset(int x, int y)
        => Offset(new Point(x, y));

    public Point Offset(Point other)
        => new Point(X + other.X, Y + other.Y);

    public override string ToString()
        => $"{X}, {Y}";
}
