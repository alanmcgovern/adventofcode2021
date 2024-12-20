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

            var madePatterns = patterns.Select (t => CanMakeRecursive (t.AsMemory (), towels)).ToArray ();
            Console.WriteLine ($"Q1 {madePatterns.Where (t => t.canMake).Count ()}");
            Console.WriteLine ($"Q2 {madePatterns.Sum (t => t.count)}");
        }

        static readonly Dictionary<ReadOnlyMemory<char>, bool> cache = new Dictionary<ReadOnlyMemory<char>, bool> (new MemoryComparer ());
        static readonly Dictionary<ReadOnlyMemory<char>, long> counts = new Dictionary<ReadOnlyMemory<char>, long> (new MemoryComparer ());
        static (bool canMake, long count) CanMakeRecursive (ReadOnlyMemory<char> remainingPattern, ReadOnlyMemory<char>[] towels)
        {
            if (cache.TryGetValue (remainingPattern, out var cached))
                return (cached, counts[remainingPattern]);

            if (remainingPattern.Length == 0)
                return (true, 1);

            for (int i = 0; i < towels.Length; i++) {
                if (remainingPattern.Span.StartsWith (towels[i].Span)) {
                    var (canMake, count) = CanMakeRecursive (remainingPattern.Slice (towels[i].Span.Length), towels);
                    cache[remainingPattern] = canMake;
                    counts[remainingPattern] = counts.GetValueOrDefault (remainingPattern, 0) + count;
                }
            }

            if (!counts.TryGetValue (remainingPattern, out var val)) {
                cache[remainingPattern] = false;
                counts[remainingPattern] = 0;
            }
            return (cache[remainingPattern], counts[remainingPattern]);
        }

        class MemoryComparer : IEqualityComparer<ReadOnlyMemory<char>>
        {
            public bool Equals (ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
                => x.Span.SequenceEqual (y.Span);

            public int GetHashCode ([DisallowNull] ReadOnlyMemory<char> obj)
                => string.GetHashCode (obj.Span, StringComparison.Ordinal);
        }
    }
}
