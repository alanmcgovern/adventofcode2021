
var input = File.ReadAllText("input.txt").AsMemory ();

Console.WriteLine($"Q1: {ConsumeCharacters(4)}");
Console.WriteLine($"Q1: {ConsumeCharacters(14)}");

int ConsumeCharacters(int messageHeaderLength)
{
    return input
        .Window(messageHeaderLength)
        .TakeWhile(t => !AllDifferent(t))
        .Count() + messageHeaderLength;
}

bool AllDifferent(ReadOnlyMemory<char> memory)
{
    var span = memory.Span;
    while(span.Length > 1)
    {
        if (span.Slice(1).Contains(span[0]))
            return false;
        span = span.Slice(1);
    }
    return true;
}

static class Extensions
{
    public static IEnumerable<ReadOnlyMemory<T>> Window<T>(this ReadOnlyMemory<T> self, int windowLength)
    {
        for (int i = 0; i + windowLength < self.Length; i++)
            yield return self.Slice(i, windowLength);
    }
}