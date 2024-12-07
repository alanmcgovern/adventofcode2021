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

            Console.WriteLine ($"Q1: {input.Where (t => IsValid (t, [Operator.Mul, Operator.Add])).Sum (t => t.target)}");
            Console.WriteLine ($"Q2: {input.Where (t => IsValid (t, [Operator.Mul, Operator.Add, Operator.Pipe])).Sum (t => t.target)}");
        }

        static bool IsValid ((long target, ReadOnlyMemory<long> operands) equation, Operator[] allowedOperators)
            => Compute (equation.operands.Span[0], equation.operands[1..], allowedOperators).Any (t => t == equation.target);

        static IEnumerable<long> Compute (long val, ReadOnlyMemory<long> remainder, Operator[] allowedOperators)
        {
            if (remainder.Length == 0) {
                yield return val;
                yield break;
            }

            foreach (var op in allowedOperators) {
                var nextVal = op switch {
                    Operator.Mul => val * remainder.Span[0],
                    Operator.Add => val + remainder.Span[0],
                    Operator.Pipe => val * NextPowerOf10 (remainder.Span[0]) + remainder.Span[0],
                    _ => throw new InvalidOperationException ()
                };
                foreach (var v in Compute (nextVal, remainder[1..], allowedOperators))
                    yield return v;
            }
        }

        public static long NextPowerOf10 (long n)
        {
            if (n < 1)
                throw new NotSupportedException ();

            long pow = 1;
            while (pow <= n)
                pow *= 10;
            return pow;
        }
    }
}
