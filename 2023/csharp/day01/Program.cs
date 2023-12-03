var numbers = File.ReadAllLines("input.txt")
    .Select(t => t.Where(digit => digit >= '0' && digit <= '9').ToArray ())
    .Select (t => $"{t.First ()}{t.Last ()}")
    .Select (int.Parse)
    .ToArray();

Console.WriteLine($"Q1: {numbers.Sum ()}");
