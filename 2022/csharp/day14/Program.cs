
using System.Collections.Immutable;

var terrain = Setup (File.ReadAllLines("input.txt"));

var q1Terrain = FillTerrain(terrain, floorOffset: null);
Print(q1Terrain);

var q2Terrain = FillTerrain(terrain, floorOffset: 2);
Print(q2Terrain);

Console.WriteLine($"Q1: {q1Terrain.Where(t => t.Value == Material.Sand).Count()}");
Console.WriteLine($"Q2: {q2Terrain.Where(t => t.Value == Material.Sand).Count()}");

static ImmutableDictionary<Point, Material> FillTerrain(ImmutableDictionary<Point, Material> initialTerrain, int? floorOffset)
{
    var terrain = new Dictionary<Point, Material>(initialTerrain);

    // The first position for every piece of sand is 1 pixel below the source point.
    var start = terrain.Where(t => t.Value == Material.Source).Select(t => t.Key).FirstOrDefault();

    // If any sand exceeds this Y position, we've overflowed.
    var lowestLevel = terrain.Select(t => t.Key.Y).Max() + floorOffset.GetValueOrDefault();

    var down = new Point(0, 1);
    var downLeft = new Point(-1, 1);
    var downRight = new Point(1, 1);

    var sand = start;
    while (sand.Y < lowestLevel)
    {
        if (floorOffset.HasValue && sand.Offset (down).Y == lowestLevel)
        {
            terrain[sand.Offset(down)] = Material.Rock;
            terrain[sand.Offset(downLeft)] = Material.Rock;
            terrain[sand.Offset(downRight)] = Material.Rock;
        }

        if (terrain.GetValueOrDefault(sand.Offset(down), Material.Air) == Material.Air)
            sand = sand.Offset(down);
        else if (terrain.GetValueOrDefault(sand.Offset(downLeft), Material.Air) == Material.Air)
            sand = sand.Offset(downLeft);
        else if (terrain.GetValueOrDefault(sand.Offset(downRight), Material.Air) == Material.Air)
            sand = sand.Offset(downRight);
        else
        {
            terrain[sand] = Material.Sand;
            if (sand == start)
                break;
            sand = start;
        }
    }

    return ImmutableDictionary.CreateRange (terrain);
}

static ImmutableDictionary<Point, Material> Setup(string[] initialGeometry)
{
    var data = new Dictionary<Point, Material>
    {
        { new Point(500, 0), Material.Source }
    };

    foreach (var line in initialGeometry)
    {
        var parts = line.Split(" -> ")
            .Select (Point.Parse)
            .ToArray ()
            .AsSpan();
        
        while (parts.Length > 1)
        {
            if (parts[0].X == parts[1].X)
            {
                for (int i = Math.Min(parts[0].Y, parts[1].Y); i <= Math.Max(parts[0].Y, parts[1].Y); i++)
                    data[new Point(parts[0].X, i)] = Material.Rock;
            } else if (parts[0].Y == parts[1].Y)
            {
                for (int i = Math.Min(parts[0].X, parts[1].X); i <= Math.Max(parts[0].X, parts[1].X); i++)
                    data[new Point(i, parts[0].Y)] = Material.Rock;
            } else
            {
                throw new NotSupportedException();
            }
            parts = parts[1..];
        }
    }

    return ImmutableDictionary.CreateRange(data);
}

static void Print (ImmutableDictionary<Point, Material> terrain)
{
    var xLocations = terrain.Where(t => t.Value == Material.Sand).Select(t => t.Key.X);
    var yLocation = terrain.Where(t => t.Value == Material.Sand).Select(t => t.Key.Y);
    var start = new Point(xLocations.Min () - 1, 0);
    var end = new Point(xLocations.Max () + 1, yLocation.Max () + 1);

    for (int j = start.Y; j <= end.Y; j++)
    {
        Console.WriteLine();
        for (int i = start.X; i <= end.X; i++)
            Console.Write((char)terrain.GetValueOrDefault(new Point(i, j), Material.Air));
    }
    Console.WriteLine();
}

enum Material
{
    Source = '+',
    Rock = '#',
    Sand = 'o',
    Air = '.'
}

readonly record struct Point(int X, int Y)
{
    public static Point Parse(string point)
        => Parse(point.AsSpan());

    public static Point Parse(ReadOnlySpan<char> point)
        => new (int.Parse(point.Slice(0, point.IndexOf(','))), int.Parse(point.Slice(point.IndexOf(',') + 1)));

    public Point Offset(int x, int y)
        => new(X + x, Y + y);

    public Point Offset(Point offset)
        => new(X + offset.X, Y + offset.Y);
}