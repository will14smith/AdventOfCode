using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2023;

[Day]
public partial class Day08 : ParseDay<Day08.Model, long, long>
{
    private static readonly TextParser<Direction> DirectionParser = Character.EqualTo('L').Select(_ => Direction.Left).Or(Character.EqualTo('R').Select(_ => Direction.Right));
    private static readonly TextParser<string> NodeParser = Span.Regex("[A-Z0-9]+").Select(x => x.ToStringValue()); 
    private static readonly TextParser<(string, (string, string))> LocationParser = NodeParser.ThenIgnore(Span.EqualTo(" = (")).Then(NodeParser.ThenIgnore(Span.EqualTo(", ")).Then(NodeParser).ThenIgnore(Character.EqualTo(')')));

    protected override TextParser<Model> Parser { get; } = DirectionParser.Many().ThenIgnore(Span.EqualTo("\n\n")).Then(LocationParser.ManyDelimitedBy(Character.EqualTo('\n')))
            .Select(x => new Model(x.Item1, x.Item2.ToDictionary(y => y.Item1, y => y.Item2)));

    [Sample("RL\n\nAAA = (BBB, CCC)\nBBB = (DDD, EEE)\nCCC = (ZZZ, GGG)\nDDD = (DDD, DDD)\nEEE = (EEE, EEE)\nGGG = (GGG, GGG)\nZZZ = (ZZZ, ZZZ)", 2L)]
    [Sample("LLR\n\nAAA = (BBB, BBB)\nBBB = (AAA, ZZZ)\nZZZ = (ZZZ, ZZZ)", 6L)]
    protected override long Part1(Model input)
    {
        var current = "AAA";
        var steps = 0;

        while (current != "ZZZ")
        {
            current = input.Next(current, steps++);
        }

        return steps;
    }

    [Sample("LR\n\n11A = (11B, XXX)\n11B = (XXX, 11Z)\n11Z = (11B, XXX)\n22A = (22B, XXX)\n22B = (22C, 22C)\n22C = (22Z, 22Z)\n22Z = (22B, 22B)\nXXX = (XXX, XXX)", 6L)]
    protected override long Part2(Model input)
    {
        var starts = input.Locations.Keys.Where(x => x.EndsWith("A")).ToArray();
        var steps = starts.Select(x => Solve(input, x)).ToList();

        return steps.Aggregate(NumberExtensions.LowestCommonMultiple);

        static long Solve(Model input, string start)
        {
            var current = start;
            var steps = 0;

            while (!current.EndsWith('Z'))
            {
                current = input.Next(current, steps++);
            }

            return steps;

        }
    }

    public record Model(IReadOnlyList<Direction> Directions, IReadOnlyDictionary<string, (string, string)> Locations)
    {
        public string Next(string current, int steps) => Directions[steps % Directions.Count] == Direction.Left ? Locations[current].Item1 : Locations[current].Item2;
    }

    public enum Direction
    {
        Left,
        Right,
    }
}