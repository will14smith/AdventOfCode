using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2023;

[Day]
public partial class Day02 : ParseLineDay<Day02.Model, int, int>
{
    public static readonly TextParser<Colour> ColourParser =
        Span.EqualTo("red").Select(_ => Colour.Red)
            .Or(Span.EqualTo("green").Select(_ => Colour.Green))
            .Or(Span.EqualTo("blue").Select(_ => Colour.Blue));
    public static readonly TextParser<Information> InformationParser =
        Numerics.IntegerInt32.ThenIgnore(Span.WhiteSpace).Then(ColourParser).ManyDelimitedBy(Span.EqualTo(", ")).Select(x => new Information(x.ToDictionary(y => y.Item2, y => y.Item1)));
    public static readonly TextParser<Model> GameParser =
        Span.EqualTo("Game ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(": ")).Then(InformationParser.ManyDelimitedBy(Span.EqualTo("; "))).Select(x => new Model(x.Item1, x.Item2));

    protected override TextParser<Model> LineParser => GameParser;

    [Sample("Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green\nGame 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue\nGame 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red\nGame 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red\nGame 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green", 8)]
    protected override int Part1(IEnumerable<Model> input) => input
        .Select(game => (game.Id, Max: MaxOfEachColour(game)))
        .Where(t => t.Max[Colour.Red] <= 12 && t.Max[Colour.Green] <= 13 && t.Max[Colour.Blue] <= 14)
        .Sum(t => t.Id);

    [Sample("Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green\nGame 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue\nGame 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red\nGame 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red\nGame 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green", 2286)]
    protected override int Part2(IEnumerable<Model> input) => input
        .Select(MaxOfEachColour)
        .Select(max => max[Colour.Red] * max[Colour.Green] * max[Colour.Blue])
        .Sum();

    private static IReadOnlyDictionary<Colour, int> MaxOfEachColour(Model game) =>
        game.Information
            .SelectMany(x => x.Counts)
            .GroupBy(x => x.Key, x => x.Value, (colour, values) => (Colour: colour, Max: values.Max()))
            .ToDictionary(x => x.Colour, x => x.Max);

    public record Model(int Id, IReadOnlyList<Information> Information);
    public record Information(IReadOnlyDictionary<Colour, int> Counts);

    public enum Colour
    {
        Red,
        Green,
        Blue,
    }
}