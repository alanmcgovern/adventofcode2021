namespace Day05
{
    class Program
    {
        static void Main (string[] args)
        {
            var lines = File.ReadAllLines ("input.txt");

            var ordering = new List<(int, int)> ();
            foreach (var line in lines.TakeWhile (t => !string.IsNullOrEmpty (t))) {
                if (string.IsNullOrEmpty (line))
                    break;

                var parts = line.Split ('|');
                ordering.Add ((int.Parse (parts[0]), int.Parse (parts[1])));
            }

            var updates = new List<List<int>> ();
            foreach (var line in lines.SkipWhile (t => !string.IsNullOrEmpty (t)))
                if (!string.IsNullOrEmpty (line))
                    updates.Add (new List<int> (line.Split (',').Select (int.Parse)));

            var correctUpdates = updates.Where (update => {
                for (int i = 1; i < update.Count; i++) {
                    if (!ordering.Any (t => t.Item1 == update[i - 1] && t.Item2 == update[i]))
                        return false;
                }
                return true;
            });

            Console.WriteLine ($"Q1: {correctUpdates.Select (t => t[t.Count / 2]).Sum ()}");


            // Part2
            var incorrectlyOrdered = updates.Except (correctUpdates).ToList ();
            foreach (var update in incorrectlyOrdered) {
                update.Sort ((left, right) => {
                    if (ordering.Any (node => node.Item1 == left && node.Item2 == right))
                        return -1;
                    if (ordering.Any (node => node.Item2 == left && node.Item1 == right))
                        return 1;

                    // Assume only one element doesn't have to be before any others, and so it's first.
                    return -1;
                });
            }

            Console.WriteLine ($"Q2: {incorrectlyOrdered.Select (t => t[t.Count / 2]).Sum ()}");
        }
    }
}
