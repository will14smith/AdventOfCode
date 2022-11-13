using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2015;

[Day]
public partial class Day19 : ParseDay<Day19.Model, int, int>
{
    private static readonly TextParser<(string, string)> ReplacementParser = SuperpowerExtensions.Name.ThenIgnore(Span.EqualTo(" => ")).Then(SuperpowerExtensions.Name);

    protected override TextParser<Model> Parser => ReplacementParser.ThenIgnore(Span.EqualTo('\n')).Many().ThenIgnore(Span.EqualTo('\n')).Then(SuperpowerExtensions.Name).AtEnd()
        .Select(x => new Model(x.Item2, x.Item1));

    [Sample("H => HO\nH => OH\nO => HH\n\nHOH", 4)]
    [Sample("H => HO\nH => OH\nO => HH\n\nHOHOHO", 7)]
    protected override int Part1(Model input) => Apply(input, input.Start).Count;

    protected override int Part2(Model input)
    {
        // my input was one that wasn't brute force-able using A*
        // so brute force using random replacement ordering instead...
        var rnd = new Random(42);
        
        var reverseModel = input with { Replacements = input.Replacements.Select(x => (x.Replacement, x.Match)).ToArray() };

        var steps = 0;
        var target = input.Start;
        while (target != "e")
        {
            var tmp = target;
            foreach(var (match, replacement) in reverseModel.Replacements)
            {
                var index = target.IndexOf(match, StringComparison.Ordinal);
                if (index == -1)
                {
                    continue;
                }

                target = target[..index] + replacement + target[(index+match.Length)..];
                steps++;
            }


            if (tmp == target)
            {
                target = input.Start;
                steps = 0;
                reverseModel = input with { Replacements = input.Replacements.OrderBy(x => rnd.Next()).ToArray() };
            }
        }
        
        return steps;
    }

    private static IReadOnlySet<string> Apply(Model model, string input)
    {
        var results = new HashSet<string>();

        foreach (var (match, replacement) in model.Replacements)
        {
            var index = 0;
            while (true)
            {
                index = input.IndexOf(match, index, StringComparison.Ordinal);
                if (index == -1)
                {
                    break;
                }

                var result = input[..index] + replacement + input[(index+match.Length)..];
                
                results.Add(result);
                index++;
            }
        }
        
        return results;
    }


    public record Model(string Start, IReadOnlyList<(string Match, string Replacement)> Replacements);
}