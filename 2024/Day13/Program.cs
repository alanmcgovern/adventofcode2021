using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Day13
{
    class Program
    {
        readonly record struct Button(long XDelta, long YDelta, long Cost);
        readonly record struct Destination (long X, long Y);
        readonly record struct Game(Button A, Button B, Destination Prize);
        readonly record struct Solution(long Cost);

        static void Main (string[] args)
        {
            var lines = File.ReadAllLines ("input.txt");
            var games = new List<Game> ();

            Button a = default;
            Button b = default;
            Destination target = default;
            var matcher = new Regex (@"X[=+](\d+), Y[=+](\d+)");
            foreach (var line in lines) {
                if (string.IsNullOrEmpty (line)) {
                    continue;
                }
                var match = matcher.Match (line);
                var x = long.Parse (match.Groups[1].Value);
                var y = long.Parse (match.Groups[2].Value);
                if (line.StartsWith ("Button A", StringComparison.Ordinal)) {
                    a = new Button (x, y, 3);
                } else if (line.StartsWith ("Button B", StringComparison.Ordinal)) {
                    b = new Button (x, y, 1);
                } else {
                    target = new Destination (x, y);
                    games.Add (new Game (a, b, target));
                }
            }

            Console.WriteLine ($"Q1: {games.Select (CalculateCost).Where (t => t.HasValue).Sum (t => t.Value.Cost)}");

            // heh heh heh.
            games = games.Select (t => new Game (t.A, t.B, new Destination (t.Prize.X + 10000000000000L, t.Prize.Y + 10000000000000L)))
                .ToList ();
            Console.WriteLine ($"Q2: {games.Select (CalculateCost).Where (t => t.HasValue).Sum (t => t.Value.Cost)}");
        }

        static Solution? CalculateCost(Game game)
        {
            // standard simultaenous equations... urgh.
            long a1x = game.A.XDelta, b1y = game.B.XDelta, c1 = game.Prize.X;
            long a2x = game.A.YDelta, b2y = game.B.YDelta, c2 = game.Prize.Y;

            long aPresses = (c1 * b2y - b1y * c2) / (a1x * b2y - b1y * a2x);
            long bPresses = (a1x * c2 - c1 * a2x) / (a1x * b2y - b1y * a2x);

            // validate?
            if ((game.A.XDelta * aPresses + game.B.XDelta * bPresses) != game.Prize.X ||
                (game.A.YDelta * aPresses + game.B.YDelta * bPresses) != game.Prize.Y)
                return null;

            return new Solution (aPresses * game.A.Cost + bPresses * game.B.Cost);
        }
    }
}
