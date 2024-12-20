using System.Collections.Frozen;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Day16
{
    class Program
    {
        readonly record struct Position (int X, int Y) : IAdditionOperators<Position, Position, Position>
        {
            public static readonly Position Left = new (-1, 0);
            public static readonly Position Right = new (1, 0);
            public static readonly Position Up = new (0, -1);
            public static readonly Position Down = new (0, 1);

            public static readonly FrozenSet<Position> AllDirections = new HashSet<Position> ([Left, Right, Up, Down]).ToFrozenSet ();

            public static Position operator + (Position left, Position right)
                => new (left.X + right.X, left.Y + right.Y);

            public Position Reverse ()
                => new (-X, -Y);

            public Position Flip ()
                => new (Y, X);
        }

        static void Main (string[] args)
        {
            int y = 0;
            Position start = default, end = default;
            var path = new HashSet<Position> ();
            foreach (var line in File.ReadLines ("input.txt")) {
                for (int x = 0; x < line.Length; x++) {
                    if (line[x] == 'S')
                        start = new Position (x, y);
                    else if (line[x] == 'E')
                        end = new Position (x, y);
                    if (line[x] != '#')
                        path.Add (new Position (x, y));
                }
                y++;
            }

            (var pathLength, var optimalTileCount) = CalculateDistance (path, start, end, Position.Right);
            Console.WriteLine ($"Q1: {pathLength}");
            Console.WriteLine ($"Q2: {optimalTileCount}");
        }

        static (int pathLength, int optimalTiles) CalculateDistance (HashSet<Position> path, Position start, Position end, Position initialDirection)
        {
            var queue = new PriorityQueue<(Position, Position), int> ([((start, initialDirection), 0)]);
            var dist = new Dictionary<Position, int> { { start, 0 } };
            var previousNodes = new Dictionary<Position, HashSet<Position>> { { start, new HashSet<Position> { start } } };

            while (queue.Count > 0) {
                (var currentPos, var currentDirection) = queue.Dequeue ();
                foreach ((var nextDirection, var rotatingCost) in new[] { (currentDirection, 0), (currentDirection.Flip (), 1000), (currentDirection.Flip ().Reverse (), 1000) }) {
                    var nextPos = currentPos + nextDirection;
                    var nextDist = dist[currentPos] + 1 + rotatingCost;
                    if (!path.Contains (nextPos))
                        continue;
                    if (nextPos == new Position (5, 7))
                        Console.WriteLine (currentPos);

                    if (dist.TryGetValue (nextPos, out var knownShortest)) {
                        if (nextDist < knownShortest) {
                            previousNodes[nextPos].Clear ();
                            previousNodes[nextPos].Add (currentPos);
                        } else if (nextDist == knownShortest) {
                            previousNodes[nextPos].Add (currentPos);
                        }
                    } else {
                        if (previousNodes.ContainsKey (nextPos))
                            Console.WriteLine ("derp");
                        previousNodes[nextPos] = new HashSet<Position> { currentPos };
                        dist[nextPos] = nextDist;
                        queue.Enqueue ((nextPos, nextDirection), nextDist);
                    }
                }
            }

            var queueToCheck = new Queue<Position>([end]);
            var optimalTiles = new HashSet<Position> ();

            while (queueToCheck.Count > 0) { 
                var cur = queueToCheck.Dequeue ();

                if (optimalTiles.Add (cur)) {
                    foreach (var prev in previousNodes[cur]) {
                        if (prev != cur)
                            queueToCheck.Enqueue (prev);
                    }
                }
            }


            // print the fucker.
            StringBuilder sb = new StringBuilder ();
            for (int y = 0; y < path.Max (t => t.Y) + 1; y++) {
                for (int x = 0; x < path.Max (t => t.X) + 1; x++) {
                    var pos = new Position (x, y);
                    if (pos == start)
                        sb.Append ('S');
                    else if (pos == end)
                        sb.Append ('E');
                    else if (optimalTiles.Contains (pos))
                        sb.Append ('o');
                    else if (path.Contains (pos))
                        sb.Append ('.');
                    else
                        sb.Append ('#');
                }
                sb.AppendLine ();
            }

            Console.WriteLine (sb.ToString ());


            return (dist[end], optimalTiles.Count);
        }
    }
}
