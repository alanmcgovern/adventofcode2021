

Console.WriteLine($"Q1: {CountParts()}");

static char[][] GetGrid(bool expand)
{
    var result = File.ReadAllLines("input.txt")
    .Select(t => ((expand ? "." : "") + t + (expand ? "." : "")).ToArray())
    .ToArray();

    if (expand)
        result = Enumerable.Concat(Enumerable.Concat(new[] { new string('.', result[0].Length).ToArray() }, result).ToArray(), new[] { new string('.', result[0].Length).ToArray() }).ToArray();
    return result;
}

int CountParts()
{
    var grid = GetGrid(true);
    int count = 0;
    for (int row = 1; row < grid.Length - 2; row++)
    {
        for (int col = 1; col < grid[row].Length - 2; col++)
        {
            ref char cell = ref grid[row][col];
            if (char.IsDigit(cell) || cell == '.')
                continue;

            foreach (var rowOffset in new[] { -1, 0, 1 })
            {
                foreach (var colOffset in new[] { -1, 0, 1 })
                    count += MaybeParse(grid, row + rowOffset, col + colOffset);
            }
        }
    }
    return count;
}

int MaybeParse(char[][] grid, int row, int col)
{
    var leftOffset = col;
    var rightOffset = col;
    // Are we in a number?
    if (!char.IsDigit(grid[row][leftOffset]))
        return 0;

    // Get leftmost digit
    while (char.IsDigit(grid[row][leftOffset - 1]))
        leftOffset--;

    // get rightmost digit?
    while (char.IsDigit(grid[row][rightOffset + 1]))
        rightOffset++;

    var span = grid[row].AsSpan(leftOffset, rightOffset - leftOffset + 1);
    var result = int.Parse(span);
    span.Fill('.');
    return result;
}