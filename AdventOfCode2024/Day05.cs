namespace AdventOfCode2024;

[Day]
public partial class Day05 : Day<Day05.Model, int, int>
{
    private const string Sample = "47|53\n97|13\n97|61\n97|47\n75|29\n61|13\n75|53\n29|13\n97|29\n53|29\n61|53\n97|53\n61|29\n47|13\n75|47\n97|75\n47|61\n75|61\n47|29\n75|13\n53|13\n\n75,47,61,53,29\n97,61,53,29,13\n75,29,13\n75,97,47,61,53\n61,13,29\n97,13,75,29,47";
    
    public record Model(IReadOnlyList<(int A, int B)> Rules, IReadOnlyList<List<int>> Updates);
    
    protected override Model Parse(string input)
    {
        var split = input.Split("\n\n");
        
        var rules = split[0].Split('\n').Select(x => x.Split('|')).Select(x => (int.Parse(x[0]), int.Parse(x[1]))).ToList();
        var pages = split[1].Split('\n').Select(x => x.Split(',').Select(int.Parse).ToList()).ToList();
        
        return new Model(rules, pages);
    }

    [Sample(Sample, 143)]
    protected override int Part1(Model input) => input.Updates.Where(update => IsCorrectlyOrdered(input.Rules, update)).Sum(GetMiddlePage);
    
    [Sample(Sample, 123)]
    protected override int Part2(Model input) => input.Updates.Where(update => !IsCorrectlyOrdered(input.Rules, update)).Select(update => OrderUpdate(input.Rules, update)).Sum(GetMiddlePage);
    
    private bool IsCorrectlyOrdered(IReadOnlyList<(int A, int B)> rules, List<int> update)
    {
        for (var i = 0; i < update.Count; i++)
        {
            var applicableRules = rules.Where(rule => update[i] == rule.A).ToList();
            if (!applicableRules.All(rule => IsCorrectRule(rule, i, update)))
            {
                return false;
            }
        }

        return true;
        
        static bool IsCorrectRule((int A, int B) rule, int aIndex, List<int> update)
        {
            var bIndex = update.IndexOf(rule.B);
            if (bIndex < 0) return true;

            return aIndex < bIndex;
        }
    }

    private static List<int> OrderUpdate(IReadOnlyList<(int A, int B)> rules, List<int> update)
    {
        return update.OrderByDescending(NumberOfPagesAfter).ToList();
        
        int NumberOfPagesAfter(int x) => rules.Where(r => r.A == x).Count(r => update.IndexOf(r.B) >= 0);
    }
    
    private static int GetMiddlePage(IReadOnlyList<int> pages) => pages[(pages.Count - 1) / 2];
}