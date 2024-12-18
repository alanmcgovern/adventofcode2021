using System.Numerics;
using System.Text;

namespace Day15
{
    readonly record struct Position (int X, int Y) : IAdditionOperators<Position, Position, Position>, ISubtractionOperators<Position, Position, Position>
    {
        public static Position operator + (Position left, Position right)
            => new (left.X + right.X, left.Y + right.Y);

        public static Position operator - (Position left, Position right)
            => new (left.X - right.X, left.Y - right.Y);
    }

    enum Direction
    {
        Up = '^',
        Down = 'v',
        Left = '<',
        Right = '>'
    }

    enum ContentType
    {
        Robot = '@',
        Space = '.',
        Wall = '#',
        Box = 'O'
    }

    class MainClass
    {
        internal class Content (ContentType type, List<Position>? parts = null)
        {
            // Only non-empty for multi-part items, such as Boxes. natch.
            public List<Position> Parts { get; } = parts ?? [];
            public ContentType Type { get; } = type;
        }

        static (Dictionary<Position, Content>, Direction[]) LoadData (bool doubled)
        {
            var map = new Dictionary<Position, Content> ();
            var lines = File.ReadAllLines ("input.txt");
            int y = 0;
            foreach (var grid in lines.TakeWhile (t => !string.IsNullOrEmpty (t))) {
                for (int x = 0; x < grid.Length; x++) {
                    Position firstPos, secondPos;
                    if (doubled) {
                        firstPos = new Position (x * 2, y);
                        secondPos = new Position (x * 2 + 1, y);
                    } else {
                        firstPos = secondPos = new Position (x, y);
                    }

                    var contentType = (ContentType) grid[x];
                    switch (contentType) {
                        case ContentType.Space:
                        case ContentType.Wall:
                            map.Add (firstPos, new Content (contentType));
                            map[secondPos] = new Content (contentType);
                            break;
                        case ContentType.Robot:
                            map.Add (firstPos, new Content (contentType));
                            if (doubled)
                                map[secondPos] = new Content (ContentType.Space);
                            break;
                        case ContentType.Box:
                            var content = new Content (contentType, new[] { firstPos, secondPos }.Distinct().ToList ());
                            map.Add (firstPos, content);
                            map[secondPos] = content;
                            break;
                    }
                }
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
            var part1 = Run (false, true);
            var part2 = Run (true);

            Console.WriteLine ($"Q1: {part1}");
            Console.WriteLine ($"Q2: {part2}");
        }

        static int Run (bool doubled, bool visualization = false)
        {
            (var map, var directions) = LoadData (doubled);

            // replace robot with space... for convenience.
            var robot = map.First (t => t.Value.Type == ContentType.Robot).Key;
            map[robot] = new Content (ContentType.Space);

            for (int i = 0; i < directions.Length; i++) {
                var delta = directions[i] switch {
                    Direction.Left => new Position (-1, 0),
                    Direction.Right => new Position (1, 0),
                    Direction.Up => new Position (0, -1),
                    Direction.Down => new Position (0, 1),
                    _ => throw new NotSupportedException ()
                };

                if (CanMove (map, robot, delta, out var boxes)) {
                    if (boxes is not null)
                        Move (map, boxes, delta);
                    robot += delta;
                }

                // Let's have a look!
                if (visualization) {
                    Console.SetCursorPosition (0, 0);
                    var width = map.Keys.Max (t => t.X) + 1;
                    Console.WriteLine ("Enjoy the visulization".PadRight (width));
                    Console.WriteLine ($"{(float) i * 100 / directions.Length:0.00}% complete");
                    Console.WriteLine ("".PadRight (width));

                    Print (map, robot);
                }
            }

            return map
                .Where (t => t.Value.Type == ContentType.Box)
                .Select (t => t.Value)
                .Distinct ()
                .Select (t => t.Parts[0])
                .Sum (t => t.X + t.Y * 100);
        }

        static void Print (Dictionary<Position, Content> map, Position robot)
        {
            var sb = new StringBuilder ();

            for (int y = 0; y <= map.Keys.Max (t => t.Y); y++) {
                for (int x = 0; x <= map.Keys.Max (t => t.X); x++) {
                    if (new Position (x, y) == robot) {
                        sb.Append ('@');
                    } else if (map.TryGetValue (new Position (x, y), out Content? content) && content != null) {
                        if (content.Type == ContentType.Space)
                            sb.Append ('.');
                        else if (content.Type == ContentType.Wall)
                            sb.Append ('#');
                        else if (content.Type == ContentType.Box)
                            sb.Append (content.Parts[0] == new Position (x, y) ? (content.Parts.Count == 1 ? "O" : "[") : "]");
                    }
                }
                sb.AppendLine ();
            }
            Console.WriteLine (sb.ToString ());
        }

        static bool CanMove (Dictionary<Position, Content> map, Position location, Position delta, out HashSet<Content>? boxes)
        {
            boxes = null;
            var destItem = map[location + delta];
            if (destItem.Type == ContentType.Space)
                return true;
            if (destItem.Type == ContentType.Wall)
                return false;

            var foundBoxes = new HashSet<Content> ();
            if (CanMoveBox (map, destItem, delta, foundBoxes)) {
                boxes = foundBoxes;
                return true;
            }
            return false;
        }

        static bool CanMoveBox (Dictionary<Position, Content> map, Content box, Position delta, HashSet<Content> foundBoxes)
        {
            if (foundBoxes.Contains (box))
                throw new InvalidOperationException ("shouldn't happen");
            foundBoxes.Add (box);
            foreach (var part in box.Parts) {
                // Can't push yourself
                if (map[part + delta] == box)
                    continue;

                //
                // We can push the same box twice. pushing c will cause x to be pushed twice.
                //     xx
                //    aabb
                //     cc
                //
                if (foundBoxes.Contains (map[part + delta]))
                    continue;

                // We're pushing a box, so recurse!
                if (map[part + delta].Type == ContentType.Box)
                    if (!CanMoveBox (map, map[part + delta], delta, foundBoxes))
                        return false;

                // If it's a wall, we fail.
                if (map[part + delta].Type == ContentType.Wall)
                    return false;
            }

            return true;
        }

        static void Move (Dictionary<Position, Content> map, HashSet<Content> boxes, Position delta)
        {
            foreach (var box in boxes) {
                foreach (var position in box.Parts) {
                    map.Remove (position);
                    map.Add (position, new Content (ContentType.Space));
                }
            }
            foreach (var box in boxes) {
                var newBox = new Content (box.Type, box.Parts.Select (t => t + delta).ToList ());
                foreach (var position in newBox.Parts) {
                    if (map[position].Type != ContentType.Space)
                        throw new Exception ("whoops");
                    map[position] = newBox;
                }
            }
        }
    }
}
