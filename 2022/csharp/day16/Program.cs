
using System.Collections.Immutable;
using System.Text.RegularExpressions;

(var valves, var distances) = Parse(File.ReadAllLines("input.txt"));

Console.WriteLine($"Q1: {MaximumPressureRelease(valves, distances, 30)}");
Console.WriteLine($"Q2: {Q2MaximumPressureRelease(valves, distances, 26)}");

// Q1
int MaximumPressureRelease(ImmutableArray<Valve> valves, ImmutableDictionary<(Valve, Valve), int> distances, int remainingActions)
{
    var startValve = valves.Single(t => t.Name == "AA");
    var openableValves = valves.Where(t => t.FlowRate > 0).ToImmutableArray();

    if (startValve.FlowRate != 0)
        throw new NotSupportedException();

    int maxPressureRelief = 0;
    for (int i = 0; i < openableValves.Length; i++)
    {
        var currentValve = openableValves[i];
        MaximumPressureReleaseRecurse(currentValve, openableValves.Remove(currentValve), distances, remainingActions - distances[(startValve, currentValve)], 0, 0, ref maxPressureRelief);
    }
    return maxPressureRelief;
}

void MaximumPressureReleaseRecurse(Valve currentValve, ImmutableArray<Valve> openableValves, ImmutableDictionary<(Valve, Valve), int> distances, int remainingTime, int currentReliefRate, int currentRelief, ref int maxPressureRelief)
{
    currentReliefRate += currentValve.FlowRate;

    // Ran out of time
    if (remainingTime == 0)
    {
        maxPressureRelief = Math.Max(maxPressureRelief, currentRelief);
    }

    // all valves open
    else if (openableValves.Length == 0)
    {
        maxPressureRelief = Math.Max(maxPressureRelief, currentRelief + (currentReliefRate * remainingTime));
    }
    
    // Recurse as we have time and there are more valves
    else
    {

        for (int i = 0; i < openableValves.Length; i++)
        {
            var nextOpenValve = openableValves[i];
            var timeToOpen = Math.Min(remainingTime, distances[(currentValve, nextOpenValve)]);
            MaximumPressureReleaseRecurse(nextOpenValve, openableValves.Remove(nextOpenValve), distances, remainingTime - timeToOpen, currentReliefRate, currentRelief + currentReliefRate * timeToOpen, ref maxPressureRelief);
        }
    }
}

int Q2MaximumPressureRelease(ImmutableArray<Valve> valves, ImmutableDictionary<(Valve, Valve), int> distances, int remainingActions)
{
    var startValve = new[] { valves.Single(t => t.Name == "AA") };
    var openableValves = valves.Where(t => t.FlowRate > 0).ToImmutableArray();
    if (openableValves.Length > 32)
        throw new NotSupportedException();

    // Try... all combinations?
    var totalReleasedForCombo = new Dictionary<int, int>
    {
        { 0, 0 }
    };
    for (int i = 1; i < (1 << openableValves.Length + 1); i++) {
        var shardedValves = openableValves.Where((valve, index) => ((1 << index) & i) != 0).Concat(startValve).ToImmutableArray();
        totalReleasedForCombo[i] = MaximumPressureRelease(shardedValves, distances, remainingActions);
    }

    int max = 0;
    for (int i = 0; i < (1 << openableValves.Length + 1); i++) {
        var inverted = (~i) & ((1 << openableValves.Length + 1) - 1);
        max = Math.Max(max, totalReleasedForCombo[i] + totalReleasedForCombo[inverted]);
    }
    return max;
}


static (ImmutableArray<Valve> valves, ImmutableDictionary<(Valve, Valve), int> distances) Parse(string[] rawData)
{
    var regex = new Regex("Valve (..) has flow rate=(\\d*); tunnel[s]? lead[s]? to valve[s]? (.*)", RegexOptions.Compiled);
    var valveSystem = rawData
        .Select(t => regex.Match(t))
        .Where(t => t.Success)
        .Select(t => new Valve(t.Groups[1].Value, int.Parse(t.Groups[2].Value), t.Groups[3].Value.Split(',').Select(t => t.Trim()).ToArray()))
        .ToImmutableDictionary(t => t.Name);

    foreach (var valve in valveSystem.Values)
        valve.Valves.AddRange(valve.Destinations.Select(t => valveSystem[t]!));

    // Compute shortest distance between points
    var distances = new Dictionary<(Valve, Valve), int>();
    foreach (var first in valveSystem.Values)
    {
        foreach (var second in valveSystem.Values)
        {
            var q = new Queue<(Valve node, int l)>();
            q.Enqueue((first, 0));
            var visited = new HashSet<Valve>();

            while (q.Count > 0)
            {
                var cur = q.Dequeue();

                if (cur.node == second)
                {
                    distances[(first, second)] = cur.l;
                    break;
                }

                foreach (var n in cur.node.Valves)
                    if (visited.Add(n))
                        q.Enqueue((n, cur.l + 1));
            }
        }
    }

    // Add 1 to account for the fact it takes a whole cycle to open the valve. The 'cost' of moving to a valve should be
    // the cost of moving to an *open* valve. Who cares about closed ones.
    return (valveSystem.Values.ToImmutableArray(), distances.ToDictionary(kvp => kvp.Key, kvp => kvp.Value + 1).ToImmutableDictionary());
}

class Valve
{
    public string Name { get; }
    public int FlowRate { get; }
    public string[] Destinations { get; }
    public List<Valve> Valves { get; } = new List<Valve>();

    public Valve(string name, int flowRate, string[] destinations)
        => (Name, FlowRate, Destinations) = (name, flowRate, destinations);

    public override int GetHashCode()
        => Name.GetHashCode();

    public override bool Equals(object? obj)
        => obj == this;

    public override string ToString()
        => Name;
}
