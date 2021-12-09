var input = File.ReadLines("input.txt")
    .Select(t => t.ToArray().Select(c => (int)(c - '0')).ToArray())
    .ToArray();

int risk = 0;
for (int i = 0; i < input.Length ; i++)
{
    var line = input[i];
    for (int j = 0; j < line.Length; j ++)
    {
        var leftOk = j == 0 || line[j - 1] > line[j];
        var rightOk = j == line.Length - 1 || line[j + 1] > line[j];
        var topOk = i == 0 || input[i - 1][j] > line[j];
        var bottomOk = i == input.Length - 1 || input[i + 1][j] > line[j];

        if (leftOk && rightOk && topOk && bottomOk)
            risk += 1 + input[i][j];
    }
}
Console.WriteLine($"Risk: {risk}");

// q2
var painted = input
    .Select(t => new object[t.Length])
    .ToArray();

// Fill in the breakers between basins first.
object Breaker = new object();
for (int x = 0; x < input.Length; x++)
    for (int y = 0; y < input[x].Length; y++)
        if (input[x][y] == 9)
            painted[x][y] = Breaker;

// Paint the interiors of each basin
for (int x = 0; x < painted.Length; x++)
    for (int y = 0; y < painted[x].Length; y++)
        if (painted[x][y] == null)
            TryFill(painted, new object (), x, y);

// Count how many times each painter is in the graph
var result = painted.SelectMany(t => t)
    .Where(t => t != Breaker)
    .GroupBy(t => t)
    .Select(t => t.Count())
    .OrderByDescending(t => t)
    .Take(3)
    .Aggregate((a, b) => a * b);

Console.WriteLine($"Largest 3 basins: {result}");

static void FillNeighbours(object[][] painted, object painter, int x, int y)
{
    TryFill(painted, painter, x - 1, y);
    TryFill(painted, painter, x + 1, y);
    TryFill(painted, painter, x, y + 1);
    TryFill(painted, painter, x, y - 1);
}

static void TryFill(object[][] painted, object painter, int x, int y)
{
    if (x >= 0 && x < painted.Length &&
        y >= 0 && y < painted[x].Length &&
        (painted[x][y] == null))
    {
        painted[x][y] = painter;
        FillNeighbours(painted, painter, x, y);
    }
}
