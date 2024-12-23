using System.Collections.Immutable;

namespace AdventOfCode2024;

[Day]
public partial class Day23 : Day<Day23.Model, int, string>
{
    public record Model(IReadOnlyDictionary<string, ImmutableHashSet<string>> Edges);
    
    protected override Model Parse(string input)
    {
        var edges = new Dictionary<string, ImmutableHashSet<string>>();

        foreach (var line in input.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = line.Split('-', StringSplitOptions.RemoveEmptyEntries);

            if (!edges.TryGetValue(parts[0], out var part0))
            {
                edges[parts[0]] = part0 = [];
            }
            edges[parts[0]] = part0.Add(parts[1]);

            if (!edges.TryGetValue(parts[1], out var part1))
            {
                edges[parts[1]] = part1 = [];
            }
            edges[parts[1]] = part1.Add(parts[0]);
        }
        
        return new Model(edges);
    }

    [Sample("kh-tc\nqp-kh\nde-cg\nka-co\nyn-aq\nqp-ub\ncg-tb\nvc-aq\ntb-ka\nwh-tc\nyn-cg\nkh-ub\nta-co\nde-co\ntc-td\ntb-wq\nwh-td\nta-ka\ntd-qp\naq-cg\nwq-ub\nub-vc\nde-ta\nwq-aq\nwq-vc\nwh-yn\nka-de\nkh-ta\nco-tc\nwh-qp\ntb-vc\ntd-yn\n", 7)]
    protected override int Part1(Model input)
    {
        var triples = new HashSet<string>();
        foreach (var (node, edges) in input.Edges)
        {
            if(!node.StartsWith('t')) continue;
            
            foreach (var pair in Combinations.Get(edges.ToList(), 2))
            {
                if (input.Edges[pair.Span[0]].Contains(pair.Span[1]))
                {
                    triples.Add(string.Join("-", new List<string>([node, pair.Span[0], pair.Span[1]]).OrderBy(x => x)));
                }
            }
        }
        
        return triples.Count;
    }

    [Sample("kh-tc\nqp-kh\nde-cg\nka-co\nyn-aq\nqp-ub\ncg-tb\nvc-aq\ntb-ka\nwh-tc\nyn-cg\nkh-ub\nta-co\nde-co\ntc-td\ntb-wq\nwh-td\nta-ka\ntd-qp\naq-cg\nwq-ub\nub-vc\nde-ta\nwq-aq\nwq-vc\nwh-yn\nka-de\nkh-ta\nco-tc\nwh-qp\ntb-vc\ntd-yn\n", "co,de,ka,ta")]
    protected override string Part2(Model input) => 
        string.Join(",", input.Edges.Keys.Select(x => BiggestGroup(input.Edges, [x], input.Edges[x])).MaxBy(x => x.Count)?.OrderBy(x => x) ?? throw new InvalidOperationException());

    private static ImmutableHashSet<string> BiggestGroup(IReadOnlyDictionary<string, ImmutableHashSet<string>> edges, ImmutableHashSet<string> group, ImmutableHashSet<string> candidates)
    {
        var biggest = group;

        foreach (var candidate in candidates)
        {
            // if the current biggest already contains this candidate
            // then a previous sub-search must've already tried it
            // so no point trying again since it'll find the same group. 
            if(biggest.Contains(candidate)) continue;
            
            var attempt = group.Add(candidate);
            
            var attemptResult = BiggestGroup(edges, attempt, candidates.Intersect(edges[candidate]));
            if (attemptResult.Count > biggest.Count)
            {
                biggest = attemptResult;
            }
        }
        
        return biggest;
    }
}