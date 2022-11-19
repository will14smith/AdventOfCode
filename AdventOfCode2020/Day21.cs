namespace AdventOfCode2020;

[Day]
public partial class Day21 : ParseDay<Day21.Model, Day21.TokenType, long, string>
{
    private const string Sample = "mxmxvkd kfcds sqjhc nhms (contains dairy, fish)\ntrh fvjkl sbzzf mxmxvkd (contains dairy)\nsqjhc fvjkl (contains soy)\nsqjhc mxmxvkd sbzzf (contains fish)\n";
    
    [Sample(Sample, 5L)]
    protected override long Part1(Model input)
    {
        var grouped = GroupByAllergen(input.Lines);

        var possibleIngredients = grouped.SelectMany(x => x.Value).ToHashSet();
        var allIngredients = input.Lines.SelectMany(x => x.Ingredients).Where(x => !possibleIngredients.Contains(x)).ToList();

        return allIngredients.Count;
    }

    [Sample(Sample, "mxmxvkd,sqjhc,fvjkl")]
    protected override string Part2(Model input)
    {
        var assigned = new Dictionary<string, string>();
        var grouped = GroupByAllergen(input.Lines);

        while (grouped.Count > 0)
        {
            var singleAssigned = grouped.Where(x => x.Value.Count == 1);
            foreach (var (k, i) in singleAssigned)
            {
                assigned.Add(k, i.Single());
                grouped.Remove(k);
                foreach (var (_, gi) in grouped) gi.RemoveAll(x => x == i.Single());
            }
        }

        return string.Join(",", assigned.OrderBy(x => x.Key).Select(x => x.Value));
    }
    
    private static Dictionary<string, List<string>> GroupByAllergen(IEnumerable<Line> input)
    {
        return input
            .SelectMany(x => x.Allergens.Select(a => (a, x.Ingredients)))
            .GroupBy(x => x.a)
            .ToDictionary(x => x.Key, x => x.Select(l => l.Ingredients).Aggregate((a, i) => a.Intersect(i).ToList()).ToList());
    }

    public record Line(IReadOnlyList<string> Ingredients, IReadOnlyList<string> Allergens) { }
    public record Model(Line[] Lines);
}
