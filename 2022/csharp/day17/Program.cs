
using System.Collections.Immutable;

ImmutableArray<ImmutableArray<Point>> ShapeTemplates = new[] {
    // ####
    new Point[] { new(0, 0), new(1, 0), new(2, 0), new(3, 0) }.ToImmutableArray (),

    //  #
    // ###  // Should I elide the interior point so there are fewer points to care about? Sure... why not.
    //  #
    new Point[] {new(0,1), new(1,0), new(1,1), new(1,2), new(2,1) }.ToImmutableArray (),

    //   #
    //   #
    // ###
    new Point[] {new(0,0), new(1,0), new(2,0), new(2,1), new(2,2)}.ToImmutableArray (),

    // #
    // #
    // #
    // #
    new Point[] {new(0,0), new(0,1), new(0,2), new(0,3)}.ToImmutableArray (),

    // ##
    // ##
    new Point[] {new(0,0), new(0,1), new(1,0), new(1,1)}.ToImmutableArray (),
}.ToImmutableArray();

ReadOnlyMemory<char> Jets = File.ReadAllText("input.txt").Trim().AsMemory();

Console.WriteLine($"Q1: {HeightAfter(rockCount: 2022, width: 7)}");


int HeightAfter(int rockCount, int width)
{
    // Pop in a floor of 0.
    var tetrisGrid = new HashSet<Point>(
        Enumerable.Range(0, width).Select(t => new Point(t, 0))
    );
    var templates = new Queue<ImmutableArray<Point>>(ShapeTemplates);
    var jets = new Queue<char>(Jets.ToArray());

    var maxHeightSoFar = 0;
    while (rockCount-- > 0)
    {
        // Take the next shape
        var currentTemplate = templates.Dequeue();
        templates.Enqueue(currentTemplate);

        // Constuct the shape at the correct offset
        var newShape = currentTemplate.ToArray();
        for (int i = 0; i < newShape.Length; i++)
            newShape[i] = newShape[i].Offset(2, maxHeightSoFar + 4); // need 3 empty segments between the existing shapes and the new shape
        maxHeightSoFar = Math.Max(maxHeightSoFar, DropShape(tetrisGrid, newShape, jets, width));
        //PrintTetris(tetrisGrid, width, maxHeightSoFar + 6);
    }

    return maxHeightSoFar;
}

int DropShape(HashSet<Point> tetrisGrid, Point[] shape, Queue<char> jets, int width)
{
    while (true)
    {
        var jet = jets.Dequeue();
        jets.Enqueue(jet);

        Point offset = jet switch
        {
            '<' => new Point(-1, 0),
            '>' => new Point(1, 0),
            _ => throw new NotSupportedException()
        };

        // We don't care if it can't move left or right
        _ = TryMove(tetrisGrid, shape, offset, width);

        // If it can't move down, we add it and have a new max height!
        if (!TryMove (tetrisGrid, shape, new Point (0, -1), width))
        {
            for (int i = 0; i < shape.Length; i++)
                if (!tetrisGrid.Add(shape[i]))
                    throw new InvalidOperationException("Rock already exists here");
            return shape.Max(t => t.Y);
        }
    }
}

bool TryMove(HashSet<Point> tetrisGrid, Point[] shape, Point offset, int width)
{

    bool canMove = true;
    for (int i = 0; i < shape.Length && canMove; i++)
    {
        var dest = shape[i].Offset(offset);
        if (dest.X < 0 || dest.X >= width || tetrisGrid.Contains(dest))
            return false;
    }

    for (int i = 0; i < shape.Length; i++)
        shape[i] = shape[i].Offset(offset);
    return true;
}

void PrintTetris(HashSet<Point> tetrisGrid, int width, int startingHeight)
{
    Console.WriteLine();
    Console.WriteLine();
    while (startingHeight > 0)
    {
        Console.Write('|');
        for (int i = 0; i < width; i++)
        {
            if (tetrisGrid.Contains(new Point(i, startingHeight)))
                Console.Write('#');
            else
                Console.Write('.');
        }
        Console.Write('|');
        Console.WriteLine();
        startingHeight--;
    }

    foreach (var e in Enumerable.Range(0, width + 2))
        Console.Write('-');

}

readonly record struct Point(int X, int Y)
{
    public Point Offset(int x, int y)
        => new(X + x, Y + y);

    public Point Offset(Point offset)
        => new(X + offset.X, Y + offset.Y);
}
