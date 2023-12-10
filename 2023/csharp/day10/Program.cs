using System.Numerics;

var grid = ParseGrid(File.ReadAllLines("input.txt"));

var startingRowIndex = grid.FindIndex(t => t.Any(t => t.Connectors.Any(c => c == Direction.StartingPoint)));
var startingColIndex = grid[startingRowIndex].FindIndex(t => t.Connectors.Any(c => c == Direction.StartingPoint));

var directionsAndLengths = new[] { Direction.North, Direction.South, Direction.East, Direction.West }.Select(dir => (dir, FindPathLength(dir))).ToArray ();

Console.WriteLine($"Q1: {directionsAndLengths.OrderByDescending (t => t.Item2.steps).First ().dir} - {directionsAndLengths.Select(t => t.Item2.steps).Max() / 2}");

Console.WriteLine($"Q2: {PaintAndCount(directionsAndLengths.OrderByDescending(t => t.Item2.steps).First())}");

long PaintAndCount((Direction initialDirection, (long? steps, HashSet<(int, int)> visitedLocations, (int, int) lastLocationBeforeLoop) data) allData)
{
    var visitedLocations = allData.data.visitedLocations;
    var lastLocationBeforeLoop = allData.data.lastLocationBeforeLoop;
    var initialDirection = allData.initialDirection;

    // Zero out everywhere we have not been.
    for (int i = 0; i < grid.Count; i++)
        for (int j = 0; j < grid[i].Count; j++)
            if (!visitedLocations.Contains((i, j)))
                grid[i][j] = new Pipe(new[] { Direction.None });

    // Replace 'S' with the correct shape pipe
    var actualDirections = new List<Direction> { initialDirection };
    if (lastLocationBeforeLoop.Item1 == startingRowIndex)
    {
        if (lastLocationBeforeLoop.Item2 > startingColIndex)
            actualDirections.Add(Direction.East);
        else
            actualDirections.Add(Direction.West);
    } else
    {
        if (lastLocationBeforeLoop.Item1 > startingRowIndex)
            actualDirections.Add(Direction.North);
        else
            actualDirections.Add(Direction.South);
    }
    grid[startingRowIndex][startingColIndex] = new Pipe(actualDirections.ToArray());

    // Count inside for whole grid
    int count = 0;
    for (int i = 0; i < grid.Count; i++)
        for (int j = 0; j < grid[i].Count; j++)
            count += IsInside(grid, (i, j)) ? 1 : 0;
    return count;
}

bool IsInside(List<List<Pipe>> grid, (int, int) position)
{
    var cell = grid[position.Item1][position.Item2];

    // The only remaining items are the wall itself - so they aren't counted as contained.
    if (cell.Connectors.Length != 1)
        return false;

    // The internet says if we have to jump over an uneven number of vertical walls when moving left, the point is contained?
    // But we also only care about 'north' pipes, not south pipesl
    var inside = false;
    var currentPosition = position.Item2 - 1;
    while (currentPosition >= 0)
    {
        if (grid[position.Item1][currentPosition].Connectors.Contains (Direction.North))
            inside = !inside;
        currentPosition--;
    }
    return inside;
}


(long? steps, HashSet<(int, int)> visitedPoints, (int, int) lastLocationBeforeLoop) FindPathLength(Direction initialDirection)
{
    var startLocation = (startingRowIndex, startingColIndex);
    var lastLocationBeforeLoop = startLocation;
    var location = startLocation;
    Direction? cameFrom = null;
    int steps = 0;
    HashSet<(int, int)> visitedPoints = new HashSet<(int, int)>();
    do
    {
        var possibleDirections = grid[location.startingRowIndex][location.startingColIndex].Connectors;
        if (cameFrom.HasValue)
        {
            // This was an impossible path. We probably took an invalid direction from the starting point.
            if (!possibleDirections.Contains(cameFrom.Value))
                return (-1, new HashSet<(int, int)>(), (-1, -1));
            possibleDirections = possibleDirections.Except(new[] { cameFrom.Value }).ToArray();
        }
        else
        {
            // We don't know what S is, so let's assume it's something useful!
            possibleDirections = new[] { initialDirection };
        }

        visitedPoints.Add(location);

        var direction = possibleDirections.Single();
        lastLocationBeforeLoop = location;
        if (direction == Direction.North)
        {
            location = (location.startingRowIndex - 1, location.startingColIndex);
            cameFrom = Direction.South;
        }
        else if (direction == Direction.South)
        {
            location = (location.startingRowIndex + 1, location.startingColIndex);
            cameFrom = Direction.North;
        }
        else if (direction == Direction.East)
        {
            location = (location.startingRowIndex, location.startingColIndex + 1);
            cameFrom = Direction.West;
        }
        else if (direction == Direction.West)
        {
            location = (location.startingRowIndex, location.startingColIndex - 1);
            cameFrom = Direction.East;
        }
        else if (direction == Direction.None)
        {
            return (-1, new HashSet<(int, int)> (), (-1, -1));
        }
        else
        {
            throw new NotSupportedException();
        }
        steps++;
    } while (location != startLocation);
    return (steps, visitedPoints, lastLocationBeforeLoop);
}

List<List<Pipe>> ParseGrid(string[] strings)
{
    var groundPipe = new Pipe(new[] { Direction.None });
    var grid = strings.Select(row =>
    {
        List<Pipe> result = row.Select(pipe =>
        {
            var array = pipe switch
            {
                /*
                    | is a vertical pipe connecting north and south.
                     is a horizontal pipe connecting east and west.
                     is a 90-degree bend connecting north and east.
                    J is a 90-degree bend connecting north and west.
                    7 is a 90-degree bend connecting south and west.
                    F is a 90-degree bend connecting south and east.
                    . is ground; there is no pipe in this tile.
                    S is the starting position of the animal; there is a pipe on this tile, but your sketch doesn't show what shape the pipe has.
                */
                '|' => new[] { Direction.North, Direction.South },
                '-' => new[] { Direction.East, Direction.West },
                'L' => new[] { Direction.North, Direction.East },
                'J' => new[] { Direction.North, Direction.West },
                '7' => new[] { Direction.South, Direction.West },
                'F' => new[] { Direction.South, Direction.East },
                '.' => new[] { Direction.None },
                'S' => new[] { Direction.StartingPoint },
                _ => throw new NotSupportedException()
            };
            return new Pipe(array);
        }).ToList();
        
        // Bulk it out to not worry about indexoutofrange errors
        result.Insert(0, groundPipe);
        result.Add(groundPipe);
        return result;
    }).ToList ();

    // Bulk it out to not worry about indexoutofrange errors
    grid.Insert(0, Enumerable.Repeat(groundPipe, grid[0].Count).ToList ());
    grid.Add(Enumerable.Repeat(groundPipe, grid[0].Count).ToList ());
    return grid.ToList();
}

struct Pipe(Direction[] connectors)
{
    public Direction[] Connectors { get; } = connectors;
    public override string ToString()
    {
        return string.Join(", ", Connectors.Select(t => t.ToString()));
    }
}

enum Direction
{
    North, South, East, West, None, StartingPoint
}
