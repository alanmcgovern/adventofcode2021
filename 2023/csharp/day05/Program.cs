(var seeds, var maps) = ParseMaps(File.ReadAllLines("input.txt"));

Console.WriteLine($"Q1: {seeds.Select(seed => GetLocation(seed, maps)).Min()}");
Console.WriteLine($"Q2: {SeedLocationsByRange(seeds, maps).Min()}");

static IList<long> SeedLocationsByRange(IList<long> seedPairs, IList<Map> maps)
{
    var pairs = new List<Task<long>>();
    for (int i = 0; i < seedPairs.Count; i += 2)
    {
        var seed = seedPairs[i];
        var length = seedPairs[i + 1];
        pairs.Add(Task.Run(() =>
        {
            var smallestSoFar = long.MaxValue;
            for (long j = seed; length > 0; j++, length--)
            {
                var maybe = GetLocation(j, maps);
                if (maybe < smallestSoFar)
                    smallestSoFar = maybe;
            }
            return smallestSoFar;
        }));
    }

    return pairs.Select(t => t.Result).ToArray();
}

static long GetLocation(long seed, IList<Map> maps)
{
    long value = seed;
    var map = maps.Single(t => t.SourceType == "seed");
    while (map.DestinationType != "location")
    {
        value = map.Convert(value);
        map = maps.Single(t => t.SourceType == map.DestinationType);
    }
    return map.Convert(value);
}

static (IList<long>, IList<Map>) ParseMaps(string[] text)
{
    var seeds = new List<long>();
    var maps = new List<Map>();

    (string sourceType, string destType, List<Range> ranges)? currentMap = null;
    foreach (var line in text)
    {
        if (line.StartsWith("seeds"))
        {
            seeds.AddRange(line.Split(":", StringSplitOptions.RemoveEmptyEntries)[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse));
        }
        else if (line.Length == 0)
        {
            if (currentMap.HasValue)
                maps.Add(new Map(currentMap.Value.sourceType, currentMap.Value.destType, currentMap.Value.ranges));
            currentMap = null;
        }
        else if (line.Contains("map"))
        {
            var parts = line.Split(" map")[0].Split("-");
            currentMap = (parts[0], parts[2], new List<Range>());
        }
        else
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
            currentMap!.Value.ranges.Add(new Range(parts[0], parts[1], parts[2]));
        }
    }

    if (currentMap != null)
        maps.Add(new Map(currentMap.Value.sourceType, currentMap.Value.destType, currentMap.Value.ranges));
    return (seeds.ToArray().AsReadOnly(), maps.ToArray().AsReadOnly());
}

class Map
{
    public string SourceType { get; }
    public string DestinationType { get; }
    ReadOnlyMemory<Range> Ranges { get; }

    public Map(string sourceType, string destinationType, IEnumerable<Range> ranges)
        => (SourceType, DestinationType, Ranges) = (sourceType, destinationType, ranges.OrderBy(t => t.SourceStart).ToArray());

    public long Convert(long source)
    {
        foreach (var v in Ranges.Span)
            if (source >= v.SourceStart && source < (v.SourceStart + v.Length))
                return v.DestStart + source - v.SourceStart;
        return source;
    }
}

struct Range
{
    public long SourceStart { get; }
    public long DestStart { get; }
    public long Length { get; }

    public Range(long destStart, long sourceStart, long length)
        => (SourceStart, DestStart, Length) = (sourceStart, destStart, length);
}