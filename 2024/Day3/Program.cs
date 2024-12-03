using System.Text.RegularExpressions;
using System.Transactions;

namespace Day3
{
    class Program
    {
        static long Product (Match match) => long.Parse (match.Groups[1].ValueSpan) * long.Parse (match.Groups[2].ValueSpan);

        static void Main (string[] args)
        {
            var input = File.ReadAllText ("input.txt");

            var regex = new Regex ("mul\\((\\d{1,3}),(\\d{1,3})\\)");
            Console.WriteLine ($"Q1 {regex.Matches (input).Sum (Product)}");

            regex = new Regex ("mul\\((\\d{1,3}),(\\d{1,3})\\)|do\\(\\)|don't\\(\\)");

            bool enabled = true;
            long rollingSum = 0;
            foreach (Match match in regex.Matches (input)) {
                if (match.Groups[0].Value == "do()")
                    enabled = true;
                else if (match.Groups[0].Value == "don't()")
                    enabled = false;
                else if (enabled)
                    rollingSum += Product (match);
            }
            Console.WriteLine ($"Q2 {rollingSum}");

        }
    }
}
