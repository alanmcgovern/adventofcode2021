using System.ComponentModel;
using System.Transactions;

namespace Day06
{
    class Program
    {
        enum Orientation
        {
            North, East, South, West
        }

        enum Type
        {
            Open,
            Wall,
        }

        class Position
        {
            public Type Type { get; set; }
            public bool Visited { get; set; }
        }

        static ((int x, int y) position, Orientation nextOrientation) NextPositionAndOrientation (Dictionary<(int x, int y), Position> map, (int x, int y) position, Orientation orientation)
        {
            static (int x, int y) NextPosition ((int x, int y) position, Orientation orientation)
            {
                return orientation switch {
                    Orientation.North => (position.x, position.y - 1),
                    Orientation.South => (position.x, position.y + 1),
                    Orientation.East => (position.x + 1, position.y),
                    Orientation.West => (position.x - 1, position.y),
                    _ => throw new NotSupportedException ()
                };
            }

            var nextPosition = NextPosition (position, orientation);
            while (map.ContainsKey (nextPosition) && map[nextPosition].Type == Type.Wall) {
                orientation = (Orientation) ((((int) orientation) + 1) % Enum.GetValues<Orientation> ().Length);
                nextPosition = NextPosition (position, orientation);
            }
            return (nextPosition, orientation);
        }

        static void Main (string[] args)
        {
            var map = new Dictionary<(int x, int y), Position> ();
            (int x, int y) startingPosition = (-1, -1);

            var lines = File.ReadAllLines ("input.txt");
            for (int y = 0; y < lines.Length; y++) {
                for (int x = 0; x < lines[y].Length; x++) {
                    map[(x, y)] = new Position { Type = lines[y][x] == '#' ? Type.Wall : Type.Open };
                    if (lines[y][x] == '^')
                        startingPosition = (x, y);
                }
            }

            var orientation = Orientation.North;
            var position = startingPosition;
            while (map.ContainsKey (position)) {
                map[position].Visited = true;
                (position, orientation) = NextPositionAndOrientation (map, position, orientation);
            }
            Console.WriteLine ($"Q1: {map.Count (t => t.Value.Visited)}");


            // We only need to put obstacles in the path where we know the guard walks.
            // We then need to count the variants where the guard ends up meeting himself.
            int loops = 0;
            foreach (var v in map.Where (t => t.Value.Visited && t.Key != startingPosition)) {
                map[v.Key] = new Position { Type = Type.Wall, Visited = true };

                var slowPosition = startingPosition;
                var fastPosition = startingPosition;

                var slowOrientation = Orientation.North;
                var fastOrientation = Orientation.North;

                do {
                    // Walk once
                    (slowPosition, slowOrientation) = NextPositionAndOrientation (map, slowPosition, slowOrientation);

                    // Walk twice
                    (fastPosition, fastOrientation) = NextPositionAndOrientation (map, fastPosition, fastOrientation);
                    (fastPosition, fastOrientation) = NextPositionAndOrientation (map, fastPosition, fastOrientation);
                } while (map.ContainsKey (fastPosition) && map.ContainsKey (slowPosition) && !(slowPosition == fastPosition && slowOrientation == fastOrientation));

                // If the two walkers meet, there's a loop.
                if (slowPosition == fastPosition && slowOrientation == fastOrientation)
                    loops++;

                map[v.Key] = v.Value;
            }
            Console.WriteLine ($"Q2: {loops}");
        }
    }
}
