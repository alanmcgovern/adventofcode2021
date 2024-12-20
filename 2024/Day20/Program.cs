using System.Drawing;
using System.Numerics;

namespace Day20
{
    class Program
    {
        readonly record struct Position (int X, int Y) : IAdditionOperators<Position, Position, Position>
        {
            public static Position operator + (Position left, Position right)
                => new Position (left.X + right.X, left.Y + right.Y);
        }

        static readonly Position Left = new Position (-1, 0);
        static readonly Position Right = new Position (1, 0);
        static readonly Position Up = new Position (0, -1);
        static readonly Position Down = new Position (0, 1);

        enum Area
        {
            ThinWall,
            Wall,
            Track,
        }

        static void Main (string[] args)
        {
            int y = 0;
            Position start = default, end = default;
            var track = new Dictionary<Position, Area> ();
            foreach (var line in File.ReadLines ("input.txt")) {
                for (int x = 0; x < line.Length; x++) {
                    if (line[x] == 'S')
                        start = new Position (x, y);
                    else if (line[x] == 'E')
                        end = new Position (x, y);
                    track.Add (new Position (x, y), line[x] == '#' ? Area.Wall : Area.Track);
                }
                y++;
            }

            // Pattern match for skippable walls.
            //
            //      track -> wall -> track
            //
            // convert into
            //
            //      track -> thin wall -> track
            //
            foreach (var position in track.Where (t => t.Value == Area.Track).ToArray ()) {
                foreach (var adjacent in new[] { Left, Right, Up, Down }) {
                    if (track.GetValueOrDefault (position.Key + adjacent, Area.ThinWall) == Area.Wall &&
                        track.GetValueOrDefault (position.Key + adjacent + adjacent, Area.ThinWall) == Area.Track) {
                        track[position.Key + adjacent] = Area.ThinWall;
                    }
                }
            }

            var regularDistance = CalculateDistance (track, start, start, end);

            var thinWalls = track.Where (t => t.Value == Area.ThinWall).ToArray ();

            // Now, shortest distance which each thin wall removed.
            var distances = thinWalls
                .Select ((t, i) => CalculateDistance (track, t.Key, start, end))
                .ToArray ();

            Console.WriteLine ($"Q1: {distances.Count (t => regularDistance - t >= 100)}");
        }

        static int CalculateDistance (Dictionary<Position, Area> track, Position thinWall, Position start, Position end)
        {
            var queue = new PriorityQueue<Position, int> ([(start, 0)]);
            var dist = new Dictionary<Position, int> { { start, 0 } };

            while (queue.Count > 0) {
                var currentPos = queue.Dequeue ();
                foreach (var adjacent in new[] { Left, Right, Up, Down }) {
                    var nextPos = currentPos + adjacent;
                    if (dist.ContainsKey (nextPos))
                        continue;
                    if (track[nextPos] == Area.Track || nextPos == thinWall) {
                        dist[nextPos] = dist[currentPos] + 1;
                        queue.Enqueue (nextPos, dist[nextPos]);
                    }
                }
            }

            return dist[end];
        }
    }
}
