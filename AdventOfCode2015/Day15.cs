using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2015;

[Day]
public partial class Day15 : ParseLineDay<Day15.Model, int, int>
{
    protected override TextParser<Model> LineParser =>
        from name in SuperpowerExtensions.Name
        from _1 in Span.EqualTo(": capacity ") from capacity in Numerics.IntegerInt32
        from _2 in Span.EqualTo(", durability ") from durability in Numerics.IntegerInt32
        from _3 in Span.EqualTo(", flavor ") from flavor in Numerics.IntegerInt32
        from _4 in Span.EqualTo(", texture ") from texture in Numerics.IntegerInt32
        from _5 in Span.EqualTo(", calories ") from calories in Numerics.IntegerInt32
        select new Model(name, capacity, durability, flavor, texture, calories);

    [Sample("Butterscotch: capacity -1, durability -2, flavor 6, texture 3, calories 8\nCinnamon: capacity 2, durability 3, flavor -2, texture -1, calories 3", 62842880)]
    protected override int Part1(IEnumerable<Model> input) => AllRatios(input.ToArray()).Max(CalculateScore);

    [Sample("Butterscotch: capacity -1, durability -2, flavor 6, texture 3, calories 8\nCinnamon: capacity 2, durability 3, flavor -2, texture -1, calories 3", 57600000)]
    protected override int Part2(IEnumerable<Model> input) => AllRatios(input.ToArray()).Where(Is500Calories).Max(CalculateScore);
    
    private static IEnumerable<IReadOnlyCollection<(Model Model, int Ratio)>> AllRatios(ReadOnlySpan<Model> ingredients, int unallocated = 100)
    {
        var allRatios = new List<IReadOnlyCollection<(Model Model, int Ratio)>>();

        if (ingredients.Length == 1)
        {
            allRatios.Add(new [] { (ingredients[0], unallocated) });

            return allRatios;
        }
        
        for (var allocation = 0; allocation < unallocated; allocation++)
        {
            var allSubRatios = AllRatios(ingredients[1..], unallocated - allocation);
            foreach (var subRatios in allSubRatios)
            {
                allRatios.Add(subRatios.Prepend((ingredients[0], allocation)).ToList());
            }
        }
        
        return allRatios;
    }

    private static int CalculateScore(IReadOnlyCollection<(Model Model, int Ratio)> ratios)
    {
        var capacity = ratios.Sum(x => x.Model.Capacity * x.Ratio);
        var durability = ratios.Sum(x => x.Model.Durability * x.Ratio);
        var flavor = ratios.Sum(x => x.Model.Flavor * x.Ratio);
        var texture = ratios.Sum(x => x.Model.Texture * x.Ratio);

        if (capacity < 0 || durability < 0 || flavor < 0 || texture < 0)
        {
            return 0;
        }

        return capacity * durability * flavor * texture;
    }
    
    private static bool Is500Calories(IReadOnlyCollection<(Model Model, int Ratio)> ratios) => ratios.Sum(x => x.Model.Calories * x.Ratio) == 500;

    public record Model(string Name, int Capacity, int Durability, int Flavor, int Texture, int Calories);
}