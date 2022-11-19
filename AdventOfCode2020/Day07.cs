using Superpower;

namespace AdventOfCode2020;

[Day]
public partial class Day07 : ParseDay<Day07.Rule[], Day07.Token, int, int>
{
    private const string Sample1 = "light red bags contain 1 bright white bag, 2 muted yellow bags.\ndark orange bags contain 3 bright white bags, 4 muted yellow bags.\nbright white bags contain 1 shiny gold bag.\nmuted yellow bags contain 2 shiny gold bags, 9 faded blue bags.\nshiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.\ndark olive bags contain 3 faded blue bags, 4 dotted black bags.\nvibrant plum bags contain 5 faded blue bags, 6 dotted black bags.\nfaded blue bags contain no other bags.\ndotted black bags contain no other bags.";
    private const string Sample2 = "shiny gold bags contain 2 dark red bags.\ndark red bags contain 2 dark orange bags.\ndark orange bags contain 2 dark yellow bags.\ndark yellow bags contain 2 dark green bags.\ndark green bags contain 2 dark blue bags.\ndark blue bags contain 2 dark violet bags.\ndark violet bags contain no other bags.";
    
    [Sample(Sample1, 4)]
    protected override int Part1(Rule[] input) => CountOuterBags(input, "shiny gold");
    
    [Sample(Sample2, 126)]
    protected override int Part2(Rule[] input) => CountInnerBags(input, "shiny gold");

    private static int CountOuterBags(Rule[] rules, string target)
    {
        var outers = new HashSet<string>();
        var workList = new Queue<string>();

        outers.Add(target);
        workList.Enqueue(target);

        while (workList.Count > 0)
        {
            var bag = workList.Dequeue();

            var outerRules = rules.Where(rule => rule.CanContain(bag));
            foreach (var rule in outerRules)
            {
                if (outers.Add(rule.Outer))
                {
                    workList.Enqueue(rule.Outer);
                }
            }
        }

        // ignore the original target
        return outers.Count - 1;
    }

    private static int CountInnerBags(Rule[] rules, string target)
    {
        var indexedRules = rules.ToDictionary(x => x.Outer);
        return CountInner(indexedRules, target) - 1;

        static int CountInner(IReadOnlyDictionary<string, Rule> rules, string target)
        {
            return 1 + rules[target].Inner.Sum(x => x.Value * CountInner(rules, x.Key));
        }
    }

    public record Rule(string Outer, IReadOnlyDictionary<string, int> Inner)
    {
        public bool CanContain(string bag) => Inner.ContainsKey(bag);
    }
}
