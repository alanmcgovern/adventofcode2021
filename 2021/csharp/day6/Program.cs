// If i have a list of fish and i want to access
// one fish, what do i name it? Fish? fishes? LeFish?
var fishes = File.ReadAllText("input.txt")
    .Split(",", StringSplitOptions.RemoveEmptyEntries)
    .Select(int.Parse)
    .GroupBy(t => t);

foreach (var days in new int[] { 18, 80, 256 }) {
    var fishByAge = new long[9];
    foreach (var age in fishes)
        fishByAge[age.Key] = age.Count();

    for (int i = 0; i < days; i++)
        fishByAge[(i + 7) % 9] += fishByAge[(i % 9)];
    Console.WriteLine($"Total fish after {days} days: {fishByAge.Sum()}");
}