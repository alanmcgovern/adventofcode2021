bool sample = false;

ReadOnlyMemory<char> binary = File.ReadAllLines(sample ? "sample.txt" : "input.txt")
    .SelectMany(line => line.ToArray())
    .SelectMany(c => Convert.ToString(Convert.ToInt32($"{c}", 16), 2).PadLeft(4, '0'))
    .ToArray()
    .AsMemory();

Console.WriteLine($"Q1: {Parse(binary).Sum(t => t.Version)}");


static IEnumerable<Header> Parse(ReadOnlyMemory<char> memory)
{
    while (memory.Length > 0)
    {
        var packet = new Header(memory.Slice(0, 6));
        memory = memory.Slice(6);

        if (packet.PacketType == 4)
        {
            memory = memory.Slice(LiteralValuePacket.Parse(memory).Length);
        }
        else
        {
            if (memory.Span[0] == '0')
                memory = memory.Slice(16);
            else
                memory = memory.Slice(12);
        }
        yield return packet;
    }
}


class Header
{
    ReadOnlyMemory<char> Memory { get; }
    protected ReadOnlySpan<char> Span => Memory.Span;

    public int Length => Memory.Length;

    public int Version => Span.Slice(0, 3).ToInt ();
    public int PacketType => Span.Slice(3, 3).ToInt();

    public Header(ReadOnlyMemory<char> memory)
        => (Memory) = (memory);
}

class Packet
{
    protected ReadOnlyMemory<char> Memory { get; }
    protected ReadOnlySpan<char> Span => Memory.Span;

    public int Length => Memory.Length;
    public Packet(ReadOnlyMemory<char> memory)
        => Memory = memory;
}

class OperatorPacket
{
    public List<Packet> Packets { get; } = new List<Packet>();
}

class LiteralValuePacket : Packet
{
    public static LiteralValuePacket Parse (ReadOnlyMemory<char> memory)
    {
        var start = memory;
        for (int i = 0; i < start.Length; i += 5)
            if (start.Span[i] == '0')
                return new LiteralValuePacket (start.Slice(0, i + 5));
        throw new InvalidDataException();

    }

    public LiteralValuePacket(ReadOnlyMemory<char> memory)
        : base(memory)
    {

    }

    public int Value
    {
        get
        {
            int value = 0;
            var valueSpan = Span;
            while (valueSpan.Length > 0)
            {
                value = (value << 4) + valueSpan.Slice(1, 4).ToInt();
                valueSpan = valueSpan.Slice(5);
            }
            return value;
        }
    }
}

static class Extensions
{
    public static int ToInt(this ReadOnlySpan<char> span)
    {
        int result = 0;
        for (int i = 0; i < span.Length; i++)
            result |= (span[i] == '1' ? 1 : 0) << (span.Length - i - 1);
        return result;
    }
}