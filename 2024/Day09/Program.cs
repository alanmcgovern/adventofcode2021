
namespace Day09
{
    class Program
    {
        class Node
        {
            /// <summary>
            /// id -1 means it's empty space
            /// </summary>
            public int FileId { get; set; }
            public int Size { get; set; }
        }


        static List<int> ToBlocks (IEnumerable<Node> nodes)
        {
            var blocks = new List<int> ();
            foreach (var file in nodes)
                blocks.AddRange (Enumerable.Repeat (file.FileId, file.Size));
            return blocks;
        }

        static long CalculateChecksum (List<int> blocks)
        {
            var checksums = new Dictionary<int, long> ();
            for (int i = 0; i < blocks.Count; i++) {
                if (blocks[i] == -1)
                    continue;

                if (!checksums.TryGetValue (blocks[i], out long checksum))
                    checksum = 0;
                checksums[blocks[i]] = checksum + blocks[i] * i;
            }
            return checksums.Values.Sum ();
        }

        static void Main (string[] args)
        {
            var input = File.ReadAllLines ("input.txt").First ()
                .ToArray ()
                .Select ((val, index) => new Node { FileId = index % 2 == 0 ? index / 2 : -1, Size = int.Parse (val.ToString ()) });

            var blocks = ToBlocks (input);

            int NextEmpty (int startIndex)
            {
                for (int i = startIndex; i < blocks.Count; i++)
                    if (blocks[i] == -1)
                        return i;
                return -1;
            }

            int PreviousFull (int startIndex)
            {
                for (int j = startIndex; j >= 0; j--)
                    if (blocks[j] != -1)
                        return j;
                return -1;
            }

            for (int i = NextEmpty (0), j = PreviousFull (blocks.Count - 1); i < j; i = NextEmpty (i), j = PreviousFull (j))
                (blocks[i], blocks[j]) = (blocks[j], blocks[i]);

            Console.WriteLine ($"Q1 {CalculateChecksum (blocks)}");


            // Uuuuhh, it's 24 hours later. Ain't nobody got time to remember what i wrote
            // yesterday. However, my original plan for this was to defrag. Now we're gonna
            // defrag.
            var defragging = new List<Node> (input);

            // Try each file once.
            for (int source = defragging.Count - 1; source >= 0;) {
                var sourceFile = defragging[source];
                // We can't defrag empty space
                if (sourceFile.FileId == -1) {
                    source--;
                    continue;
                }

                for (int dest = 0; dest < source; dest++) {
                    var destFile = defragging[dest];
                    if (defragging[dest].FileId != -1 || defragging[dest].Size < sourceFile.Size)
                        continue;

                    // Remove the file and replace it with empty space
                    defragging.RemoveAt (source);
                    defragging.Insert (source, new Node { FileId = -1, Size = sourceFile.Size });

                    // Insert the file into the earlier position
                    defragging.Insert (dest, sourceFile);
                    // Shrink the remaining space.
                    defragging[dest + 1] = new Node { FileId = -1, Size = destFile.Size - sourceFile.Size };
                    break;
                }

                // If we can't move the file, move to the next file.
                if (defragging[source] == sourceFile)
                    source--;
            }

            Console.WriteLine ($"Q2 {CalculateChecksum (ToBlocks (defragging))}");
        }
    }
}
