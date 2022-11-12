using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2015;

[Day]
public partial class Day06 : ParseLineDay<Day06.Instruction, int, int>
{
    private static readonly TextParser<Point> PointParser = 
            from x in Numerics.IntegerInt32
            from _ in Character.EqualTo(',')
            from y in Numerics.IntegerInt32
            select new Point(x, y);
    private static readonly TextParser<Region> RegionParser = 
        from tl in PointParser
        from _ in Span.EqualTo(" through ")
        from br in PointParser
        select new Region(tl, br);

    protected override TextParser<Instruction> LineParser { get; } =
        (Span.EqualTo("turn off ").Then(RegionParser).Select(x => (Instruction) new Instruction.TurnOff(x.Item2))).Try()
        .Or(Span.EqualTo("turn on ").Then(RegionParser).Select(x => (Instruction) new Instruction.TurnOn(x.Item2)).Try())
        .Or(Span.EqualTo("toggle ").Then(RegionParser).Select(x => (Instruction) new Instruction.Toggle(x.Item2)));

    [Sample("turn on 0,0 through 999,999", 1_000_000)]
    [Sample("turn on 0,0 through 999,999\ntoggle 0,0 through 999,0", 1_000_000 - 1_000)]
    [Sample("turn on 0,0 through 999,999\ntoggle 0,0 through 999,0\nturn off 499,499 through 500,500", 1_000_000 - 1_000 - 4)]
    protected override int Part1(IEnumerable<Instruction> input)
    {
        var lights = new int[1_000_000];

        foreach (var instruction in input)
        {
            switch (instruction)
            {
                case Instruction.TurnOn turnOn: ApplyToRegion(lights, turnOn.Region, _ => 1); break;
                case Instruction.Toggle toggle: ApplyToRegion(lights, toggle.Region, x => x > 0 ? 0 : 1); break;
                case Instruction.TurnOff turnOff: ApplyToRegion(lights, turnOff.Region, _ => 0); break;
            }
        }
        
        return lights.Sum(x => x);
    }

    protected override int Part2(IEnumerable<Instruction> input)
    {
        var lights = new int[1_000_000];

        foreach (var instruction in input)
        {
            switch (instruction)
            {
                case Instruction.TurnOn turnOn: ApplyToRegion(lights, turnOn.Region, x => x + 1); break;
                case Instruction.Toggle toggle: ApplyToRegion(lights, toggle.Region, x => x + 2); break;
                case Instruction.TurnOff turnOff: ApplyToRegion(lights, turnOff.Region, x => x == 0 ? 0 : x - 1); break;
            }
        }
        
        return lights.Sum(x => x);
    }

    private static void ApplyToRegion(int[] input, Region region, Func<int, int> apply)
    {
        for (var y = region.TopLeft.Y; y <= region.BottomRight.Y; y++)
        {
            for (var x = region.TopLeft.X; x <= region.BottomRight.X; x++)
            {
                input[y * 1000 + x] = apply(input[y * 1000 + x]);
            }
        }
    } 

    
    public abstract record Instruction
    {
        public record TurnOff(Region Region) : Instruction;
        public record TurnOn(Region Region) : Instruction;
        public record Toggle(Region Region) : Instruction;
    }

    public record Point(int X, int Y);
    public record Region(Point TopLeft, Point BottomRight);
}