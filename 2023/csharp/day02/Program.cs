
using System.Security.Cryptography.X509Certificates;

var games = File.ReadAllLines("input.txt")
    .Select(ParseGame)
    .ToArray();

Console.WriteLine($"Q1: {games.Where(game => game.rounds.All(round => round.red <= 12 && round.green <= 13 && round.blue <= 14)).Sum(g => g.id)}");

Console.WriteLine($"Q2: {games.Select(game => game.rounds.Max (t => t.red) * game.rounds.Max(t => t.green) * game.rounds.Max(t => t.blue)).Sum ()}");

static Game ParseGame(string game)
{
    var parts = game.Split(':');
    return new Game
    {
        id = int.Parse(parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]),
        rounds = parts[1].Split(';').Select(ParseRound).ToArray()
    };
}

static Round ParseRound(string round)
{
    var result = new Round();
    foreach (var part in round.Split(',', StringSplitOptions.RemoveEmptyEntries))
    {
        var numberColor = part.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        switch (numberColor[1])
        {
            case "green":
                result.green = int.Parse(numberColor[0]);
                break;
            case "red":
                result.red = int.Parse(numberColor[0]);
                break;
            case "blue":
                result.blue = int.Parse(numberColor[0]);
                break;
            default:
                throw new InvalidDataException();
        }
    }
    return result;
}


struct Game
{
    public int id;
    public Round[] rounds;
}

struct Round
{
    public int red, green, blue;
};
