namespace Day11
{
    class Program
    {
        static void Main (string[] args)
        {
            var stones = File.ReadAllLines ("input.txt")
                .First ()
                .Split (' ', StringSplitOptions.RemoveEmptyEntries)
                .Select (long.Parse)
                .GroupBy (t => t)
                .ToDictionary (t => t.Key, t => (long) t.Count ());

            var blunk = stones;
            foreach (var val in Enumerable.Range (0, 25))
                blunk = Iterate (blunk);

            Console.WriteLine ($"Q1 {blunk.Values.Sum ()}");

            blunk = stones;
            foreach (var val in Enumerable.Range (0, 75))
                blunk = Iterate (blunk);

            Console.WriteLine ($"Q2 {blunk.Values.Sum ()}");
        }

        static Dictionary<long, long> Iterate (Dictionary<long, long> stones)
        {
            var transformed = new Dictionary<long, long> ();

            void Increment (long stone, long count)
            {
                if (!transformed.TryGetValue (stone, out long cur))
                    cur = 0;
                transformed[stone] = cur + count;
            }

            foreach (var stone in stones) {
                if (stone.Key == 0) {
                    Increment (1, stone.Value);
                } else if (stone.Key.ToString ().Length % 2 == 0) {
                    var str = stone.Key.ToString ();
                    var l = str.Substring (0, str.Length / 2);
                    var r = str.Substring (str.Length / 2);
                    Increment (long.Parse (l), stone.Value);
                    Increment (long.Parse (r), stone.Value);
                } else {
                    Increment (stone.Key * 2024, stone.Value);
                }
            }
            return transformed;
        }
    }
}
