namespace Day07
{
    class Program
    {
        enum Operator
        {
            Mul,
            Add,
            Pipe
        }

        static void Main (string[] args)
        {
            (long target, ReadOnlyMemory<long> operands)[] input = File.ReadAllLines ("input.txt").Select (line => {
                var target = long.Parse (line.Substring (0, line.IndexOf (':')));
                var operands = line.Substring (line.IndexOf (':') + 1).Split (' ', StringSplitOptions.RemoveEmptyEntries).Select (long.Parse).ToArray ();
                return (target, (ReadOnlyMemory<long>) operands);
            }).ToArray ();

            Console.WriteLine ($"Q1: {input.Where (IsValid).Sum (t => t.target)}");
        }

        static bool IsValid ((long target, ReadOnlyMemory<long> operands) equation)
            => Compute(equation.operands.Span[0],equation.operands.Slice(1)).Any (t => t == equation.target);

        static IEnumerable<long> Compute(long val, ReadOnlyMemory<long> remainder)
        {
            if (remainder.Length == 0) {
                yield return val;
                yield break;
            }

            foreach(var op in allowedOperations) {
                var nextVal = op switch {
                    Operator.Mul => val * remainder.Span[0],
                    Operator.Add => val + remainder.Span[0],
                    _ => throw new InvalidOperationException ()
                };
                foreach (var v in Compute (nextVal, remainder.Slice (1)))
                    yield return v;
            }
        }
    }
}
