
using System.Drawing;
using System.Text.RegularExpressions;

// Ensure there's always an empty line at the end
var input = File.ReadLines("input.txt")
    .Select(t => t.Trim())
    .Concat(new[] { Environment.NewLine });

Console.WriteLine($"Q1: {ProcessRounds(20, 3).OrderByDescending(t => t.InspectionCount).Take(2).Select(t => t.InspectionCount).Aggregate((left, right) => left * right)}");
Console.WriteLine($"Q2: {ProcessRounds(10000, 1).OrderByDescending (t => t.InspectionCount).Take(2).Select(t => t.InspectionCount).Aggregate((left, right) => left * right)}");

IList<Monkey> ProcessRounds(int count, long reliefReductionRate)
{
    var monkeys = ParseMonkeys(input);
    long gcd = monkeys.Select(t => t.TestValue).Aggregate((left, right) => left * right);

    while (count -- > 0)
    {
        foreach (var monkey in monkeys)
        {
            while (monkey.Items.Count > 0)
            {
                var item = monkey.Items.Dequeue();
                monkey.InspectionCount++;
                item.Value = (monkey.NewWorryLevel(item) / reliefReductionRate) % gcd;
                monkeys.Single(t => t.Name == monkey.TestItem(item)).Items.Enqueue(item);
            }
        }
    }

    return monkeys;
}

static IList<Monkey> ParseMonkeys(IEnumerable<string> data)
{
    Monkey? currentMonkey = null;
    List<Monkey> monkeys = new List<Monkey>();

    var nameMatcher = new Regex(@"Monkey (\d*):", RegexOptions.NonBacktracking | RegexOptions.CultureInvariant);
    var startingItemsMatcher = new Regex(@"Starting items: ([,0-9 ]*)", RegexOptions.NonBacktracking | RegexOptions.CultureInvariant);
    var operationMatcher = new Regex(@"Operation: new = (.+) (.+) (.+)", RegexOptions.NonBacktracking | RegexOptions.CultureInvariant);
    var testMatcher = new Regex(@"Test: divisible by (\d*)", RegexOptions.NonBacktracking | RegexOptions.CultureInvariant);
    var ifTrue = new Regex(@"If true: throw to monkey (\d*)", RegexOptions.NonBacktracking | RegexOptions.CultureInvariant);
    var ifFalse = new Regex(@"If false: throw to monkey (\d*)", RegexOptions.NonBacktracking | RegexOptions.CultureInvariant);

    long divisibility = 0;
    string? ifTrueMonkey = null;
    string? ifFalseMonkey = null;
    foreach (var line in data)
    {
        Match match;
        if (string.IsNullOrEmpty(line.Trim()) && currentMonkey is not null)
        {
            var closure = (divisibility, ifTrueMonkey, ifFalseMonkey);
            currentMonkey.TestItem = item => item.Value % closure.divisibility == 0 ? closure.ifTrueMonkey! : closure.ifFalseMonkey!;
            monkeys.Add(currentMonkey);
            currentMonkey = null;
            divisibility = 0;
            ifTrueMonkey = ifFalseMonkey = null;
        }

        if ((match = nameMatcher.Match(line)).Success)
            currentMonkey = new Monkey(match.Groups[1].Value);

        if (currentMonkey is null)
            continue;

        if ((match = startingItemsMatcher.Match(line)).Success)
            foreach (var item in match.Groups[1].Value.Split(", ").Select(long.Parse).Select(t => new Item { Value = t }))
                currentMonkey.Items.Enqueue(item);

        if ((match = operationMatcher.Match(line)).Success)
        {
            // new = $1 $op $2
            long.TryParse(match.Groups[1].Value, out long parsedParam1);
            long.TryParse(match.Groups[3].Value, out long parsedParam2);
            Func<Item, long> param1 = match.Groups[1].Value == "old" ? t => t.Value : t => parsedParam1;
            Func<Item, long> param2 = match.Groups[3].Value == "old" ? t => t.Value : t => parsedParam2;
            currentMonkey.NewWorryLevel = match.Groups[2].Value switch
            {
                "+" => item => param1(item) + param2(item),
                "-" => item => param1(item) - param2(item),
                "*" => item => param1(item) * param2(item),
                "/" => item => param1(item) / param2(item),
                _ => throw new NotSupportedException()
            };
        }

        if ((match = testMatcher.Match (line)).Success)
            currentMonkey.TestValue = divisibility = long.Parse (match.Groups[1].Value);
        if ((match = ifTrue.Match(line)).Success)
            ifTrueMonkey = match.Groups[1].Value;
        if ((match = ifFalse.Match(line)).Success)
            ifFalseMonkey = match.Groups[1].Value;
    }

    return monkeys;
}


class Monkey
{
    public string Name { get; }
    public long InspectionCount { get; set; }
    public Queue<Item> Items { get; set; } = new Queue<Item>();
    public Func<Item, long> NewWorryLevel { get; set; } = t => throw new InvalidOperationException();
    public Func<Item, string> TestItem { get; set; } = t => "";
    public long TestValue { get; set; }

    public Monkey(string name)
        => Name = name;

}

class Item
{
    public long Value { get; set; }
}