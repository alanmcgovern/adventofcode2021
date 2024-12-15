
namespace Day12
{
    class Program
    {
        static void Main (string[] args)
        {
            int y = 0;
            var input = new Dictionary<(int, int), char> ();
            foreach (var line in File.ReadAllLines ("input.txt")) {
                for (int x = 0; x < line.Length; x++)
                    input.Add ((x, y), line[x]);
                y++;
            }

            // Count the size of the fence for each plot...
            // dupe it and then iterate til success!
            var allPlots = new List<Dictionary<(int x, int y), char>> ();
            var plots = input.ToDictionary ();
            while (plots.Count > 0) {
                var first = plots.First ();
                plots.Remove (first.Key);

                var currentPlot = new Dictionary<(int, int), char> ([first]);
                GetAllNeighbours (plots, first, currentPlot);
                allPlots.Add (currentPlot);
            }

            Console.WriteLine ($"Q1 {allPlots.Select (t => CountFenceSize (t) * t.Count).Sum ()}");
        }

        private static int CountFenceSize (Dictionary<(int x, int y), char> currentPlot)
        {
            var fenceSize = 0;
            foreach (var node in currentPlot) {
                if (!currentPlot.ContainsKey ((node.Key.x, node.Key.y - 1)))
                    fenceSize++;
                if (!currentPlot.ContainsKey ((node.Key.x, node.Key.y + 1)))
                    fenceSize++;
                if (!currentPlot.ContainsKey ((node.Key.x - 1, node.Key.y)))
                    fenceSize++;
                if (!currentPlot.ContainsKey ((node.Key.x + 1, node.Key.y)))
                    fenceSize++;
            }
            return fenceSize;
        }

        static void GetAllNeighbours (Dictionary<(int x, int y), char> plots, KeyValuePair<(int x, int y), char> currentNode, Dictionary<(int x, int y), char> currentPlot)
        {
            foreach ((int x, int y) delta in new[] { (-1, 0), (1, 0), (0, -1), (0, 1) }) {
                var deltaNode = (currentNode.Key.x + delta.x, currentNode.Key.y + delta.y);
                if (plots.TryGetValue (deltaNode, out char value) && value == currentNode.Value) {

                    // Move it over.
                    currentPlot.Add (deltaNode, value);
                    plots.Remove (deltaNode);

                    // Recurse!
                    GetAllNeighbours (plots, new KeyValuePair<(int x, int y), char> (deltaNode, value), currentPlot);
                }
            }
        }
    }
}
