
var sum = File.ReadAllLines("input.txt")
    .Select(int.Parse)
    .Pairwise(2)
    .Sum(tuple => tuple[0] < tuple[1] ? 1 : 0);
Console.WriteLine($"Answer Challenge 1: {sum}");

sum = File.ReadAllLines("input.txt")
    .Select(int.Parse)
    .Pairwise(3)
    .Select(triplet => triplet.Sum ())
    .Pairwise(2)
    .Sum(pair => pair[0] < pair[1] ? 1 : 0);
Console.WriteLine($"Answer Challenge 2: {sum}");

public static class Extensions
{
    public static IEnumerable<T?[]> Pairwise<T>(this IEnumerable<T?> input, int n)
    {
        var values = new List<T?>(n);

        foreach (var v in input)
        {
            values.Add(v);
            if (values.Count == n)
            {
                yield return values.ToArray();
                values.RemoveAt(0);
            }
        }
    }
}
