using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2015;

[Day]
public partial class Day09 : ParseLineDay<(string A, string B, int Distance), int, int>
{
    protected override TextParser<(string A, string B, int Distance)> LineParser =>
        from a in Identifier.CStyle
        from _1 in Span.EqualTo(" to ")
        from b in Identifier.CStyle
        from _2 in Span.EqualTo(" = ")
        from distance in Numerics.IntegerInt32
        select (a.ToStringValue(), b.ToStringValue(), distance);

    [Sample("London to Dublin = 464\nLondon to Belfast = 518\nDublin to Belfast = 141", 605)]
    protected override int Part1(IEnumerable<(string A, string B, int Distance)> input) => Solve(input, (x, y) => x < y ? x : y);

    [Sample("London to Dublin = 464\nLondon to Belfast = 518\nDublin to Belfast = 141", 982)]
    protected override int Part2(IEnumerable<(string A, string B, int Distance)> input) => Solve(input, (x, y) => x < y ? y : x);

    private static int Solve(IEnumerable<(string A, string B, int Distance)> input, Func<int, int, int> selectSolution)
    {
        var edges = input.ToList();
        var indexedEdges = new Dictionary<(string A, string B), int>();
        edges.ForEach(e => indexedEdges[(e.A, e.B)] = e.Distance);
        edges.ForEach(e => indexedEdges[(e.B, e.A)] = e.Distance);

        var cities = edges.SelectMany(x => new[] { x.A, x.B }).Distinct().ToList();
        var permutations = Permutations.Get(cities, cities.Count).ToList();

        int? solution = null;
        foreach (var permutation in permutations)
        {
            var permutationList = permutation.ToList();

            var distance = 0;
            var valid = true;

            for (var i = 0; i < permutationList.Count - 1; i++)
            {
                var key = (permutationList[i], permutationList[i + 1]);
                if (!indexedEdges.TryGetValue(key, out var dist))
                {
                    valid = false;
                    break;
                }

                distance += dist;
            }

            if (valid)
            {
                solution = solution == null ? distance : selectSolution(solution.Value, distance);
            }
        }

        return solution ?? 0;
    }
}