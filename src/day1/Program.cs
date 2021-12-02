
var sum = File.ReadAllLines("input.txt")
    .Select(int.Parse)
    .Pairwise()
    .Select(pair => pair.Left < pair.Right ? 1 : 0)
    .Sum();
Console.WriteLine($"Answer: {sum}");

public static class Extensions
{ 
    public static IEnumerable<(T? Left, T? Right)> Pairwise<T> (this IEnumerable<T?> input)
    {
        bool hasFirst = false;
        T? first;
        T? second = default;

        foreach (var v in input)
        {
            first = second;
            second = v;
            if (hasFirst)
                yield return (first, second);
            hasFirst = true;
        }
    }
}
