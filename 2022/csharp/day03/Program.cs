using System;

var backpacks = File.ReadAllLines("input.txt")
   .Select(t => (t.Substring(0, t.Length / 2), t.Substring(t.Length / 2)))
   .Select (t => (ToFrequency(t.Item1), ToFrequency (t.Item2)))
   .ToArray();

Console.WriteLine($"Q1: {backpacks.Sum(backpack => backpack.Item1.Keys.Intersect(backpack.Item2.Keys).Sum(ToPriority))}");

var groups = backpacks.Select((backpack, index) => (index / 3, backpack.Item1.Keys.Union(backpack.Item2.Keys))).GroupBy(t => t.Item1, t=> t.Item2).ToArray ();

Console.WriteLine($"Q2: {groups.Sum(t => t.Aggregate ((a, b) => a.Intersect (b)).Sum(ToPriority))}");

Dictionary<char, int> ToFrequency(string value)
    => value.ToHashSet ().ToDictionary(key => key, key => value.Count(rune => rune == key));

int ToPriority(char value)
    => 1 + (value >= 'a' && value <= 'z' ? (value - 'a') : (value - 'A') + 26);
