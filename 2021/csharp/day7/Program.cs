var positions = File.ReadAllText("input.txt")
    .Split(',')
    .Select(t => int.Parse (t))
    .ToArray();

var lowestPosition = positions.Min();
var highestPosition = positions.Max();

int bestPosition = -100;
var lowestCost = int.MaxValue;
for (int i = lowestPosition; i <= highestPosition; i++)
{
    var cost = positions.Select(position => Math.Abs(position - i)).Sum();
    if (cost < lowestCost)
    {
        lowestCost = cost;
        bestPosition = i;
    }
}

Console.WriteLine($"Best position {bestPosition}. Fuel cost: {lowestCost}");