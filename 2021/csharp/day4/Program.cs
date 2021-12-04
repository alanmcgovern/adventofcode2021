var input = File.ReadAllLines("input.txt");

var current = (Board?)null;
var boards = new List<Board>();
foreach (var item in input.Skip (1))
{
    if (string.IsNullOrEmpty(item))
    {
        if (current.HasValue)
        {
            boards.Add(current.Value);
            current = null;
        }
    }
    else
    {
        var numbers = item.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        if (!current.HasValue)
        {
            current = new Board
            {
                Columns = new List<HashSet<int>>(),
                Rows = new List<HashSet<int>>()
            };
            for (int i = 0; i < numbers.Length; i++)
                current.Value.Columns.Add(new HashSet<int>());
        }
        current.Value.Rows.Add(numbers.ToHashSet());
        for (int i = 0; i < numbers.Length; i++)
            current.Value.Columns[i].Add(numbers[i]);
    }
}

int winningboard = -1;
foreach (var draw in input.First().Split(',').Select(int.Parse))
{
    foreach (var board in boards)
    {
        foreach (var row in board.Rows)
        {
            board.Rows.ForEach(t => t.Remove(draw));
            board.Columns.ForEach(t => t.Remove(draw));
        }
    }

    winningboard = boards.FindIndex (t => t.Rows.Any(t => t.Count == 0) || t.Columns.Any(t => t.Count == 0));
    if (winningboard != -1)
    {
        Console.WriteLine($"Winning score: {draw * boards[winningboard].Rows.Select(t => t.Sum()).Sum()}");
        break;
    }
}


struct Board
{
    public List<HashSet<int>> Columns;
    public List<HashSet<int>> Rows;
}