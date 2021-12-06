var vectors = File.ReadLines("input.txt")
    .Select(t => t.Split(" -> "))
    .Select(t => new Vector(t[0].Split (',').Select (int.Parse).ToArray (), t[1].Split(',').Select (int.Parse).ToArray ()))
    .ToArray();

Dictionary<Point, int> collisions = new Dictionary<Point, int>();
foreach (var vector in vectors)
{
    if (vector.Start.X == vector.End.X)
    {
        for (int i = Math.Min(vector.Start.Y, vector.End.Y); i <= Math.Max(vector.Start.Y, vector.End.Y); i++)
        {
            var key = new Point(vector.Start.X, i);
            if (!collisions.ContainsKey(key))
                collisions[key] = 1;
            else
                collisions[key]++;
        }
    }
    else if (vector.Start.Y == vector.End.Y)
    {
        for (int i = Math.Min(vector.Start.X, vector.End.X); i <= Math.Max(vector.Start.X, vector.End.X); i++)
        {
            var key = new Point(i, vector.Start.Y);
            if (!collisions.ContainsKey(key))
                collisions[key] = 1;
            else
                collisions[key]++;
        }
    }
}
Console.WriteLine($"Total intersections: {collisions.Values.Where(t => t > 1).Count()}");


/*
for (int i = 0; i < horzVertVectors.Length; i ++)
{
    for (int j = i + 1; j < horzVertVectors.Length; j++)
    {
        var vectorA = horzVertVectors[i];
        var vectorB = horzVertVectors[j];

        var determinant = vectorA.A * vectorB.B - vectorA.B * vectorB.A;
        // These ones are parallel... now we should check if they overlap?
        if (determinant == 0)
        {
            if (vectorA.Start.X == vectorB.Start.X && vectorA.End.X == vectorB.End.X)
            {
            }

            if (vectorA.Start.Y == vectorB.Start.Y && vectorA.End.Y == vectorB.End.Y)
            {
            }
        }
        else
        {
            var intersection = new Point(
                (vectorB.B * vectorA.C - vectorA.B * vectorB.C) / determinant,
                (vectorA.A * vectorB.C - vectorB.A * vectorA.C) / determinant
            );

            if (vectorA.Contains(intersection) && vectorB.Contains(intersection))
                collisions.Add(intersection);
        }
    }
}

Console.WriteLine("Vectors");

// Needed?
static double CalculateSide (Vector segment, Point point)
{
    // A = (x1, y1) to B = (x2, y2) a point P = (x, y)
    // d = (x-x1)(y2-y1) - (y-y1)(x2-x1)
    return (point.X - segment.Start.X) * (segment.End.Y - segment.Start.Y)
         - (point.Y - segment.Start.Y) * (segment.End.X - segment.Start.X);  
}

*/
readonly struct Vector
{
    // ax + by = c
    public double A => End.Y - Start.Y;
    public double B => Start.X - End.X;
    public double C => A * Start.X + B * Start.Y;

    public readonly Point Start;
    public readonly Point End;

    public Vector(int[] start, int[] end)
        => (Start, End) = (new Point(start[0], start[1]), new Point(end[0], end[1]));

    public bool Contains(Point point)
        => point.X >= Math.Min(Start.X, End.X) && point.X <= Math.Max(Start.X, End.X)
        && point.Y >= Math.Min(Start.Y, End.Y) && point.Y <= Math.Max(Start.Y, End.Y);

    public override string ToString()
        => $"{Start} -> {End}";
}

readonly struct Point : IEquatable<Point>
{
    public readonly int X;
    public readonly int Y;

    public Point(int x, int y)
       => (X, Y) = (x, y);

    public override int GetHashCode()
        => X;

    public override bool Equals(object? obj)
        => obj is Point x && Equals(x);

    public bool Equals(Point other)
        => X == other.X && Y == other.Y;

    public override string ToString()
        => $"{X},{Y}";
}

/*
readonly struct PointD : IEquatable<Point>
{
    public readonly double X;
    public readonly double Y;

    public Point(double x, double y)
       => (X, Y) = (x, y);

    public override int GetHashCode()
        => (int)X;

    public override bool Equals(object? obj)
        => obj is Point x && Equals(x);

    public bool Equals(Point other)
        => Math.Abs(X - other.X) < 0.01 && Math.Abs(Y - other.Y) < 0.01;

    public override string ToString()
        => $"{X},{Y}";
}
*/