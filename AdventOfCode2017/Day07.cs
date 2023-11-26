using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2017;

[Day]
public partial class Day07 : ParseLineDay<Day07.Model, string, int>
{
    private static readonly TextParser<string> ProgramName = Character.Letter.Many().Select(x => new string(x));
    private static readonly TextParser<int> ProgramWeight = Span.EqualTo(" (").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Character.EqualTo(')'));
    private static readonly TextParser<string[]> ProgramChildren = Span.EqualTo( " -> ").IgnoreThen(ProgramName.ManyDelimitedBy(Span.EqualTo(", ")));

    protected override TextParser<Model> LineParser { get; } =
        from name in ProgramName
        from weight in ProgramWeight
        from children in ProgramChildren.OptionalOrDefault(Array.Empty<string>())
        select new Model(name, weight, children);

    [Sample("pbga (66)\nxhth (57)\nebii (61)\nhavc (66)\nktlj (57)\nfwft (72) -> ktlj, cntj, xhth\nqoyq (66)\npadx (45) -> pbga, havc, qoyq\ntknk (41) -> ugml, padx, fwft\njptl (61)\nugml (68) -> gyxo, ebii, jptl\ngyxo (61)\ncntj (57)", "tknk")]
    protected override string Part1(IEnumerable<Model> input) => FindRoot(input);

    [Sample("pbga (66)\nxhth (57)\nebii (61)\nhavc (66)\nktlj (57)\nfwft (72) -> ktlj, cntj, xhth\nqoyq (66)\npadx (45) -> pbga, havc, qoyq\ntknk (41) -> ugml, padx, fwft\njptl (61)\nugml (68) -> gyxo, ebii, jptl\ngyxo (61)\ncntj (57)", 60)]
    protected override int Part2(IEnumerable<Model> input)
    {
        var indexed = input.ToDictionary(x => x.Name, x => x);
        
        var totalWeights = CalculateTotalProgramWeights(input, indexed);
        var node = FindRoot(input);

        // find lowest unbalanced node
        var offset = 0;
        while(true)
        {
            var program = indexed[node];

            var childWeights = program.Children.Select(x => (Name: x, TotalWeight: totalWeights[x])).OrderBy(x => x.Item2).ToList();
            var sameWeight = childWeights[1].TotalWeight;

            var firstChild = childWeights[0];
            
            if (sameWeight != firstChild.TotalWeight)
            {
                node = firstChild.Name;
                offset = firstChild.TotalWeight - sameWeight;
            } 
            else
            {
                var lastChild = childWeights[^1];
                if (sameWeight != lastChild.TotalWeight)
                {
                    node = lastChild.Name;
                    offset = lastChild.TotalWeight - sameWeight;
                }
                else
                {
                    return indexed[node].Weight - offset;
                }
            }
        }
    }

    private static string FindRoot(IEnumerable<Model> input)
    {
        var names = input.Select(x => x.Name).ToHashSet();
        var referenced = input.SelectMany(x => x.Children).ToHashSet();
        names.ExceptWith(referenced);
        var node = names.Single();
        return node;
    }

    private static Dictionary<string, int> CalculateTotalProgramWeights(IEnumerable<Model> input, Dictionary<string, Model> indexed)
    {
        var totalWeights = new Dictionary<string, int>();

        var remaining = input.Select(x => x.Name).ToHashSet();
        var queue = new Queue<string>(remaining);

        while (queue.Count > 0)
        {
            var item = queue.Dequeue();
            var program = indexed[item];

            if (program.Children.All(x => !remaining.Contains(x)))
            {
                totalWeights[item] = program.Weight + program.Children.Sum(x => totalWeights[x]);
                remaining.Remove(item);
            }
            else
            {
                queue.Enqueue(item);
            }
        }

        return totalWeights;
    }

    public record Model(string Name, int Weight, IReadOnlyList<string> Children);
}
