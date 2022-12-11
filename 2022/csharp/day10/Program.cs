var instructionsList = File.ReadLines("input.txt")
    .Select(t => t.Split(' '))
    .Select(parts => (Enum.Parse<Instruction>(parts[0]), parts.Length == 1 ? null : parts[1]));

Console.WriteLine($"Q1: {CalculateSignalStrength(220)}");

long CalculateSignalStrength(int atClock) {
    var instructionQueue = new Queue<(Instruction, string?)>(instructionsList);

    long X = 1;
    (Instruction type, string? args) instruction = default;
    int remainingCycles = 0;
    long strength = 0;
    foreach (var clock in Enumerable.Range(1, 2000))
    {
        if (remainingCycles == 0)
        {
            if (instructionQueue.Count == 0)
                break;
            instruction = instructionQueue.Dequeue();
            remainingCycles = (int)instruction.type;
        }

        if (clock == 20 || (clock - 20) % 40 == 0)
            strength += clock * X;

        if ((clock - 1) % 40 >= (X - 1) && (clock - 1) % 40 <= (X + 1))
            Console.Write("#");
        else
            Console.Write(".");

        if (clock % 40 == 0)
            Console.WriteLine();

        remainingCycles--;
        if (instruction.type == Instruction.addx && remainingCycles == 0)
            X += int.Parse(instruction.args!);
    }

    return strength;
}

enum Instruction
{
    None,
    noop = 1,
    addx = 2,
}