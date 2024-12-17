using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Day14
{
    class Program
    {
        readonly record struct Robot(int X, int Y, Velocity Velocity);
        readonly record struct Velocity(int xDelta, int yDelta);

        static void Main (string[] args)
        {
            var regex = new Regex (@"p=(-?\d+),(-?\d+)\s*v=(-?\d+),(-?\d+)");
            var robots = File.ReadAllLines ("input.txt")
                .Select (t => regex.Match (t))
                .Select (t =>  new Robot (
                    int.Parse (t.Groups[1].Value),
                    int.Parse (t.Groups[2].Value),
                    new Velocity (
                        int.Parse (t.Groups[3].Value),
                        int.Parse (t.Groups[4].Value)
                    )
                ))
                .ToArray ();

            // Calculate 100s worth...
            (int x, int y) roomSize = (101, 103);
            var newPositions = robots.Select (t => Move (t, 100, roomSize)).ToArray ();
            Console.WriteLine ($"Q1: {CalculateSafety (newPositions, roomSize)}");

            // Uhh.. maybe they're all beside each other?
            int iterationCounter = 0;
            newPositions = robots;
            while (!Together(newPositions)) {
                iterationCounter++;
                newPositions = newPositions.Select (t => Move (t, 1, roomSize)).ToArray ();
                if (iterationCounter > 10000)
                    throw new Exception ("probably not gonna happen?");
            }

            Print (newPositions, roomSize);

            Console.WriteLine ($"Q2: {iterationCounter}");
        }
        static void Print (Robot[] robots, (int x, int y) roomSize)
        {
            var allPositions = robots.Select (t => (t.X, t.Y)).ToHashSet ();

            Console.WriteLine ();
            for (int y = 0; y < roomSize.y; y++) {
                for (int x = 0; x < roomSize.x; x++) {
                    Console.Write (allPositions.Contains ((x, y)) ? "X" : " ");
                }
                Console.WriteLine ();
            }
        }
        static bool Together (Robot[] robots)
        {
            var offsets = new int[] { -1, 0, 1 };
            var allVariants = offsets.SelectMany (t => offsets.Select (u => (u, t))).Except (new[] { (0, 0) });
            var allPositions = robots.Select (t => (t.X, t.Y)).ToHashSet ();
            var count = robots.Where (robot => {
                return allVariants.Any (v => allPositions.Contains ((robot.X + v.Item1, robot.Y + v.Item2)));
            }).Count ();

            return count > ((robots.Length * 2) / 3);
        }

        static Robot Move (Robot r, int times, (int x, int y) room)
        {
            var newX = (r.X + r.Velocity.xDelta * times) % room.x;
            var newY = (r.Y + r.Velocity.yDelta * times) % room.y;
            if (newX < 0)
                newX += room.x;
            if (newY < 0)
                newY += room.y;

            return new Robot {
                X = newX,
                Y = newY,
                Velocity = r.Velocity
            };
        }

        static long CalculateSafety (Robot[] robots, (int x, int y) roomSize)
        {
            var midX = roomSize.x / 2;
            var midY = roomSize.y / 2;

            long topLeft = robots.Count (t => t.X < midX && t.Y < midY);
            long topRight = robots.Count (t => t.X > midX && t.Y < midY);
            long bottomRight = robots.Count (t => t.X > midX && t.Y > midY);
            long bottomLeft = robots.Count (t => t.X < midX && t.Y > midY);

            return topLeft * topRight * bottomRight * bottomLeft;
        }
    }
}
