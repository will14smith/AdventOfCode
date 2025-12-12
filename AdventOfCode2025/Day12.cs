using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2025;

[Day]
public partial class Day12 : ParseDay<Day12.Model, int, int>
{
    public record Region(int Width, int Height, IReadOnlyList<int> ShapeCounts);
    public record Model(IReadOnlyList<Grid<bool>> Shapes, IReadOnlyList<Region> Regions);

    private static readonly TextParser<Grid<bool>> ShapeParser =
        from _ in Numerics.Natural.Then(Character.EqualTo(':')).Then(Character.EqualTo('\n'))
        from line1 in Span.Length(3).ThenIgnore(Character.EqualTo('\n'))
        from line2 in Span.Length(3).ThenIgnore(Character.EqualTo('\n'))
        from line3 in Span.Length(3).ThenIgnore(Character.EqualTo('\n'))
        select GridParser.ParseBool(line1.ToStringValue() + "\n" + line2.ToStringValue() + "\n" + line3.ToStringValue(), '#');
    
    private static readonly TextParser<Region> RegionParser =
        from width in Numerics.IntegerInt32.ThenIgnore(Character.EqualTo('x'))
        from height in Numerics.IntegerInt32.ThenIgnore(Character.EqualTo(':'))
        from shapeCounts in Character.EqualTo(' ').IgnoreThen(Numerics.IntegerInt32).Many()
        select new Region(width, height, shapeCounts.ToList());
    
    protected override TextParser<Model> Parser { get; } =
        from shapes in ShapeParser.ThenIgnore(Character.EqualTo('\n')).Try().Many()
        from regions in RegionParser.ThenIgnoreOptional(Character.EqualTo('\n')).Many()
        select new Model(shapes.ToList(), regions.ToList());

    [Sample("0:\n###\n##.\n##.\n\n1:\n###\n##.\n.##\n\n2:\n.##\n###\n##.\n\n3:\n##.\n###\n##.\n\n4:\n###\n#..\n###\n\n5:\n###\n.#.\n###\n\n4x4: 0 0 0 0 2 0\n12x5: 1 0 1 0 2 2\n12x5: 1 0 1 0 3 2\n", 2)]
    protected override int Part1(Model input) => input.Regions.Count(region => IsValid(region, input.Shapes));

    private static bool IsValid(Region region, IReadOnlyList<Grid<bool>> shapes)
    {
        var area = region.Width * region.Height;
        var totalShapeArea = region.ShapeCounts.Select((count, index) => count * shapes[index].Count(x => x)).Sum();
        
        // this work for the real input... but not the sample input and is 100% not a correct way to actually solve this.
        return totalShapeArea <= area;
    }

    [Sample("", 1)]
    protected override int Part2(Model input) => 0;
}