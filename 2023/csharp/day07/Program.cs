
var hands = File.ReadAllLines("input.txt");


Console.WriteLine($"Q1: {ParseHands(hands, false).Select((hand, index) => hand.Bid * (index + 1)).Sum()}");
Console.WriteLine ($"Q2: {ParseHands(hands, true).Select((hand, index) => hand.Bid * (index + 1)).Sum()}");

static IEnumerable<Hand> ParseHands (string[] hands, bool wildCard)
{
    var result = hands.Select(t => ParseHand(t, wildCard)).ToList();
    result.Sort((left, right) => {
        var delta = left.Strength.CompareTo(right.Strength);
        if (delta != 0)
            return delta;
        return StringComparer.Ordinal.Compare(left.OrderKey, right.OrderKey);
    });
    return result;
}

static Hand ParseHand(string handAndBid, bool wildCard)
{
    var mapping = wildCard ? "J23456789TQKA" : "23456789TJQKA";

    var hand = handAndBid.Split(' ')[0];
    var jokers = wildCard ? hand.Count(h => h == 'J') : 0;
    var buckets = new HashSet<char>[]
    {
        mapping.Where(t => hand.Count(h => h == t) == 1).ToHashSet(),
        mapping.Where(t => hand.Count(h => h == t) == 2).ToHashSet(),
        mapping.Where(t => hand.Count(h => h == t) == 3).ToHashSet(),
        mapping.Where(t => hand.Count(h => h == t) == 4).ToHashSet(),
        mapping.Where(t => hand.Count(h => h == t) == 5).ToHashSet()
    };

    if (jokers > 0 && jokers < 5)
    {
        buckets[jokers - 1].Remove('J');
        var toRemove = Array.FindLastIndex(buckets, b => b.Count > 0);
        buckets[toRemove + jokers].Add(buckets[toRemove].First());
        buckets[toRemove].Remove(buckets[toRemove].First());
    }

    int strength;
    if (buckets[4].Count > 0)
        strength = 6;
    else if (buckets[3].Count > 0)
        strength = 5;
    else if (buckets[2].Count == 1 && buckets[1].Count == 1)
        strength = 4;
    else if (buckets[2].Count == 1)
        strength = 3;
    else if (buckets[1].Count == 2)
        strength = 2;
    else if(buckets[1].Count == 1)
        strength = 1;
    else
        strength = 0;

    var orderingKey = hand;
    for (int i = 0; i < mapping.Length; i++)
        orderingKey = orderingKey.Replace(mapping[i].ToString(), ((char)i).ToString());

    int bid = int.Parse(handAndBid.Split(' ')[1]);
    return new Hand(orderKey: orderingKey, bid: bid, strength: strength);
}

class Hand(string orderKey, int strength, long bid)
{
    public string OrderKey = orderKey;
    public int Strength = strength;
    public long Bid = bid;
}