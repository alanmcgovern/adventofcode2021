var records = File.ReadAllLines("input.txt").Select(ParseRecord).ToArray ();

Console.WriteLine($"Q1: {records.Select(CountVariations).Sum()}");
Console.Write(records);

Record ParseRecord(string line)
{
    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    return new Record(
        parts[0].Select(t => (SpringState)t).ToArray(),
        parts[1].Split (',', StringSplitOptions.RemoveEmptyEntries).Select(t => int.Parse(t.ToString ())).ToArray()
    );
}

static long CountVariations(Record record)
{
    var potential = record.States.Span;
    return 0;
}

class Record(ReadOnlyMemory<SpringState> states, ReadOnlyMemory<int> parityCounts)
{
    public ReadOnlyMemory<SpringState> States { get; } = states;
    ReadOnlyMemory<int> ParityCounts { get; } = parityCounts;
}

enum SpringState
{
    operational = '.',
    damaged = '#',
    unknown = '?'
}
