using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Day21
{
    class Program
    {
        readonly record struct Position (int X, int Y) : IAdditionOperators<Position, Position, Position>, ISubtractionOperators<Position, Position, Position>
        {
            public static readonly Position Left = new (-1, 0);
            public static readonly Position Right = new (1, 0);
            public static readonly Position Up = new (0, -1);
            public static readonly Position Down = new (0, 1);

            public static readonly ISet<Position> AllDirections = new HashSet<Position> ([Left, Right, Up, Down]).ToFrozenSet ();

            public static Position operator + (Position left, Position right)
                => new (left.X + right.X, left.Y + right.Y);

            public static Position operator - (Position left, Position right)
                => new (left.X - right.X, left.Y - right.Y);
        }

        static void Main (string[] args)
        {
            var codes = File.ReadLines ("input.txt")
                .ToImmutableArray ();

            // I was originally going to calculate this depth first, and 'solve'
            // each layer independently. However, the more I think about it, the
            // more likely it is that there'll be paths which are globally optimal
            // but aren't necessarily optimal at a given layer.
            //
            // This is why i've usually stopped before question 20. This amount of
            // thought only goes into actual programs/software i'm writing :p
            //
            // Let's try a different approach...
            //
            //
            // Narrator: some time later. after dinner...
            //
            // Thought some more.
            //
            // Regretted it.
            //
            // But at least it's 'easier' because we "just" need to figure out how to move
            // left or right on the base numpad, and the moves will be (effectively) repeated
            // every time we recurse into a numpad. I bet we'll need an int64 for this.
            //
            // The number's gonna be huge.
            //
            // Maybe let's create a transition table instead?

            var transitionTable = CreateTransitionTable (codes);

            long x = 0;
            if ((x = Calculate (CreateTransitionTable (new[] { "029A" }), "029A", 0)) != 12)
                throw new Exception ("Should be 12. was " + x);
            if ((x = Calculate (CreateTransitionTable (new[] { "029A" }), "029A", 1)) != 28)
                throw new Exception ("Should be 28. was " + x);

            var results = codes.Select (code => (code, Calculate (transitionTable, code, 2), NumFromCode (code))).ToArray ();
            Console.WriteLine ($"Q1 {results.Select (t => t.Item2 * t.Item3).Sum ()}");

            results = codes.Select (code => (code, Calculate (transitionTable, code, 25), NumFromCode (code))).ToArray ();
            Console.WriteLine ($"Q2 {results.Select (t => t.Item2 * t.Item3).Sum ()}");

        }

        static Dictionary<(char src, char dst), string> CreateTransitionTable (IList<string> codes)
        {
            var codeTable = new Dictionary<(char, char), string> ();

            var keyPad = new Dictionary<Position, char> {
                {new Position (0, 0), '7' }, { new Position (1, 0), '8' }, { new Position (2, 0), '9' },
                {new Position (0, 1), '4' }, { new Position (1, 1), '5' }, { new Position (2, 1), '6' },
                {new Position (0, 2), '1' }, { new Position (1, 2), '2' }, { new Position (2, 2), '3' },
                                             { new Position (1, 3), '0' }, { new Position (2, 3), 'A' }
            };

            // Avoid the hole at (0,3)
            foreach ((var srcPos, var srcKey) in keyPad)
                foreach ((var dstPos, var dstKey) in keyPad)
                    codeTable[(srcKey, dstKey)] = GeneratePathBetweenPoints (srcPos, dstPos, new Position (0, 3));

            var directionPad = new Dictionary<Position, char> {
                                             { new Position (1, 0), '^' }, { new Position (2, 0), 'A' },
                {new Position (0, 1), '<' }, { new Position (1, 1), 'v' }, { new Position (2, 1), '>' },
            };

            // Avoid the hole at (0,0)
            foreach ((var srcPos, var srcKey) in directionPad)
                foreach ((var dstPos, var dstKey) in directionPad)
                    codeTable[(srcKey, dstKey)] = GeneratePathBetweenPoints (srcPos, dstPos, new Position (0, 0));

            return codeTable;
        }

        static string GeneratePathBetweenPoints (Position srcPos, Position dstPos, Position hole)
        {
            var delta = dstPos - srcPos;

            var xResult = new string (delta.X < 0 ? '<' : '>', Math.Abs (delta.X));
            var yResult = new string (delta.Y < 0 ? '^' : 'v', Math.Abs (delta.Y));

            // priority? Left, Down, Up, Right? Unless it enters the hole.
            if (delta.X < 0 && srcPos + new Position (delta.X, 0) != hole)
                return xResult + yResult;
            else if (delta.Y != 0 && srcPos + new Position (0, delta.Y) != hole)
                return yResult + xResult;
            else
                return xResult + yResult;
        }

        static long Calculate (Dictionary<(char src, char dst), string> lookup, string code, int dirPads)
        {
            // Rather than capturing a string, just keep appending the parts to the dictionary?
            Dictionary<string, long> overallFrequency = new Dictionary<string, long> ();
            var pos = 'A';
            for (int i = 0; i < code.Length; i++) {
                var p = lookup[(pos, code[i])] + "A";
                overallFrequency[p] = overallFrequency.GetValueOrDefault (p, 0) + 1;
                pos = code[i];
            }

            while (dirPads > 0) {
                var frequency = new Dictionary<string, long> ();
                foreach ((var move, var count) in overallFrequency) {
                    pos = 'A';
                    for (int i = 0; i < move.Length; i++) {
                        var nextPath = lookup[(pos, move[i])] + "A";
                        frequency[nextPath] = frequency.GetValueOrDefault (nextPath, 0) + count;
                        pos = move[i];
                    }
                }
                overallFrequency = frequency;
                dirPads--;
            }
            return overallFrequency.Select (t => t.Key.Length * t.Value).Sum ();
        }

        // Hopefully :P 
        static long NumFromCode(string code)
            => long.Parse (code.Substring (0, code.Length - 1));
    }
}
