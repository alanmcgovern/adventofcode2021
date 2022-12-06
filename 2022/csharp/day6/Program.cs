
var input = File.ReadAllText("input.txt").AsMemory ();

Console.WriteLine($"Q1: {ConsumeCharacters(4)}");
Console.WriteLine($"Q1: {ConsumeCharacters(14)}");

int ConsumeCharacters(int messageHeaderLength)
{
    return Enumerable.Range(0, input.Length)
        .SkipLast(messageHeaderLength - 1)
        .Select(t => input.Slice(t, messageHeaderLength))
        .Select(AllDifferent)
        .TakeWhile(t => t == false)
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