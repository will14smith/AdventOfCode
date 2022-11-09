using AdventOfCode;
using AdventOfCode.Core;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2015;

[Day]
public partial class Day02 : ParseLineDay<Day02.Model, int, int>
{
    protected override IEnumerable<(string, int)> Tests1 { get; } = new[]
    {
        ("2x3x4", 58),
        ("1x1x10", 43),
    };

    protected override IEnumerable<(string, int)> Tests2 { get; } = new[]
    {
        ("2x3x4", 34),
        ("1x1x10", 14),
    };
    
    protected override TextParser<Model> Parser { get; } =
        from a in Numerics.IntegerInt32
        from _1 in Character.EqualTo('x')
        from b in Numerics.IntegerInt32
        from _2 in Character.EqualTo('x')
        from c in Numerics.IntegerInt32
        select new Model(a, b, c);

    protected override int Part1(IEnumerable<Model> input) => input.Sum(x => x.SurfaceArea + x.SmallestSideArea);
    protected override int Part2(IEnumerable<Model> input) => input.Sum(x => x.Volume + x.RibbonLength);

    public record Model(int A, int B, int C)
    {
        public int SurfaceArea => 2 * A * B + 2 * A * C + 2 * B * C;
        public int SmallestSideArea
        {
            get
            {
                var (x, y) = SmallestTwoSides;
                return x * y;
            }
        }
        public (int, int) SmallestTwoSides
        {
            get
            {
                int x = A, y = B;

                if (x > y) (x, y) = (y, x);
                if (y > C) y = C;
                if (x > y) (x, y) = (y, x);
                
                return (x, y);
            }
        }

        public int Volume => A * B * C;

        public int RibbonLength
        {
            get
            {
                var (x, y) = SmallestTwoSides;
                return 2 * x + 2 * y;
            }
        }
    }
}