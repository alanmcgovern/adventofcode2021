var elves = File.ReadAllText("input.txt")
    .Split($"{Environment.NewLine}{Environment.NewLine}")
    .Select(t => t.Split(Environment.NewLine).Select(int.Parse))
    .ToArray();

Console.WriteLine($"Q1: {elves.Select(t => t.Sum()).Max()}");

Console.WriteLine($"Q2: {elves.Select(t => t.Sum()).Order ().TakeLast(3).Sum ()}");
