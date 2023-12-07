var races = ParseRaces(File.ReadAllLines("input.txt"));

Console.WriteLine($"Q1: {races.Select(WinningWays).Aggregate((a, b) => a * b)}");

var combinedRace = new Race {
    MinDistance = 1 + long.Parse (new string(races.SelectMany (t => (t.MinDistance - 1).ToString()).ToArray ())),
    Time = long.Parse (new string(races.SelectMany (t => t.Time.ToString()).ToArray ())),
};

Console.WriteLine($"Q2: {WinningWays(combinedRace)}");


long WinningWays(Race race)
{
    long a = 1, b = -race.Time, c = race.MinDistance;
    var plusminus = Math.Sqrt((b * b) - 4 * a * c);

    var one = (-b + plusminus) / (2 * a);
    var other = (-b - plusminus) / (2 * a);

    var min = (long)Math.Ceiling (Math.Min(one, other));
    var max = (long)Math.Floor (Math.Max(one, other));

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
    public long Time;
    public long MinDistance;
}