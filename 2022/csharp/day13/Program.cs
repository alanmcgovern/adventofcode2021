
using System.Text.Json;

var packets = File.ReadAllLines("input.txt")
    .Where (t => !string.IsNullOrEmpty (t))
    .Select(Data.Parse)
    .ToArray();

// Sum the indices of the incorrect pairs.
var sumIndicesIncorrect = 0;
for (int i = 0; i < packets.Length; i += 2)
    if (packets[i].CompareTo(packets[i + 1]) <= 0)
        sumIndicesIncorrect += 1 + (i / 2);
Console.WriteLine($"Q1: {sumIndicesIncorrect}");

// Add 2 packets and then find their indices afterwards and divide them.
var packet2 = new NumberList(new[] { new Number(2) });
var packet6 = new NumberList(new[] { new Number(6) });

var sortedPackets = packets.Concat(new[] { packet2, packet6 })
    .Order()
    .ToList();
Console.Write($"Q2: {(1 + sortedPackets.IndexOf(packet2)) * (1 + sortedPackets.IndexOf(packet6))} ");

abstract record Data : IComparable<Data>
{
    public static Data Parse(string data)
        => FromJson(JsonSerializer.Deserialize<JsonElement>(data));

    public static Data FromJson(JsonElement element)
        => element.ValueKind switch
        {
            JsonValueKind.Array => new NumberList(element.EnumerateArray().Select(FromJson).ToArray()),
            JsonValueKind.Number => new Number(element.GetInt32()),
            _ => throw new NotSupportedException()
        };

    public int CompareTo(Data? other)
    {
        if (other is null)
            return 1;

        var leftNum = this as Number;
        var rightNum = other as Number;

        if (leftNum is not null && rightNum is not null)
            return leftNum.Value.CompareTo(rightNum.Value);

        var leftList = (this as NumberList ?? new NumberList(new[] { leftNum! })).Values.Span;
        var rightList = (other as NumberList ?? new NumberList(new[] { rightNum! })).Values.Span;

        while (true)
        {
            // If left is empty and right is not empty, then these are correctly sorted. Return -1 to show
            // this item is OK. If they're both empty, return 0 to show they're equivalent.
            if (leftList.Length == 0)
                return rightList.Length == 0 ? 0 : -1;

            // If right is empty and left *is not* empty, then these are 'improperly' sorted so return 1.
            if (rightList.Length == 0)
                return 1;

            var result = leftList[0].CompareTo(rightList[0]);
            if (result == 0)
            {
                // If these are the same, slice and continue checking the next entry
                leftList = leftList[1..];
                rightList = rightList[1..];
            }
            else
            {
                // Otherwise we have an answer!
                return result;
            }
        }
    }
}

record Number(int Value) : Data;
record NumberList(ReadOnlyMemory<Data> Values) : Data;
