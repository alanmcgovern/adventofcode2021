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

        readonly record struct Node(Position Position, Position Direction);

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

            (var pathLength, var optimalTileCount) = CalculateDistance (path, new Node (start, Position.Right), end);
            Console.WriteLine ($"Q1: {pathLength}");
            Console.WriteLine ($"Q2: {optimalTileCount}");
        }

        static (int pathLength, int optimalTiles) CalculateDistance (HashSet<Position> path, Node start, Position end)
        {
            var queue = new PriorityQueue<Node, int> ([(start, 0)]);
            var dist = new Dictionary<Node, int> { { start, 0 } };
            var previousNodes = new Dictionary<Node, HashSet<Node>> { { start, new HashSet<Node> { start } } };

            while (queue.Count > 0) {
                var currentNode = queue.Dequeue ();
                foreach ((var nextDirection, var cost) in new[] { (currentNode.Direction, 1), (currentNode.Direction.Flip (), 1000), (currentNode.Direction.Flip ().Reverse (), 1000) }) {
                    // Either walk forward or rotate 90 degrees. Both are valid 'moves'.
                    var nextNode = currentNode.Direction == nextDirection ?
                        new Node (currentNode.Position + nextDirection, nextDirection) :
                        new Node (currentNode.Position, nextDirection);
                    if (!path.Contains (nextNode.Position))
                        continue;

                    var nextDist = dist[currentNode] + cost;
                    if (dist.TryGetValue (nextNode, out var knownShortest)) {
                        if (nextDist < knownShortest) {
                            previousNodes[nextNode].Clear ();
                            previousNodes[nextNode].Add (currentNode);
                        } else if (nextDist == knownShortest) {
                            previousNodes[nextNode].Add (currentNode);
                        }
                    } else {
                        previousNodes[nextNode] = new HashSet<Node> { currentNode };
                        dist[nextNode] = nextDist;
                        queue.Enqueue (nextNode, nextDist);
                    }
                }
            }

            var queueToCheck = new Queue<Node> (Position.AllDirections.Select (t => new Node (end, t)));
            var optimalTiles = new HashSet<Node> ();

            while (queueToCheck.Count > 0) { 
                var cur = queueToCheck.Dequeue ();

                if (optimalTiles.Add (cur)) {
                    foreach (var prev in previousNodes[cur]) {
                        if (prev != cur)
                            queueToCheck.Enqueue (prev);
                    }
                }
            }

            var optimalDistance = Position.AllDirections.Select (t => new Node (end, t))
                .Select (t => dist[t])
                .Min ();

            var optimalTileCount = optimalTiles
                .Select (t => t.Position)
                .Distinct ()
                .Count ();
            return (pathLength: optimalDistance, optimalTileCount: optimalTileCount);
        }
    }
}
