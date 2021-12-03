// Question 1
var results = File.ReadAllLines("input.txt")
    .Select(s => s.Select(t => t == '0' ? 0 : 1).ToArray())
    .ToArray ();

var highBits = new int[results[0].Length];
for (int i = 0; i < highBits.Length; i++)
    highBits[i] = results.Select(t => t[i]).Sum();

var gamma = new string(highBits.Select(t => t >= results.Length / 2 ? '1' : '0').ToArray());
var epsilon = new string(highBits.Select(t => t > results.Length / 2 ? '0' : '1').ToArray());

Console.WriteLine($"Q1 answer: {Convert.ToInt32(gamma, 2) * Convert.ToInt32(epsilon, 2)}");


// Question 2
var oxygenSelector = results;
for (int i = 0; i < highBits.Length && oxygenSelector.Length > 1; i++)
{
    var mostCommon = oxygenSelector.Select(t => t[i]).Sum() * 2 >= oxygenSelector.Length ? 1 : 0;
    oxygenSelector = oxygenSelector.Where(t => t[i] == mostCommon).ToArray();
}

var scrubberSelector = results;
for (int i = 0; i < highBits.Length && scrubberSelector.Length > 1; i++)
{
    var leastCommon = scrubberSelector.Select(t => t[i]).Sum() * 2 < scrubberSelector.Length ? 1 : 0;
    scrubberSelector = scrubberSelector.Where(t => t[i] == leastCommon).ToArray();
}
var oxygen = new string(oxygenSelector.Single ().Select(t => t== 0 ? '0' : '1').ToArray());
var scrubber = new string(scrubberSelector.Single ().Select(t => t == 0 ? '0' : '1').ToArray());

Console.WriteLine($"Q2 answer: {Convert.ToInt32(oxygen, 2) * Convert.ToInt32(scrubber, 2)}");