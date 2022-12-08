
var input = File.ReadAllLines("input.txt")
    .Select(line => line.Select(t => new Tree(t - '0')).ToArray())
    .ToArray();

// Convert the input grid into four views, corresponding to viewing the 'trees' from each viewpoint.
//
// look left -> right
var leftRights = input;
// looking right -> left
var rightLefts = input.Select(t => t.Reverse());
// looking top -> bottom
var topBottoms = Enumerable.Range(0, input[0].Length).Select(col => Enumerable.Range(0, input.Length).Select(row => input[row][col]));
// looking bottom -> top
var bottomTops = topBottoms.Select(t => t.Reverse());

var allViewPoints = new[] { leftRights, rightLefts, topBottoms, bottomTops };

foreach (var grid in allViewPoints)
{
    foreach (var row in grid)
    {
        int largestSeen = -1;
        foreach (var tree in row)
        {
            if (tree.Size > largestSeen)
            {
                tree.SetVisible();
                largestSeen = tree.Size;
            }
        }
    }
}
Console.WriteLine($"Q1: {input.SelectMany(t => t).Count(t => t.Visible)}");

Dictionary<Tree, int> ScenicScores = new Dictionary<Tree, int>();
foreach (var grid in allViewPoints)
{
    foreach (var row in grid.Select(t => t.ToArray()))
    {
        for (int i = 0; i < row.Length; i++)
        {
            int scenicScore = 0;
            for (int j = i + 1; j < row.Length; j++)
            {
                scenicScore++;
                if (row[j].Size >= row[i].Size)
                    break;
            }
            ScenicScores[row[i]] = scenicScore * ScenicScores.GetValueOrDefault(row[i], 1);
        }
    }
}
Console.WriteLine($"Q2: {ScenicScores.Values.Max ()}");

class Tree
{
    public int Size { get; }
    public bool Visible { get; private set; }

    public Tree(int size)
        => Size = size;

    public void SetVisible()
        => Visible = true;
}