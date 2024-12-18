using System.Numerics;

namespace Day15
{
    class Program
    {
        enum Content
        {
            Robot = '@',
            Space = '.',
            Wall = '#',
            Box = 'O'
        }

        enum Direction
        {
            Up = '^',
            Down = 'v',
            Left = '<',
            Right = '>'
        }

        readonly record struct Position (int X, int Y) : IAdditionOperators<Position, Position, Position>, ISubtractionOperators<Position, Position, Position>
        {
            public static Position operator + (Position left, Position right)
                => new (left.X + right.X, left.Y + right.Y);

            public static Position operator - (Position left, Position right)
                => new (left.X - right.X, left.Y - right.Y);
        }

        static (Dictionary<Position, Content>, Direction[]) LoadData ()
        {
            var map = new Dictionary<Position, Content> ();
            var lines = File.ReadAllLines ("input.txt");
            int y = 0;
            foreach (var grid in lines.TakeWhile (t => !string.IsNullOrEmpty (t))) {
                for (int x = 0; x < grid.Length; x++)
                    map.Add (new Position (x, y), (Content)grid[x]);
                y++;
            }

            var directions = new List<Direction> ();
            foreach (var routePart in lines.SkipWhile (t => !string.IsNullOrEmpty (t))) {
                foreach (var d in routePart)
                    directions.Add ((Direction) d);
            }
            return (map, directions.ToArray ());
        }

        static void Main (string[] args)
        {
            (var map, var directions) = LoadData ();

            // replace robot with space... for convenience.
            var robot = map.First (t => t.Value == Content.Robot).Key;
            map[robot] = Content.Space;

            foreach (var movement in directions) {
                var delta = movement switch {
                    Direction.Left => new Position (-1, 0),
                    Direction.Right => new Position (1, 0),
                    Direction.Up => new Position (0, -1),
                    Direction.Down => new Position (0, 1),
                    _ => throw new NotSupportedException ()
                };

                if (CanMove (map, robot, delta)) {
                    Move (map, robot, delta);
                    robot += delta;
                }
            }

            Console.WriteLine ($"Q1: {map.Where (t => t.Value ==  Content.Box).Sum (t => t.Key.X + t.Key.Y * 100)}");
        }

        static bool CanMove (Dictionary<Position, Content> map, Position location, Position delta)
        {
            if (map[location + delta] == Content.Space)
                return true;
            if (map[location + delta] == Content.Wall)
                return false;

            // Is there a space after the series of boxes?
            var shuffle = location + delta;
            while (map[shuffle] == Content.Box)
                shuffle += delta;
            return map[shuffle] == Content.Space;
        }

        static void Move (Dictionary<Position, Content> map, Position location, Position delta)
        {
            // If it's a space, we just move into it.
            if (map[location + delta] == Content.Space)
                return;

            // Move the first box to the next open position.
            var firstBox = location + delta;
            var firstSpace = firstBox;
            while (map[firstSpace] == Content.Box)
                firstSpace += delta;

            map[firstSpace] = Content.Box;
            map[firstBox] = Content.Space;
        }
    }
}
