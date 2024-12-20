using System.Collections.ObjectModel;
using System.ComponentModel.Design;
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

            var counts = new Dictionary<ReadOnlyMemory<char>, long> (new MemoryComparer ());
            var madePatterns = patterns.Select (t => CanMakeRecursive (t.AsMemory (), towels, counts)).ToArray ();
            Console.WriteLine ($"Q1 {madePatterns.Where (t => t > 0).Count ()}");
            Console.WriteLine ($"Q2 {madePatterns.Sum (t => t)}");
        }

        static long CanMakeRecursive (ReadOnlyMemory<char> remainingPattern, ReadOnlyMemory<char>[] towels, Dictionary<ReadOnlyMemory<char>, long> counts)
        {
            // We matched everything. Success!
            if (remainingPattern.Length == 0)
                return 1;

            // We matched this previously - return previous count.
            if (counts.TryGetValue (remainingPattern, out var cached))
                return cached;

            // If any towel matches, recurse and count the ways it matches.
            for (int i = 0; i < towels.Length; i++) {
                if (remainingPattern.Span.StartsWith (towels[i].Span)) {
                    var count = CanMakeRecursive (remainingPattern.Slice (towels[i].Span.Length), towels, counts);
                    counts[remainingPattern] = counts.GetValueOrDefault (remainingPattern, 0) + count;
                }
            }

            // If none of the towels matched the remaining pattern, mark it as dead.
            if (!counts.TryGetValue (remainingPattern, out var val))
                counts[remainingPattern] = 0;
            return val;
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
