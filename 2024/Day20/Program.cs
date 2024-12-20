using System.Collections.Frozen;
using System.Numerics;

namespace Day20
{
    static class Extensions
    {
        // We want a minimum saving of 100 picoseconds, so we should only make pairwise combinations of nodes at least 100 apart.
        public static IEnumerable<((T item, int itemIndex) left, (T item, int itemIndex) right)> Pairwise<T> (this IList<T> list, int minimumOffset)
        {
            for (int i = 0; i < list.Count; i++) {
                for (int j = i + minimumOffset; j < list.Count; j++)
                    yield return ((list[i], i), (list[j], j));
            }
        }
    }

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

            public int ManhattenDistance (Position other)
                => Math.Abs (other.X - X) + Math.Abs (other.Y - Y);
        }

        static void Main (string[] args)
        {
            int y = 0;
            Position start = default, end = default;
            var trackPoints = new HashSet<Position> ();
            foreach (var line in File.ReadLines ("input.txt")) {
                for (int x = 0; x < line.Length; x++) {
                    if (line[x] == 'S')
                        start = new Position (x, y);
                    else if (line[x] == 'E')
                        end = new Position (x, y);
                    if (line[x] != '#')
                        trackPoints.Add (new Position (x, y));
                }
                y++;
            }

            // get the track in the correct order
            var path = SortTrack (trackPoints, start);

            // Q1 and Q2 are the same now.
            Console.WriteLine ($"Q1 {CountCheats (path,  cheatLength: 2, minimumSaving: 100)}");
            Console.WriteLine ($"Q2 {CountCheats (path, cheatLength: 20, minimumSaving: 100)}");
        }

        static int CountCheats (List<Position> path, int cheatLength, int minimumSaving)
        {
            return path
                .Pairwise (minimumSaving)
                .Where (pair => pair.right.item.ManhattenDistance (pair.left.item) <= cheatLength)
                .Select (pair => pair.right.itemIndex - pair.left.itemIndex - pair.right.item.ManhattenDistance (pair.left.item))
                .Count (t => t >= minimumSaving);
        }

        static List<Position> SortTrack (HashSet<Position> trackPoints, Position start)
        {
            var result = new List<Position> ();
            trackPoints = trackPoints.ToHashSet ();
            while (trackPoints.Count > 0) {
                result.Add (start);
                trackPoints.Remove (start);
                foreach (var option in Position.AllDirections) {
                    if (trackPoints.Contains (start + option)) {
                        start += option;
                        break;
                    }
                }
            }
            return result;
        }
    }
}
