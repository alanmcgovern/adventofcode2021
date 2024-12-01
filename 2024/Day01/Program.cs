namespace Day01
{
    internal class Program
    {
        static void Main (string[] args)
        {
            var data = File.ReadAllLines ("input.txt")
                .Select (t => t.Split (' ', StringSplitOptions.RemoveEmptyEntries))
                .Select (t => t.Select (long.Parse).ToArray ());

            var list1 = data.Select (t => t[0]).Order ().ToArray ();
            var list2 = data.Select (t => t[1]).Order ().ToArray ();
            Console.WriteLine ($"Q1 {Q1 (list1, list2)}");
            Console.WriteLine ($"Q2 {Q2 (list1, list2)}");
        }

        static long Q1 (long[] list1, long[] list2)
        {
            return list1.Zip (list2, (left, right) => Math.Abs (left - right)).Sum ();
        }

        static long Q2 (long[] list1, long[] list2)
        {
            return list1.Select (v => v * list2.Count (v2 => v2 == v)).Sum ();
        }
    }
}
