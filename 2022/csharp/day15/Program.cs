
using System.Collections.Immutable;
using System.Numerics;
using System.Text.RegularExpressions;

var sensorBeaconPairs = Parse(File.ReadAllLines("input.txt"));

int preferredY = 2000000;
ImmutableDictionary<Point, Material> populatedCaves = PopulateAreas(sensorBeaconPairs, preferredY);
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

const int minXY = 0;
const int maxXY = 4000000;
Console.WriteLine($"Q2: {FindDistressBeacon(sensorBeaconPairs, new Point(minXY, minXY), new Point(maxXY, maxXY))}");

static string FindDistressBeacon(ReadOnlyMemory<(Point sensor, Point beacon)> sensorBeaconPairs, Point min, Point max)
{
    // for each circle, look 1 position beyond the edge and check that it's further away than the beacon for each sensor/beacon pair.
    // .... right?
    Span<Point> points = stackalloc Point[2];

    for (int i = 0; i < sensorBeaconPairs.Length; i++)
    {
        (var sensor, var beacon) = sensorBeaconPairs.Span[i];
        var distance = ManhattanDistance(sensor, beacon) + 1;
        for (int offset = -distance; offset <= distance; offset++)
        {
            points[0] = sensor.Offset(offset, distance - Math.Abs(offset));
            points[1] = sensor.Offset(offset, -(distance - Math.Abs(offset)));
            foreach (ref var point in points)
            {
                if (point.X < minXY || point.X > maxXY)
                    continue;
                if (point.Y < minXY || point.Y > maxXY)
                    continue;
                bool foundIt = true;
                foreach (var pair in sensorBeaconPairs.Span)
                {
                    if (ManhattanDistance(pair.sensor, pair.beacon) >= ManhattanDistance(pair.sensor, point))
                    {
                        foundIt = false;
                        break;
                    }
                }
                if (foundIt)
                    return (new BigInteger(point.X) * maxXY + point.Y).ToString ();
            }
        }
    }
    throw new NotSupportedException();
}

static int ManhattanDistance(Point center, Point other)
    => Math.Abs(center.X - other.X) + Math.Abs(center.Y - other.Y);

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