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

        static readonly IDictionary<Position, char> KeyPad = new Dictionary<Position, char> {
            {new Position (0, 0), '7' }, {new Position (1, 0), '8' }, {new Position (2, 0), '9' },
            {new Position (0, 1), '4' }, {new Position (1, 1), '5' }, {new Position (2, 1), '6' },
            {new Position (0, 2), '1' }, {new Position (1, 2), '2' }, {new Position (2, 2), '3' },
                                         {new Position (1, 3), '0' }, {new Position (2, 3), 'A' }
        }.ToImmutableDictionary ();

        static readonly IDictionary<Position, char> DirectionPad = new Dictionary<Position, char> {
                                         {new Position (1, 0), '^' }, {new Position (2, 0), 'A' },
            {new Position (0, 1), '<' }, {new Position (1, 1), 'v' }, {new Position (2, 1), '>' },
        }.ToImmutableDictionary ();

        // Arranged furthest to closest.
        // Arranged so the direction can be reversed by adding '2'.
        //
        // Who doesn't love a good magic number :P 
        //
        static readonly ImmutableList<(char, Position)> Movements = [
            ('<', Position.Left),
            ('v', Position.Down),
            ('>', Position.Right),
            ('^', Position.Up),
        ];

        static void Main (string[] args)
        {
            var codes = File.ReadLines ("input.txt")
                .Select (t => ("A" + t).ToImmutableArray ())
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
            var completePaths = codes.Select (code => GetCompleteMoves (code, 2));
        }

        static string GetCompleteMoves (IList<char> code, int robotNumPads)
        {
            // Get the moves on the keypad.
            var sb = new StringBuilder ();
            List<Position> keyPadMoves = new List<Position> ();
            for (int i = 0; i < code.Count; i ++) {
                var from = i == 0 ? 'A' : code[i - 1];
                var to = code[i];

                var path = ShortestPath (KeyPad, from, to);
                keyPadMoves.AddRange (path);
            }
            return sb.ToString ();
        }

        static StringBuilder ShortestPath (IDictionary<Position, char> device, char startChar, char endChar)
        {
            var sb = new StringBuilder ();
            var start = device.Single (t => t.Value == startChar).Key;
            var end = device.Single (t => t.Value == endChar).Key;

            var queue = new PriorityQueue<Position, int> ([(start, 0)]);
            var dist = new Dictionary<Position, int> { { start, 0 } };
            var previousNodes = new Dictionary<Position, Position> { { start, start } };

            while (queue.Count > 0) {
                var currentPos = queue.Dequeue ();
                foreach (var nextDirection in Position.AllDirections) {
                    var nextPos = currentPos + nextDirection;
                    if (!device.ContainsKey (nextPos))
                        continue;

                    var nextDist = dist[currentPos] + 1;
                    previousNodes[nextPos] = currentPos;
                    dist[nextPos] = nextDist;
                    queue.Enqueue (nextPos, nextDist);
                }
            }

            return sb.ToString ();
        }
    }
}
