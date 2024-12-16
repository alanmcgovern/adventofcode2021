
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Day12
{

    static class Extensions
    {
        public static (int x, int y) Add (this (int x, int y) left, (int x, int y) right)
            => (left.x + right.x, left.y + right.y);
    }

    class Program
    {
        static readonly (int x, int y) Up = (0, 1);
        static readonly (int x, int y) Right = (1, 0);
        static readonly (int x, int y) Down = (0, -1);
        static readonly (int x, int y) Left = (-1, 0);


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

            Console.WriteLine ($"Q1 {allPlots.Select (t => CountFences (t) * t.Count).Sum ()}");

            Console.WriteLine ($"Q2 {allPlots.Select (t => CountCorners (input, t) * t.Count).Sum ()}");
        }

        private static int CountFences (Dictionary<(int x, int y), char> currentPlot)
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

        static int CountCorners (Dictionary<(int x, int y), char> all, Dictionary<(int x, int y), char> region)
            => region.Select (t => CountCorners (all, t.Key, t.Value)).Sum ();

        private static int CountCorners (Dictionary<(int x, int y), char> map, (int x, int y) plot, char plotType)
        {
            var res = 0;
            foreach (var (off1, off2) in new[] { (Up, Right), (Right, Down), (Down, Left), (Left, Up) }) {

                if (map.GetValueOrDefault (plot.Add (off1)) != plotType &&
                    map.GetValueOrDefault (plot.Add (off2)) != plotType)
                    res++;

                if (map.GetValueOrDefault (plot.Add (off1)) == plotType &&
                    map.GetValueOrDefault (plot.Add (off2)) == plotType &&
                    map.GetValueOrDefault (plot.Add (off1).Add (off2)) != plotType)
                    res++;
            }
            return res;
        }

        static void GetAllNeighbours (Dictionary<(int x, int y), char> plots, KeyValuePair<(int x, int y), char> currentNode, Dictionary<(int x, int y), char> currentPlot)
        {
            foreach ((int x, int y) delta in new[] { Left, Right, Down, Up }) {
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
