using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Day19
{
    class Program
    {
        static void Main (string[] args)
        {
            var lines = File.ReadAllLines ("input.txt");
            var towels = lines.First ().Split (", ").Select (t => t.AsMemory ()).ToArray ();
            var patterns = lines.Skip (2).ToArray ();

            Console.WriteLine ($"Q1 {patterns.Count (t => {
                return CanMake (t.AsMemory (), towels);
            })}");
        }

        static bool CanMake (ReadOnlyMemory<char> pattern, ReadOnlyMemory<char>[] towels)
        {
            var uniqueCombos = towels.SelectMany (t => t.ToArray ()).ToHashSet ();
            foreach (var letter in pattern.Span)
                if (!uniqueCombos.Contains (letter))
                    return false;

            return CanMakeRecursive (pattern, pattern, towels);
        }


        static readonly Dictionary<ReadOnlyMemory<char>, bool> cache = new Dictionary<ReadOnlyMemory<char>, bool> (new MemoryComparer ());
        static readonly Dictionary<ReadOnlyMemory<char>, int> counts = new Dictionary<ReadOnlyMemory<char>, int> ();
        static bool CanMakeRecursive (ReadOnlyMemory<char> fullPattern, ReadOnlyMemory<char> remainingPattern, ReadOnlyMemory<char>[] towels)
        {
            if (remainingPattern.Length == 0) {
                if (!counts.TryGetValue (fullPattern, out int count))
                    count = 0;
                counts[fullPattern] = count + 1;
                return true;
            }

            if (cache.TryGetValue (remainingPattern, out var cached))
                return cached;

            for (int i = 0; i < towels.Length; i ++) {
                if (remainingPattern.Span.StartsWith (towels[i].Span)) {
                    if (CanMakeRecursive (fullPattern, remainingPattern.Slice (towels[i].Span.Length), towels)) {
                        cache.Add (remainingPattern, true);
                        return true;
                    }
                }
            }
            cache.Add (remainingPattern, false);
            return false;
        }

        class MemoryComparer : IEqualityComparer<ReadOnlyMemory<char>>
        {
            public bool Equals (ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
                => x.Span.SequenceEqual (y.Span);

            public int GetHashCode ([DisallowNull] ReadOnlyMemory<char> obj)
                => string.GetHashCode (obj.Span);
        }
    }
}
