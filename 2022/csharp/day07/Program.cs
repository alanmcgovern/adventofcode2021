var input = new Queue<string>(File.ReadLines("input.txt"));

var filesystemPosition = new Stack<Entry>();
while (input.Count > 0)
{
    var line = input.Dequeue();
    if (line[0] == '$')
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts[1] == "cd")
        {
            if (parts[2] == "..")
                filesystemPosition.Pop();
            else
            {
                var subDir = new Entry(parts[2]);
                if (filesystemPosition.Count > 0)
                    filesystemPosition.Peek().Children.Add(subDir);
                filesystemPosition.Push(subDir);
            }
        } else if (parts[1] == "ls")
        {
            while (input.Count > 0 && input.Peek ()[0] != '$')
            {
                var entry = input.Dequeue();
                if (entry.StartsWith("dir"))
                    continue;
                var fileInfo = entry.Split(' ');
                filesystemPosition.Peek().Children.Add(new Entry(fileInfo[1], int.Parse(fileInfo[0])));
            }
        }
    }
}

// Get back to the root node.
while (filesystemPosition.Count > 1)
    filesystemPosition.Pop();

var eligibleEntries = new HashSet<Entry>();
GatherEntries(filesystemPosition.Peek(), entry => entry.Size is null && entry.SubTreeSize() <= 100000, eligibleEntries);
Console.WriteLine($"Q1: {eligibleEntries.Sum(t => t.SubTreeSize())}");

int partitionSize = 70000000;
int desiredFreeSpace = 30000000;
int usedSpace = filesystemPosition.Peek().SubTreeSize();
int amountToDelete = desiredFreeSpace - (partitionSize - usedSpace);
eligibleEntries.Clear();
GatherEntries(filesystemPosition.Peek(), entry => entry.Size is null && entry.SubTreeSize() >= amountToDelete, eligibleEntries);
Console.WriteLine($"Q2: {eligibleEntries.OrderBy(t => t.SubTreeSize()).First ().SubTreeSize ()}");

static void GatherEntries(Entry entry, Predicate<Entry> predicate, HashSet<Entry> collector)
{
    if (predicate(entry))
        collector.Add(entry);
    foreach (var v in entry.Children)
        GatherEntries(v, predicate, collector);
}

record Entry
{
    public List<Entry> Children { get; } = new List<Entry>();
    public string Name { get; }

    public int? Size { get; }

    public int SubTreeSize()
    {
        int count = Size ?? 0;
        foreach (var entry in Children)
            count += entry.SubTreeSize();
        return count;
    }

    public Entry(string name, int? size = null)
        => (Name, Size) = (name, size);
}
