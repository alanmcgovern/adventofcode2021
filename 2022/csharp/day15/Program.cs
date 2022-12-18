
using System.Collections.Immutable;
using System.Text.RegularExpressions;

int preferredY = 2000000;
var sensorBeaconPairs = Parse (File.ReadAllLines("input.txt"));
var populatedCaves = PopulateAreas(sensorBeaconPairs, preferredY);
//Print(populatedCaves);
Console.WriteLine($"Q1: {populatedCaves.Where(t => t.Key.Y == preferredY).Where(t => t.Value == Material.NoBeacon || t.Value == Material.Sensor).Count()}");

static ReadOnlyMemory<(Point sensor, Point beacon)> Parse(string[] rawData)
{
    var regex = new Regex("x=(-*\\d+), y=(-*\\d+)", RegexOptions.Compiled);

    var points = new List<(Point sensor, Point beacon)>();
    foreach (var pair in rawData)
    {
        var matches = regex.Matches(pair);
        points.Add((new Point(int.Parse(matches[0].Groups[1].Value), int.Parse(matches[0].Groups[2].Value)),
                    new Point(int.Parse(matches[1].Groups[1].Value), int.Parse(matches[1].Groups[2].Value))));
    }

    return points.ToArray ();
}

static ImmutableDictionary<Point, Material> PopulateAreas(ReadOnlyMemory<(Point sensor, Point beacon)> sensorPositions, int yPosition)
{
    var caves = new Dictionary<Point, Material>();
    foreach ((var sensor, var beacon) in sensorPositions.Span)
    {
        Console.WriteLine($"handling: {sensor} -> {beacon}");
        foreach (var point in ManhattenDistancePoints(sensor, beacon, yPosition))
            caves[point] = Material.NoBeacon;
        caves[beacon] = Material.Beacon;
        caves[sensor] = Material.Sensor;
    }

    return ImmutableDictionary.CreateRange(caves);
}

static IEnumerable<Point> ManhattenDistancePoints (Point center, Point edge, int yPosition)
{
    var maxDistance = Math.Abs(center.X - edge.X) + Math.Abs(center.Y - edge.Y);
    // walk in from the left, then walk out to the right.

    for (int xOffset = -maxDistance; xOffset <= maxDistance; xOffset++)
    {
        var minY = -(maxDistance - Math.Abs(xOffset)) + center.Y;
        var maxY = (maxDistance - Math.Abs(xOffset)) + center.Y;

        if (yPosition >= minY && yPosition <= maxY)
            yield return new Point(xOffset + center.X, yPosition);
    }
}

static void Print (ImmutableDictionary<Point, Material> caves)
{
    // *lol*, this is pointless on the actual data.
    var minY = caves.Keys.Select(p => p.Y).Min();
    var maxY = caves.Keys.Select(p => p.Y).Max();

    var minX = caves.Keys.Select(p => p.X).Min();
    var maxX = caves.Keys.Select(p => p.X).Max();

    for (int y = minY - 1; y < maxY + 1; y++)
    {
        Console.WriteLine();
        Console.Write(y.ToString().PadRight(5));
        for (int x = minX - 1; x < maxX + 1; x++)
            Console.Write((char)caves.GetValueOrDefault(new Point(x, y), Material.Unknown));
    }
    Console.WriteLine();
}

enum Material
{
    Beacon = 'B',
    Sensor = 'S',
    NoBeacon = '#',
    Unknown = '.'
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