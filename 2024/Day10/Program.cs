namespace Day10
{
    class Program
    {
        static void Main (string[] args)
        {
            var input = File.ReadAllLines ("input.txt")
                .Select (t => t.ToArray ().Select (c => c - '0').ToArray ())
                .ToArray ();

            // Count the unique paths to the peaks... just in case.
            var count = 0;
            for (int y = 0; y < input.Length; y++) {
                for (int x = 0; x < input[y].Length; x++) {
                    if (input[x][y] == 0)
                        count += CountTrails (input, x, y).Distinct ().Count ();
                }
            }

            Console.WriteLine ($"Q1: {count}");
        }

        static IEnumerable<(int x, int y)> CountTrails (int[][] map, int x, int y)
        {
            var current = map[x][y];
            if (current == 9) {
                yield return (x, y);
            } else {
                foreach (int delta in new[] { -1, 1 }) {
                    if ((x + delta) < map.Length && (x + delta >= 0) && map[x + delta][y] == current + 1)
                        foreach (var possible in CountTrails (map, x + delta, y))
                            yield return possible;
                    if ((y + delta) < map[x].Length && (y + delta >= 0) && map[x][y + delta] == current + 1)
                        foreach (var possible in CountTrails (map, x, y + delta))
                            yield return possible;
                }
            }
        }
    }
}
