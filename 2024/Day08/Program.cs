using System.Runtime.ExceptionServices;

namespace Day08
{
    static class Extensions
    {
        public static IEnumerable<(T, T)> Permutations<T> (this IEnumerable<T> enumerable)
        {
            // let's be lazy :p
            var array = enumerable.ToArray ();
            for (int i = 0; i < array.Length - 1; i++) {
                for (int j = i + 1; j < array.Length; j++) {
                    yield return (array[i], array[j]);
                }
            }
        }

        public static IEnumerable<Tower> Antinodes (this (Tower first, Tower second) towers)
        {
            // (1, 2) -> (5, 7)
            var xDelta = towers.first.Position.x - towers.second.Position.x;
            var yDelta = towers.first.Position.y - towers.second.Position.y;

            var antinode1 = (towers.first.Position.x + xDelta, towers.first.Position.y + yDelta);
            var antinode2 = (towers.second.Position.x - xDelta, towers.second.Position.y - yDelta);

            yield return new Tower { Frequency = towers.first.Frequency, Position = antinode1 };
            yield return new Tower { Frequency = towers.first.Frequency, Position = antinode2 };
        }
    }

    class Tower
    {
        public char? Frequency { get; set; }
        public (int x, int y) Position { get; set; }

        public override int GetHashCode () => Position.GetHashCode ();
        public override bool Equals (object? obj) => obj is Tower other && Position.Equals (other.Position);
    }

    class Program
    {

        static void Main (string[] args)
        {
            var map = new HashSet<Tower> ();

            var lines = File.ReadAllLines ("input.txt");
            for (int y = 0; y < lines.Length; y++) {
                for (int x = 0; x < lines[y].Length; x++)
                    map.Add (new Tower { Frequency = lines[y][x] == '.' ? null : lines[y][x], Position = (x, y) });
            }

            // group by frequency, then get permutations. The position for antinodes must exist in the hashset.
            var antinodes = map
                .Where (t => t.Frequency.HasValue)
                .GroupBy (t => t.Frequency)
                .SelectMany (t => t.Permutations (restricted: true))
                .SelectMany (pair => pair.Antinodes ())
                .Where (t => map.Contains (t))
                .Distinct ()
                .Count ();
            Console.WriteLine ($"Q1 {antinodes}");

            // Well...
            var antinodes = map
                .Where (t => t.Frequency.HasValue)
                .GroupBy (t => t.Frequency)
                .SelectMany (t => t.Permutations ())
                .SelectMany (pair => pair.Antinodes (restricted: false))
                .Where (t => map.Contains (t))
                .Distinct ()
                .Count ();
            Console.WriteLine ($"Q1 {antinodes}");
        }
    }
}
