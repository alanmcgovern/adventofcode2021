bool examples = true;

var snumbers = File.ReadAllLines(examples ? "examples.txt" : "input.txt")
    .Select(t => SnumberParser.Parse(t))
    .ToArray();

Console.Write(SnumberParser.Parse("[[[[[9,8],1],2],3],4]").Explode());
Console.Write(" == ");
Console.WriteLine(SnumberParser.Parse("[[[[0,9],2],3],4]"));
if (SnumberParser.Parse("[[[[[9,8],1],2],3],4]").Explode() != SnumberParser.Parse("[[[[0,9],2],3],4]"))
    throw new Exception("Derpa");

foreach (var snumber in snumbers)
    Console.WriteLine(snumber!.ToString ());

class Snumber
{
    public static Snumber Zero { get; } = new Snumber((0, null), (0, null));
    public (int? value, Snumber? snumber) Left = (null, null);
    public (int? value, Snumber? snumber) Right = (null, null);

    public Snumber ((int? value, Snumber? number) left, (int? value, Snumber? number) right)
        => (Left, Right) = (left, right);

    public Snumber Explode ()
    {
        // Throw away any remainder at this point
        if (TryExplode(0, out (int? value, Snumber? snumber) newValue, out int? _remainderLeft, out int? remainderRight))
            return newValue.snumber!;
        return this;
    }

    bool TryExplode(int depth, out (int? value, Snumber? snumber) newValue, out int? remainderLeft, out int? remainderRight)
    {
        newValue = (default, default);
        var newSnumber = this;
        remainderLeft = 0;
        remainderRight = 0;
        if (Left.snumber is not null)
        {
            if (Left.snumber.TryExplode (depth + 1, out (int? value, Snumber? snumber) newLeftSnumber, out remainderLeft, out remainderRight))
            {
                if (newSnumber.Right.value.HasValue && remainderRight > 0)
                {
                    newValue = (null, new Snumber(newLeftSnumber, (newSnumber.Right.value + remainderRight, null)));
                    remainderRight = 0;
                }
                else
                    newValue = (null, new Snumber(newLeftSnumber, newSnumber.Right));
                return true;
            }
        }
        if (Right.snumber is not null)
        {
            if (Right.snumber.TryExplode(depth + 1, out (int? value, Snumber? snumber) newRightSnumber, out remainderLeft, out remainderRight))
            {
                if (newSnumber.Left.value.HasValue && remainderLeft > 0)
                {
                    newValue = (null, new Snumber((newSnumber.Left.value + remainderLeft, null), newRightSnumber));
                    remainderRight = 0;
                }
                else
                    newValue = (null, new Snumber(newSnumber.Left, newRightSnumber));
            }
            return true;
        }

        if (Left.value.HasValue && Right.value.HasValue && depth >= 4)
        {
            remainderLeft = Left.value.Value;
            remainderRight = Right.value.Value;
            newValue = (0, null);
            return true;
        }

        return false;
    }

    bool Split ()
    {
        return true;

    }

    public override string ToString()
        => $"[{(Left.value.HasValue ? Left.value.Value : Left.snumber!)},{(Right.value.HasValue ? Right.value.Value : Right.snumber!)}]";


    public static Snumber operator +(Snumber left, Snumber right)
    {
        return Zero;
    }

}

static class SnumberParser
{
    public static Snumber Parse(ReadOnlySpan<char> buffer)
    {
        if (buffer[0] != '[')
            throw new ArgumentException();
        Parse(ref buffer, out (int? value, Snumber? snumber) output);
        return output.snumber!;
    }

    static bool Parse(ref ReadOnlySpan<char> buffer, out (int? number, Snumber? value) value)
    {
        if (buffer[0] == '[')
        {
            buffer = buffer.Slice(1);
            Parse(ref buffer, out (int? value, Snumber? snumber) left);
            if (buffer[0] != ',')
                throw new ArgumentException();
            buffer = buffer.Slice(1);
            Parse(ref buffer, out (int? value, Snumber? snumber) right);
            if (buffer[0] != ']')
                throw new ArgumentException();
            buffer = buffer.Slice(1);
            value = (null, new Snumber(left, right));
            return false;
        }
        else if (buffer[0] >= '0' && buffer[0] <= '9')
        {
            value = (buffer[0] - '0', null);
            buffer = buffer.Slice(1);
            return true;
        }
        else
            throw new NotSupportedException();
    }
}