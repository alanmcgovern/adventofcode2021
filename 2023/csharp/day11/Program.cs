var starMap = ParseMap(File.ReadAllLines("input.txt"));


Console.WriteLine($"Q1: {MegaDistance(starMap, 2)}");
Console.WriteLine($"Q2: {MegaDistance(starMap, 1000000)}");


static IList<IList<SpaceType>> ParseMap(string[] input)
    => input.Select((row, Index) => (IList<SpaceType>)row.Select(t => (SpaceType)t).ToList().AsReadOnly()).ToList().AsReadOnly();

static (int[] expansionRows, int[] expansionColumns) FindExpansionPoints(IList<IList<SpaceType>> map)
{
    var expansionRows = Enumerable.Range(0, map.Count).Where(row => map[row].All(t => t == SpaceType.Empty)).ToArray();
    var expansionCols = Enumerable.Range(0, map[0].Count).Where(col => Enumerable.Range(0, map.Count).All(row => map[row][col] == SpaceType.Empty)).ToArray();
    return (expansionRows, expansionCols);
}

static long MegaDistance(IList<IList<SpaceType>> starMap, int expansionSize)
{
    Func<int, int, int, bool> Between = (int min, int max, int value)
        => (value >= min && value <= max) || (value >= max && value <= min);

    long totalDistance = 0;
    (int[] expansionRows, int[] expansionColumns) = FindExpansionPoints(starMap);
    foreach (((int row, int col) firstStar, (int row, int col) secondStar) in AllStarPairs(starMap))
    {
        totalDistance += Math.Abs(firstStar.row - secondStar.row) + Math.Abs(firstStar.col - secondStar.col);
        var totalExpansionRows = expansionRows.Count(row => Between(firstStar.row, secondStar.row, row));
        var totalExpansionCols = expansionColumns.Count(col => Between(firstStar.col, secondStar.col, col));
        totalDistance += (totalExpansionRows + totalExpansionCols) * expansionSize - (totalExpansionRows + totalExpansionCols);
    }
    return totalDistance;
}

static IList<((int, int) firstStar, (int, int) secondStar)> AllStarPairs(IList<IList<SpaceType>> map)
{
    var starLocations = new List<(int row, int col)>();
    for (int row = 0; row < map.Count; row++)
        for (int col = 0; col < map[row].Count; col++)
            if (map[row][col] == SpaceType.Galaxy)
                starLocations.Add((row, col));

    return starLocations.SelectMany(t =>
    {
        return starLocations.Select(u =>
        {
            if (u.row != t.row)
                return u.row < t.row ? (u, t) : (t, u);
            return u.col < t.col ? (u, t) : (t, u);
        });
    })
        .Where(t => t.Item1 != t.Item2)
        .ToHashSet()
        .ToArray();
}

enum SpaceType
{
    Empty = '.',
    Galaxy = '#',
}