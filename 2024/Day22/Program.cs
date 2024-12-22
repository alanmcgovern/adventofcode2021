using System.IO.MemoryMappedFiles;
using System.Security.AccessControl;

namespace Day22
{
    static class Extensions
    {
        public static IEnumerable<(T left, T right)> Pairwise<T> (this IEnumerable<T> enumerable)
        {
            T prev = default!;
            bool hasFirst = false;
            foreach (var v in enumerable) {
                if (hasFirst)
                    yield return (prev, v);
                prev = v;
                hasFirst = true;
            }
        }
    }

    class Program
    {
        static void Main (string[] args)
        {
            var secretSequences = File.ReadAllLines ("input.txt")
                .Select (long.Parse)
                .Select (secret => Iterate (secret, 2000).ToArray ())
                .ToArray ();

            Console.WriteLine ($"Q1: {secretSequences.Select (sequence => sequence.Last ()).Sum ()}");

            // Calculate diffs
            var deltas = secretSequences.Select (sequence =>
                sequence.Pairwise ().Select (val => (val.right % 10) - (val.left % 10)).ToArray ()
            ).ToArray ();

            var bestSequence = new Dictionary<(long a, long b, long c, long d), long> ();
            for (int seq = 0; seq < secretSequences.Length; seq++) {
                var seen = new HashSet<(long a, long b, long c, long d)> ();
                var currentSequence = deltas[seq];
                for (int i = 0; i < currentSequence.Length - 4; i++) {
                    var key = (currentSequence[i], currentSequence[i + 1], currentSequence[i + 2], currentSequence[i + 3]);
                    if (seen.Add (key))
                        bestSequence[key] = bestSequence.GetValueOrDefault (key, 0) + secretSequences[seq][i + 4] % 10;
                }
            }

            Console.WriteLine ($"Q2: {bestSequence.Max (t => t.Value)}");
        }

        static IEnumerable<long> Iterate (long secret, int count)
        {
            // We need to keep the secret in as the first number. We use it for deltas.
            yield return secret;

            while (count--> 0) {
                secret = Prune (Mix (secret, (secret * 64)));
                secret = Prune (Mix (secret, (secret / 32)));
                secret = Prune (Mix (secret, (secret * 2048)));
                yield return secret;
            }
        }

        static long Mix (long secret, long value)
            => secret ^ value;

        static long Prune (long secret)
            => secret % 16777216;
    }
}
