
var rounds = File.ReadAllLines("input.txt")
   .Select(t => t.Split(' '))
   .Select(t => t.Select(Enum.Parse<Shape>).ToArray())
   .Select(t => ((Shape them, Shape me))(t[0], t[1]))
   .ToArray();

var q1ScoringForMatch = new Dictionary<(Shape, Shape), int>
{
    {(Shape.Rock, Shape.Rock), 3 },
    {(Shape.Paper, Shape.Paper), 3 },
    {(Shape.Scissors, Shape.Scissors), 3 },
    {(Shape.Scissors, Shape.Rock), 6 },
    {(Shape.Rock, Shape.Paper), 6 },
    {(Shape.Paper, Shape.Scissors), 6 },
};

var q2ScoringForMatch = new Dictionary<Shape, int>
{
    { Shape.Lose, 0 },
    { Shape.Draw, 3 },
    { Shape.Win, 6 },
};

var q2ScoringForChosenShape = new Dictionary<(Shape, Shape), int>
{
    {(Shape.Rock, Shape.Win), 2 },
    {(Shape.Rock, Shape.Draw), 1 },
    {(Shape.Rock, Shape.Lose), 3 },

    {(Shape.Paper, Shape.Win), 3 },
    {(Shape.Paper, Shape.Draw), 2 },
    {(Shape.Paper, Shape.Lose), 1 },

    {(Shape.Scissors, Shape.Win), 1 },
    {(Shape.Scissors, Shape.Draw), 3 },
    {(Shape.Scissors, Shape.Lose), 2 },
};

Console.WriteLine($"Q1: {rounds.Sum(t => (int)t.me + q1ScoringForMatch.GetValueOrDefault(t, 0))}");
Console.WriteLine($"Q2: {rounds.Sum(t => q2ScoringForMatch[t.me] + q2ScoringForChosenShape[t])}");

enum Shape
{
    // "Me"
    X = 1,
    Y = 2,
    Z = 3,

    // "Them"
    A = X,
    B = Y,
    C = Z,

    // "easy names" for both hands in q1
    Rock = X,
    Paper = Y,
    Scissors = Z,

    // "easy names" for the desired outcome in q2
    Lose = X,
    Draw = Y,
    Win = Z
}
