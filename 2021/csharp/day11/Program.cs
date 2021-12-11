var swarm = File.ReadLines("input.txt")
    .Select(t => t.Select (t => new Octopus(t - '0')).ToArray())
    .ToArray();

// Connect em up!
for (int row = 0; row < swarm.Length; row++)
{
    for (int col = 0; col < swarm[row].Length; col++)
    {
        var octopus = swarm[row][col];
        octopus.Left = col > 0 ? swarm[row][col - 1] : null;
        octopus.Right = col < swarm[row].Length - 1 ? swarm[row][col + 1] : null;
        octopus.Top = row > 0 ? swarm[row - 1][col] : null;
        octopus.Bottom = row < swarm.Length - 1 ? swarm[row + 1][col] : null;
    }
}

int flashes = 0;
int steps = 100;
foreach (var step in Enumerable.Range (0, steps))
{
    foreach (var octopus in swarm.SelectMany(t => t))
        octopus.IncreaseEnergy();
    flashes += swarm.SelectMany(t => t).Count(t => t.HasFlashed);
    foreach (var octopus in swarm.SelectMany(t => t))
        octopus.Step();
}
Console.WriteLine($"Total flashes after {steps} steps: {flashes}.");

class Octopus
{
    const int FlashEnergy = 10;

    static int counter = 0;
    int Id { get; } = counter++;

    public Octopus? Left { get; set; }
    public Octopus? Right { get; set; }
    public Octopus? Top { get; set; }
    public Octopus? Bottom { get; set; }

    public int Energy { get; private set; }
    public bool HasFlashed => Energy >= FlashEnergy;

    public Octopus(int energy)
        => Energy = energy;

    public void IncreaseEnergy()
    {
        if (++Energy == FlashEnergy)
        {
            Left?.IncreaseEnergy();
            Right?.IncreaseEnergy();
            Top?.IncreaseEnergy();
            Bottom?.IncreaseEnergy();

            Left?.Top?.IncreaseEnergy();
            Left?.Bottom?.IncreaseEnergy();

            Right?.Top?.IncreaseEnergy();
            Right?.Bottom?.IncreaseEnergy();
        }
    }

    public void Step()
    {
        if (HasFlashed)
            Energy = 0;
    }

    public override string ToString()
        => $"{Id}";
}
