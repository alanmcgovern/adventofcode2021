namespace Day11
{
    class Program
    {
        static void Main (string[] args)
        {
            IList<long> stones = File.ReadAllLines ("input.txt")
                .First ()
                .Split (' ', StringSplitOptions.RemoveEmptyEntries)
                .Select (long.Parse)
                .ToArray ()
                .AsReadOnly ();

            foreach (var val in Enumerable.Range (0, 25))
                stones = Iterate (stones);

            Console.WriteLine ($"Q1 {stones.Count}");
        }

        static IList<long> Iterate (IList<long> stones)
        {
            var transformed = new List<long> ();
            foreach (var stone in stones) {
                if (stone == 0) {
                    transformed.Add (1);
                } else if (stone.ToString ().Length % 2 == 0) {
                    var str = stone.ToString ();
                    var l = str.Substring (0, str.Length / 2);
                    var r = str.Substring (str.Length / 2);
                    transformed.Add (long.Parse (l));
                    transformed.Add (long.Parse (r));
                } else {
                    transformed.Add (stone * 2024);
                }
            }
            return transformed;
        }
    }
}
