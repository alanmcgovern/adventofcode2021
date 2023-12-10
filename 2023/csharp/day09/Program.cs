var input = File.ReadAllLines("input.txt");

Console.WriteLine($"Q1: {Parse(input, reverse: false).Select(GrowSequence).Select(t => t[0]).Select(t => t.Last()).Sum()}");
Console.WriteLine ($"Q2: {Parse(input, reverse: true).Select(GrowSequence).Select(t => t[0]).Select(t => t.Last()).Sum()}");

static List<List<long>> GrowSequence(List<List<long>> sequence)
{
    while (!sequence.Last().All(t => t == 0))
    {
        var row = sequence.Last();
        var nextRow = new List<long>(row.Count - 1);
        for (int i = 1; i < row.Count; i++)
            nextRow.Add(row[i] - row[i - 1]);
        sequence.Add(nextRow);
    }

    for (int i = sequence.Count - 2; i >= 0; i--)
    {
        var previous = sequence[i + 1];
        var current = sequence[i];
        current.Add(current.Last () + previous.Last());
    }
    return sequence;
}

static List<List<long>>[] Parse(string[] input, bool reverse)
{
    return input.Select(line =>
    {
        var data = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
        if (reverse)
            data.Reverse();
        return new List<List<long>>
        {
            data
        };
    }).ToArray();
}
