var races = ParseRaces(File.ReadAllLines("input.txt"));

Console.WriteLine($"Q1: {races.Select(WinningWays).Aggregate((a, b) => a * b)}");

int WinningWays(Race race)
{
    int a = 1, b = -race.Time, c = race.MinDistance;
    var plusminus = Math.Sqrt((b * b) - 4 * a * c);

    var one = (-b + plusminus) / (2 * a);
    var other = (-b - plusminus) / (2 * a);

    var min = (int)Math.Ceiling (Math.Min(one, other));
    var max = (int)Math.Floor (Math.Max(one, other));

    return max - min + 1;
}

static IList<Race> ParseRaces(string[] lines)
{
    //Time: 48     87     69     81
    //Distance: 255   1288   1117   1623
    var races = new List<Race>();
    var time = lines.Single(t => t.StartsWith("Time")).Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
    var distance = lines.Single(t => t.StartsWith("Distance")).Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
    for (int i = 0; i < time.Length; i++)
        races.Add(new Race { Time = time[i], MinDistance = distance[i] + 1 });
    return races;
}

class Race
{
    public int Time;
    public int MinDistance;
}