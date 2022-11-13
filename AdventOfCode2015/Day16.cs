using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2015;

[Day]
public partial class Day16 : ParseLineDay<Day16.Model, int, int>
{
    private static readonly IReadOnlyDictionary<string, int> KnownAttributes = new Dictionary<string, int>
    {
        { "children", 3 },
        { "cats", 7 },
        { "samoyeds", 2 },
        { "pomeranians", 3 },
        { "akitas", 0 },
        { "vizslas", 0 },
        { "goldfish", 5 },
        { "trees", 3 },
        { "cars", 2 },
        { "perfumes", 1 },
    };
    private static readonly IReadOnlySet<string> RetroEncabulatorGreater = new HashSet<string> { "cats", "tree" };
    private static readonly IReadOnlySet<string> RetroEncabulatorFewer = new HashSet<string> { "pomeranians", "goldfish" };

    private static readonly TextParser<(string, int)> AttributeParser = SuperpowerExtensions.Name.ThenIgnore(Span.EqualTo(": ")).Then(Numerics.IntegerInt32);
    protected override TextParser<Model> LineParser => Span.EqualTo("Sue ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(": ")).Then(AttributeParser.ManyDelimitedBy(Span.EqualTo(", ")))
        .Select(x => new Model(x.Item1, x.Item2.ToDictionary(e => e.Item1, e => e.Item2)));

    protected override int Part1(IEnumerable<Model> input) => input.Single(x => IsPartialMatch(KnownAttributes, x.Attributes)).Number;

    protected override int Part2(IEnumerable<Model> input) => input.Single(x => IsPartialMatchRetroEncabulator(KnownAttributes, x.Attributes)).Number;
    
    private static bool IsPartialMatch(IReadOnlyDictionary<string, int> known, IReadOnlyDictionary<string, int> partial)
    {
        foreach (var (partialAttribute, partialValue) in partial)
        {
            if (known[partialAttribute] != partialValue) return false;
        }

        return true;
    }

    private static bool IsPartialMatchRetroEncabulator(IReadOnlyDictionary<string, int> known, IReadOnlyDictionary<string, int> partial)
    {
        foreach (var (partialAttribute, partialValue) in partial)
        {
            if (RetroEncabulatorGreater.Contains(partialAttribute))
            {
                if (known[partialAttribute] >= partialValue) return false;
            }
            else if (RetroEncabulatorFewer.Contains(partialAttribute))
            {
                if (known[partialAttribute] <= partialValue) return false;
            }
            else
            {
                if (known[partialAttribute] != partialValue) return false;
            }
        }

        return true;
    }

    
    public record Model(int Number, IReadOnlyDictionary<string, int> Attributes);
}