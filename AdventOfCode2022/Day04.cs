using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2022;

[Day]
public partial class Day04 : ParseLineDay<Day04.ElfPair, int, int>
{
    private const string Sample = "2-4,6-8\n2-3,4-5\n5-7,7-9\n2-8,3-7\n6-6,4-6\n2-6,4-8";
    
    public readonly static TextParser<ElfRange> ElfRangeParser = Numerics.IntegerInt32.ThenIgnore(Character.EqualTo('-')).Then(Numerics.IntegerInt32).Select(x => new ElfRange(x.Item1, x.Item2));
    public readonly static TextParser<ElfPair> ElfPairParser = ElfRangeParser.ThenIgnore(Character.EqualTo(',')).Then(ElfRangeParser).Select(x => new ElfPair(x.Item1, x.Item2));

    protected override TextParser<ElfPair> LineParser => ElfPairParser;

    [Sample(Sample, 2)]
    protected override int Part1(IEnumerable<ElfPair> input) => input.Count(FullyOverlapping);

    [Sample(Sample, 4)]
    protected override int Part2(IEnumerable<ElfPair> input) => input.Count(PartOverlapping);
    
    private static bool FullyOverlapping(ElfPair pair)
    {
        var aMask = pair.A.BitMask;
        var bMask = pair.B.BitMask;

        var cMask = aMask | bMask;

        return Int128.PopCount(cMask) == Int128.Max(Int128.PopCount(aMask), Int128.PopCount(bMask));
    }
    
    private static bool PartOverlapping(ElfPair pair)
    {
        var aMask = pair.A.BitMask;
        var bMask = pair.B.BitMask;

        var cMask = aMask & bMask;

        return cMask != 0;
    }

    public record ElfPair(ElfRange A, ElfRange B);

    public record ElfRange(int Lower, int Upper)
    {
        public Int128 BitMask
        {
            get
            {
                var mask = Int128.Zero;

                for (var i = Lower; i <= Upper; i++)
                {
                    mask |= Int128.One << i;
                }
                
                return mask;
            }
        }
    }
}
