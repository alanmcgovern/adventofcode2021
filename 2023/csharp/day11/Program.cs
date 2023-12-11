var starMap = ParseMap(File.ReadAllLines("input.txt"));
(var expandedStarMap, int[] expansionRows, int[] expansionColumns) = ExpandMap(starMap);

IList<((int row, int col) firstStar, (int row, int col) secondStar)> starTuples = AllStarPairs(expandedStarMap);

Console.WriteLine($"Q1: {starTuples.Select(pair => Math.Abs(pair.firstStar.row - pair.secondStar.row) + Math.Abs(pair.firstStar.col - pair.secondStar.col)).Sum ()}");
Console.WriteLine($"Q2: {MegaDistance(starMap, expansionRows, expansionColumns)}");

static long MegaDistance(IList<IList<SpaceType>> starMap, int[] expansionRows, int[] expansionColumns)
{
    static bool Between (int min, int max, int value)
    {
        if (min < max)
            return value >= min && value <= max;
        return value >= max && value <= min;
    }

    var expansionSize = 1000000;
    long totalDistance = 0;
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

static (IList<IList<SpaceType>> expandedMap, int[] expansionRows, int[] expansionColumns) ExpandMap(IList<IList<SpaceType>> original)
{
    var expansionRows = Enumerable.Range(0, original.Count).Where(row => original[row].All(t => t == SpaceType.Empty)).ToArray();
    var expansionCols = Enumerable.Range(0, original[0].Count).Where(col => Enumerable.Range (0, original.Count).All (row => original[row][col] == SpaceType.Empty)).ToArray ();

    var expandedMap = new IList<SpaceType>[original.Count + expansionRows.Length];
    for (int i = 0; i < expandedMap.Length; i++)
        expandedMap[i] = new SpaceType[original[0].Count + expansionCols.Length];

    // Iterate through the original map
    int row = 0;
    int expandedRow = 0;
    while (row < original.Count)
    {
        if (expansionRows.Contains(row)) {
            for (int i = 0; i < expandedMap[row].Count; i++)
            {
                expandedMap[expandedRow][i] = SpaceType.Empty;
                expandedMap[expandedRow + 1][i] = SpaceType.Empty;
            }
            expandedRow++;
        } else {
            int col = 0, expandedCol = 0;
            while (col < original[row].Count)
            {
                expandedMap[expandedRow][expandedCol] = original[row][col];
                if (expansionCols.Contains(col))
                {
                    expandedCol++;
                    expandedMap[expandedRow][expandedCol] = SpaceType.Empty;
                }
                expandedCol++;
                col++;
            }
        }
        expandedRow++;
        row++;
    }

    return (expandedMap, expansionRows, expansionCols);
}

static IList<IList<SpaceType>> ParseMap(string[] input)
    => input.Select((row, Index) => (IList<SpaceType>)row.Select(t => (SpaceType)t).ToList().AsReadOnly()).ToList().AsReadOnly();

enum SpaceType
{
    Empty = '.',
    Galaxy = '#',
}