namespace ConsoleApp1
{
    class Program
    {
        static void Main (string[] args)
        {
            char[][] grid = File.ReadAllLines ("input.txt")
                .Select (t => t.ToArray ())
                .ToArray ();

            var needle = "XMAS".ToArray ();

            var directions = Enumerable.Range (-1, 3).SelectMany (x => Enumerable.Range (-1, 3).Select (y => (x, y)))
                .Except (new[] { (0, 0) })
                .ToArray ();

            Console.WriteLine ($"Q1: {directions.Select (direction => Search (grid, needle, direction)).Sum ()}");

            Console.WriteLine ($"Q2: {Searchmas (grid)}");

        }

        static int Search (char[][] haystack, char[] needle, (int x, int y) direction)
        {
            int totalFound = 0;
            for (int i = 0; i < haystack.Length; i++) {
                for (int j = 0; j < haystack[i].Length; j++) {
                    bool found = true;
                    int needle_index = 0;
                    for (int needle_i = i, needle_j = j; found && needle_index < needle.Length; needle_i += direction.x, needle_j += direction.y, needle_index++) {
                        found &= needle_i >= 0 && needle_j >= 0 && needle_i < haystack.Length && needle_j < haystack[i].Length
                            && haystack[needle_i][needle_j] == needle[needle_index];
                    }
                    if (found && needle_index == needle.Length)
                        totalFound++;
                }
            }
            return totalFound;
        }

        static int Searchmas (char[][] haystack)
        {
            int totalFound = 0;

            var directions = new[] {
                new(int x, int y)[] { (1, 1), (-1, -1) },
                new(int x, int y)[]{ (1, -1), (-1, 1) }
            };

            var needles = new[] { "MS", "SM" };
            for (int i = 1; i < haystack.Length - 1; i++) {
                for (int j = 1; j < haystack[i].Length - 1; j++) {
                    if (haystack[i][j] != 'A')
                        continue;

                    totalFound += directions.All (direction => {
                        return needles.Any (needle => {
                            return needle[0] == haystack[i + direction[0].x][j + direction[0].y] && needle[1] == haystack[i + direction[1].x][j + direction[1].y];
                        });
                    }) ? 1 : 0;
                }
            }
            return totalFound;
        }
    }
}
