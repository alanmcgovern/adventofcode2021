var movements = File.ReadAllLines("input.txt")
    .Select(t => t.Split(' '))
    .Select(t => (t[0], int.Parse(t[1])))
    .ToArray();

//part 1
(var depth, var forward, var aim) = movements.ProcessMovements(tuple => tuple.movement switch
    {
        "forward" => (tuple.depth + tuple.aim * tuple.value,
                      tuple.forward + tuple.value,
                      0),

        "up" =>      (tuple.depth - tuple.value,
                      tuple.forward,
                      0),

        "down" =>    (tuple.depth + tuple.value,
                      tuple.forward,
                      0),
        _ => throw new NotSupportedException(tuple.movement)
    });
Console.WriteLine($"Q1: Depth: {depth}. Forward: {forward}. Product: {forward * depth}");

// Part 2
(depth, forward, aim) = movements.ProcessMovements(tuple => tuple.movement switch
        {
            "forward" => (tuple.depth + tuple.aim * tuple.value,
                          tuple.forward + tuple.value,
                          tuple.aim),

            "up" =>      (tuple.depth,
                          tuple.forward,
                          tuple.aim - tuple.value),

            "down" =>    (tuple.depth,
                          tuple.forward,
                          tuple.aim + tuple.value),

            _ => throw new NotSupportedException(tuple.movement)
        });

Console.WriteLine($"Q2: Depth: {depth}. Forward: {forward}. Product: {(long)forward * depth}");

static class Extensions
{
    public static (int depth, int forward, int aim) ProcessMovements(this IEnumerable<(string, int)> enumerable, Func<(string movement, int value, int depth, int forward, int aim), (int depth, int forward, int aim)> func)
    {
        int depth = 0, forward = 0, aim = 0;
        foreach (var move in enumerable)
            (depth, forward, aim) = func((move.Item1, move.Item2, depth, forward, aim));
        return (depth, forward, aim);
    }
}
