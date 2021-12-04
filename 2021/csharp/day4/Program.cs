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

int winner;
List<(Board board, int number)> winners = new List<(Board board, int number)>();

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

    while ((winner = boards.FindIndex (t => t.Rows.Any(t => t.Count == 0) || t.Columns.Any(t => t.Count == 0))) != -1) {
        winners.Add ((boards[winner], draw));
        boards.RemoveAt(winner);
    }
}
Console.WriteLine($"First winning board: {winners.First ().board.Rows.Select (t => t.Sum ()).Sum () * winners.First ().number}");
Console.WriteLine($"Last winning board: {winners.Last ().board.Rows.Select(t => t.Sum()).Sum() * winners.Last ().number}");


struct Board
{
    public List<HashSet<int>> Columns;
    public List<HashSet<int>> Rows;
}