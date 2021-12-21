using System.Text.RegularExpressions;

var matches = Regex.Matches(File.ReadAllText("input.txt"), @"[xy]=(-?\d+)\.\.(-?\d+)");
var xMatch = matches.Single(m => m.Value.StartsWith("x"));
var yMatch = matches.Single(m => m.Value.StartsWith("y"));

(var x1, var x2) = (int.Parse(xMatch.Groups[1].Value), int.Parse(xMatch.Groups[2].Value));
(var y1, var y2) = (int.Parse(yMatch.Groups[1].Value), int.Parse(yMatch.Groups[2].Value));
if (y1 < y2)
    (y2, y1) = (y1, y2);
if (x1 > x2)
    (x2, x1) = (x1, x2);
// Start at 0,0. Figure out the maximum 'y' and
Console.WriteLine($"{x1}..{x2}.  {y1}..{y2}");

// Brute force it as we can put some reasonable bounding boxes on the value.
// v^2 = u^2 + 2as
// min 'x' velocity  to reach x1 is: (0)  = u^2 + 2(-1)(x1) => c
// max 'x' velocity to not exceed x2 is: x2.
var minXVelocity = (int) Math.Abs(Math.Sqrt(2 * x1)) - 1;
var maxXVelocity = x2 + 1;

// Min 'y' velocity to meet y2 is... y2. One shot wonder
// With the upper and lower bounds on X I could compute an actual bounding box, but computers are fast.
var minYVelocity = y2 - 1;
var maxYVelocity = 500 + 1;
(int x, int y)[] results = Enumerable.Range(minXVelocity, maxXVelocity + minXVelocity)
    .SelectMany(x => Enumerable.Range(minYVelocity, maxYVelocity + Math.Abs (y2)).Select(y => (x, y)))
   .ToArray();

Dictionary<(int x, int y), int> maxHeights = new Dictionary<(int x, int y), int>();
foreach (var initialVelocity in results)
{
    int maxHeight = 0;
    (int x, int y) position = (0, 0);
    (int x, int y) velocity = initialVelocity;

    while (position.x <= x2 && position.y >= y2)
    {
        (velocity, position) = Move(velocity, position);
        maxHeight = Math.Max(maxHeight, position.y);

        if (position.x >= x1 && position.x <= x2 && position.y <= y1 && position.y >= y2)
            maxHeights[initialVelocity] = maxHeight;
    }
}

Console.WriteLine($"Initial Velocity and max height: {maxHeights.OrderByDescending(t => t.Value).First()}");
Console.WriteLine($"Valid velocities: {maxHeights.Count}");

static ((int x, int y) velocity, (int x, int y) position) Move((int x, int y) velocity, (int x, int y) position)
    => ((Math.Max (0, velocity.x - 1), velocity.y - 1), (position.x + velocity.x, position.y + velocity.y));