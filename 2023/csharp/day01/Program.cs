var lines = File.ReadAllLines("input.txt");

IList<(string word, string number)> replacements = new[]
{
    ( "zero",   "z--0--o" ),
    ( "one",    "o--1--e" ),
    ( "two",    "t--2--o" ),
    ( "three",  "t--3--e" ),
    ( "four",   "f--4--r" ),
    ( "five",   "f--5--e" ),
    ( "six",    "s--6--x" ),
    ( "seven",  "s--7--n" ),
    ( "eight",  "e--8--t" ),
    ( "nine",   "n--9--e" ),
}.AsReadOnly();

static int CalculateCalibrationValue(IEnumerable<string> input, Func<string, string> filter)
    => input
    .Select(filter)
    .Select(t => t.Where(char.IsDigit).ToArray())
    .Select(t => $"{t.First()}{t.Last()}")
    .Select(int.Parse)
    .Sum();


Console.WriteLine($"Q1: {CalculateCalibrationValue(lines, t => t)}");
Console.WriteLine($"Q2: {CalculateCalibrationValue(lines, line =>
{
    foreach (var v in replacements)
        line = line.Replace(v.word, v.number);
    return line;
})}");
