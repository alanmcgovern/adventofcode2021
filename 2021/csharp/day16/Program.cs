using System.Collections;

// Convert to 'binary' 1s and 0s 
ReadOnlyMemory<byte> binary = File.ReadAllLines("input.txt")
    .SelectMany(line => line.ToArray())
    .SelectMany(c => Convert.ToString(Convert.ToInt32($"{c}", 16), 2).PadLeft(4, '0'))
    .Select(c => (byte)(c - '0'))
    .ToArray();

var packet = PacketParser.Parse(binary);
Console.WriteLine($"Q1: {packet.Sum(t => t.Version)}");
Console.WriteLine($"Q2: {packet.Compute()}");

static class PacketParser
{
    public static Packet Parse(ReadOnlyMemory<byte> memory)
     => Parse(ref memory);

    static Packet Parse(ref ReadOnlyMemory<byte> memory)
    {
        var packetMemory = memory.Slice(6);
        if ((Type)memory.Slice(3, 3).ToLong() == Type.LiteralValue)
            _ = ReadLiteralValue(ref packetMemory);
        else if (packetMemory.Span[0] == 0)
            packetMemory = packetMemory.Slice(16);
        else
            packetMemory = packetMemory.Slice(12);

        var packet = new Packet(memory.Slice(0, memory.Length - packetMemory.Length));
        memory = memory.Slice(packet.Memory.Length);

        if (packet.Value.lengthInBits > 0)
        {
            var remaining = memory.Length - packet.Value.lengthInBits.Value;
            while (remaining != memory.Length)
                packet.Operands.Add(Parse(ref memory));
        }
        else if (packet.Value.lengthInMessages > 0)
        {
            while (packet.Operands.Count != packet.Value.lengthInMessages.GetValueOrDefault(0))
                packet.Operands.Add(Parse(ref memory));
        }
        return packet;
    }

    public static long ReadLiteralValue(ReadOnlyMemory<byte> memory)
        => ReadLiteralValue(ref memory);

    static long ReadLiteralValue(ref ReadOnlyMemory<byte> memory)
    {
        long value = 0;
        bool shouldExit = false;
        while (memory.Length > 0 && !shouldExit)
        {
            value = (value << 4) + memory.Span.Slice(1, 4).ToLong();
            shouldExit = memory.Span[0] == 0;
            memory = memory.Slice(5);
        }
        return value;
    }
}

struct Packet : IEnumerable<Packet>
{
    public ReadOnlyMemory<byte> Memory { get; }
    public List<Packet> Operands { get; } = new List<Packet>();
    public Type PacketType => (Type)Memory.Slice(3, 3).ToLong();
    public long Version => Memory.Slice(0, 3).ToLong();

    public (long? lengthInBits, long? lengthInMessages, long? literalValue) Value
    {
        get
        {
            var dataStart = Memory.Slice(6);
            if (PacketType == Type.LiteralValue)
                return (default, default, PacketParser.ReadLiteralValue(dataStart));
            else if (dataStart.Span[0] == 0)
                return (dataStart.Slice(1).ToLong(), default, default);
            else
                return (default, dataStart.Slice(1).ToLong(), default);
        }
    }

    public Packet(ReadOnlyMemory<byte> memory)
        => Memory = memory;

    public long Compute()
        => PacketType switch
        {
            Type.Sum => Operands.Sum(t => t.Compute()),
            Type.Product => Operands.Select(t => t.Compute()).Aggregate((left, right) => left * right),
            Type.Min => Operands.Select(t => t.Compute()).Min(),
            Type.Max => Operands.Select(t => t.Compute()).Max(),
            Type.LiteralValue => Value.literalValue!.Value,
            Type.GreaterThan => Operands[0].Compute() > Operands[1].Compute() ? 1 : 0,
            Type.LessThan => Operands[0].Compute() < Operands[1].Compute() ? 1 : 0,
            Type.Equal => Operands[0].Compute() == Operands[1].Compute() ? 1 : 0,
            _ => throw new NotSupportedException()
        };

    public IEnumerator<Packet> GetEnumerator()
    {
        yield return this;

        foreach (var child in Operands)
            foreach (var message in child)
                yield return message;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}

static class Extensions
{
    public static long ToLong(this ReadOnlyMemory<byte> memory)
        => memory.Span.ToLong();

    public static long ToLong(this ReadOnlySpan<byte> span)
    {
        long result = 0;
        for (int i = 0; i < span.Length; i++)
            result |= (span[i] == 1 ? 1L : 0) << (span.Length - i - 1);
        return result;
    }
}

enum Type
{
    Sum = 0,
    Product = 1,
    Min = 2,
    Max = 3,
    LiteralValue = 4,
    GreaterThan = 5,
    LessThan = 6,
    Equal = 7
}
