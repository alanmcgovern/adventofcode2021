using System.Collections.Frozen;
using System.Numerics;

namespace Day18
{
    class Program
    {
        readonly record struct Point (int X, int Y) : IAdditionOperators<Point, Point, Point>
        {
            public static Point operator + (Point left, Point right)
                => new Point (left.X + right.X, left.Y + right.Y);
        }

        static void Main (string[] args)
        {
            var points = File.ReadAllLines ("input.txt")
                .Select (t => t.Split (','))
                .Select (t => new Point (int.Parse (t[0]), int.Parse (t[1])));

            Console.WriteLine ($"Q1: {ShortestDistance (new Point (0, 0), new Point (70, 70), points.Take (1024))}");
        }

        static int ShortestDistance (Point start, Point end, IEnumerable<Point> impassablePoints)
        {
            // append impassable nodes representing grid edges.
            var impassable = impassablePoints.ToHashSet ();
            for (int i = 0; i <= end.X; i++) {
                impassable.Add (new Point (i, -1));
                impassable.Add (new Point (i, end.Y + 1));
            }
            for (int i = 0; i <= end.Y; i++) {
                impassable.Add (new Point (-1, i));
                impassable.Add (new Point (end.X + 1, i));
            }

            var offsets = new[] { new Point (0, 1), new Point (0, -1), new Point (1, 0), new Point (-1, 0) };
            var queue = new PriorityQueue<Point, int> ([(start, 0)]);
            var dist = new Dictionary<Point, int> { { start, 0 } };
            while (queue.Count > 0) {
                var current = queue.Dequeue ();
                foreach (var offset in offsets) {
                    var nextNode = current + offset;
                    if (!impassable.Contains (nextNode) && !dist.ContainsKey (nextNode)) {
                        dist[nextNode] = dist[current] + 1;
                        queue.Enqueue (nextNode, dist[nextNode]);
                    }
                }
            }

            return dist[end];
        }
    }
}
