using System.Linq;

namespace Program02
{
    class Program
    {
        static void Main (string[] args)
        {
            var reports = File.ReadAllLines ("input.txt")
                .Select (t => t.Split (' ').Select (long.Parse).ToArray ());

            static int DirectionComparator (long a, long b) => Math.Clamp (a.CompareTo (b), -1, 1);
            static bool SafeReportCounter (long[] report)
            {
                var comparison = DirectionComparator (report[0], report[1]);
                return report.Zip (report.Skip (1)).All (pair => DirectionComparator (pair.First, pair.Second) == comparison && Math.Abs (pair.First - pair.Second) >= 1 && Math.Abs (pair.First - pair.Second) <= 3);
            }
            Console.WriteLine ($"Q1: {reports.Count (SafeReportCounter)}");

            static IEnumerable<long[]> ReportExpander (long[] report)
            {
                for (int i = 0; i < report.Length; i++)
                    yield return report.Where ((value, index) => index != i).ToArray ();
            }
            Console.WriteLine ($"Q2: {reports.Count (report => ReportExpander (report).Any (SafeReportCounter))}");
        }
    }
}
