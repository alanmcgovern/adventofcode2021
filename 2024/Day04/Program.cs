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
    }
}
