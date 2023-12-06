var games = File.ReadAllLines("input.txt")
    .Select(ParseGame)
    .ToArray()
    .AsReadOnly ();

Console.WriteLine($"Q1 {games.Select(g => g.Matches == 0 ? 0 : (int)Math.Pow(2, g.Matches - 1)).Sum()}");
Console.WriteLine($"Q2 {ScoreGame2(games)}");

static int ScoreGame2(IList<Card> games)
{
    var totalCards = 0;
    var remaining = new Queue<Card>(games);
    while(remaining.Count > 0)
    {
        var current = remaining.Dequeue();
        totalCards++;

        var score = current.Matches;
        for (int i = current.Id; score > 0 && i < games.Count; i++, score--)
        {
            remaining.Enqueue(games[i]);
        }
    }

    return totalCards;
}

static Card ParseGame(string game)
{
    var parts = game.Split(':', StringSplitOptions.RemoveEmptyEntries);
    var numbers = parts[1].Split('|', StringSplitOptions.RemoveEmptyEntries);
    return new Card (
        id: int.Parse(parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]),
        winningNumbers: numbers[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray().AsReadOnly(),
        myNumbers: numbers[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray().AsReadOnly()
    );
}

class Card
{
    public int Id { get; }
    public IList<int> WinningNumbers { get; }
    public IList<int> MyNumbers { get; }
    public int Matches { get; }

    public Card(int id, IList<int> winningNumbers, IList<int> myNumbers)
    {
        Id = id;
        WinningNumbers = winningNumbers;
        MyNumbers = myNumbers;

        Matches = myNumbers.Intersect(winningNumbers).Count();
    }
}
