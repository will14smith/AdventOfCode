using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2015;

[Day]
public partial class Day13 : ParseLineDay<Day13.Model, int, int>
{
    private static readonly TextParser<string> Name = Identifier.CStyle.Select(x => x.ToStringValue());
    private static readonly TextParser<int> Direction = Span.EqualTo(" would ").IgnoreThen(Span.EqualTo("gain").Select(x => 1).Or(Span.EqualTo("lose").Select(x => -1))).ThenIgnore(Span.EqualTo(" "));
    protected override TextParser<Model> LineParser => Name.Then(Direction).Then(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" happiness units by sitting next to ")).Then(Name).ThenIgnore(Span.EqualTo("."))
        .Select(x => new Model(x.Item1.Item1.Item1, x.Item2, x.Item1.Item1.Item2 * x.Item1.Item2));

    [Sample("Alice would gain 54 happiness units by sitting next to Bob.\nAlice would lose 79 happiness units by sitting next to Carol.\nAlice would lose 2 happiness units by sitting next to David.\nBob would gain 83 happiness units by sitting next to Alice.\nBob would lose 7 happiness units by sitting next to Carol.\nBob would lose 63 happiness units by sitting next to David.\nCarol would lose 62 happiness units by sitting next to Alice.\nCarol would gain 60 happiness units by sitting next to Bob.\nCarol would gain 55 happiness units by sitting next to David.\nDavid would gain 46 happiness units by sitting next to Alice.\nDavid would lose 7 happiness units by sitting next to Bob.\nDavid would gain 41 happiness units by sitting next to Carol.", 330)]
    protected override int Part1(IEnumerable<Model> input) => Solve(input);
    
    protected override int Part2(IEnumerable<Model> input)
    {
        var edges = input.ToList();
        var people = edges.SelectMany(x => new[] { x.A, x.B }).ToHashSet();

        foreach (var a in people)
        {
            edges.Add(new Model(a, "Me", 0));
            edges.Add(new Model("Me", a, 0));
        }
        
        return Solve(edges);
    }
    
    private static int Solve(IEnumerable<Model> input)
    {
        var edges = input.ToList();
        var people = edges.SelectMany(x => new[] { x.A, x.B }).ToHashSet();

        var indexedEdges = new Dictionary<(string A, string B), int>();
        edges.ForEach(e => indexedEdges[(e.A, e.B)] = e.Gain);
        edges.ForEach(e => indexedEdges[(e.B, e.A)] = e.Gain);

        var permutations = Permutations.Get(people);

        int? solution = null;
        foreach (var permutation in permutations)
        {
            var permutationList = permutation.ToList();

            var gain = 0;
            var valid = true;

            for (var i = 0; i < permutationList.Count; i++)
            {
                var a = permutationList[i];
                var b = permutationList[(i + 1) % permutationList.Count];

                var key = (a, b);
                if (!indexedEdges.TryGetValue(key, out var g))
                {
                    valid = false;
                    break;
                }

                gain += g;

                key = (b, a);
                if (!indexedEdges.TryGetValue(key, out g))
                {
                    valid = false;
                    break;
                }

                gain += g;
            }

            if (valid)
            {
                solution = solution == null ? gain : solution.Value < gain ? gain : solution.Value;
            }
        }

        return solution ?? 0;
    }

    public record Model(string A, string B, int Gain);
}