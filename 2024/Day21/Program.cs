﻿using System.Collections.Frozen;
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

            if (Calculate (CreateTransitionTable (new[] { "029A" }), "029A", 2) != 68)
                throw new Exception ();

            var results = codes.Select (code => (code, Calculate (transitionTable, code, 2), NumFromCode (code))).ToArray ();
            foreach (var result in results)
                Console.WriteLine ($"{result.code} - {result.Item2} - {result.Item3}");
            Console.WriteLine ($"Q1 {results.Select (t => t.Item2 * t.Item3).Sum ()}");

            results = codes.Select (code => (code, Calculate (transitionTable, code, 25), NumFromCode (code))).ToArray ();
            foreach (var result in results)
                Console.WriteLine ($"{result.code} - {result.Item2} - {result.Item3}");
            Console.WriteLine ($"Q1 {results.Select (t => t.Item2 * t.Item3).Sum ()}");

        }

        static Dictionary<(char src, char dst), string> CreateTransitionTable (IList<string> codes)
        {
            var codeTable = new Dictionary<(char, char), string> ();

            // I hardcoded this the wrong way around.
            //
            // Sue me. Just reverse it
            IDictionary<char, Position> KeyPad = new Dictionary<Position, char> {
                {new Position (0, 0), '7' }, {new Position (1, 0), '8' }, {new Position (2, 0), '9' },
                {new Position (0, 1), '4' }, {new Position (1, 1), '5' }, {new Position (2, 1), '6' },
                {new Position (0, 2), '1' }, {new Position (1, 2), '2' }, {new Position (2, 2), '3' },
                                             {new Position (1, 3), '0' }, {new Position (2, 3), 'A' }
            }.ToDictionary (key => key.Value, val => val.Key);

            // Avoid the hole at (0,3) by choosing to do either the horzs or verts first
            // depending on which helps us avoid the hole
            foreach (var code in codes.Select (t => "A" + t)) {
                for (int i = 1; i < code.Length; i++) {
                    var startPos = KeyPad[code[i - 1]];
                    var endPos = KeyPad[code[i]];

                    var delta = endPos - startPos;

                    var xResult = new string (delta.X < 0 ? '<' : '>', Math.Abs (delta.X));
                    var yResult = new string (delta.Y < 0 ? '^' : 'v', Math.Abs (delta.Y));

                    // priority? Left, Down, Up, Right?
                    if (delta.X < 0 && startPos + new Position (delta.X, 0) != new Position (0, 3))
                        codeTable[(code[i - 1], code[i])] = xResult + yResult;
                    else if (delta.Y != 0 && startPos + new Position (0, delta.Y) != new Position (0, 3))
                        codeTable[(code[i - 1], code[i])] = yResult + xResult;
                    else
                        codeTable[(code[i - 1], code[i])] = xResult + yResult;
                }
            }

            IDictionary<Position, char> DirectionPad = new Dictionary<Position, char> {
                                         {new Position (1, 0), '^' }, {new Position (2, 0), 'A' },
                {new Position (0, 1), '<' }, {new Position (1, 1), 'v' }, {new Position (2, 1), '>' },
            }.ToImmutableDictionary ();

            // Avoid the hole at (0,0) by choosing to do either the horzs or verts first
            // depending on which helps us avoid the hole
            foreach (var src in DirectionPad) {
                foreach (var dst in DirectionPad) {

                    var delta = dst.Key - src.Key;

                    var xResult = new string (delta.X < 0 ? '<' : '>', Math.Abs (delta.X));
                    var yResult = new string (delta.Y < 0 ? '^' : 'v', Math.Abs (delta.Y));
                    if ((src.Key + new Position (delta.X, 0)) == new Position (0, 0))
                        codeTable[(src.Value, dst.Value)] = yResult + xResult;
                    else
                        codeTable[(src.Value, dst.Value)] = xResult + yResult;
                }
            }

            return codeTable;
        }

        static long Calculate (Dictionary<(char src, char dst), string> lookup, string code, int dirPads)
        {
            var movements = "";
            var numPadTargets = "A" + code;
            for (int i = 1; i < numPadTargets.Length; i++)
                movements += lookup[(numPadTargets[i - 1], numPadTargets[i])] + "A";

            while (dirPads > 0) {
                movements = "A" + movements;
                // There's just one direction pad in the example...
                var dirPadMovements = "";
                for (int i = 1; i < movements.Length; i++) {
                    dirPadMovements += lookup[(movements[i - 1], movements[i])] + "A";
                }
                movements = dirPadMovements;
                dirPads--;
            }
            return movements.Length;
        }

        // Hopefully :P 
        static int NumFromCode(string code)
            => int.Parse (code.Substring (0, code.Length - 1));
    }
}
