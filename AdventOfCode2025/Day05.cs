using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2025;

[Day]
public partial class Day05 : ParseDay<Day05.Model, long, long>
{
    public record Range(long Start, long End);
    public record Model(IReadOnlyList<Range> FreshRanges, IReadOnlyList<long> Available);

    private static readonly TextParser<Range> RangeParser =
        from start in Numerics.IntegerInt64
        from dash in Character.EqualTo('-')
        from end in Numerics.IntegerInt64
        select new Range(start, end);
    private static readonly TextParser<Range[]> FreshRangesParser = RangeParser.ThenIgnoreOptional(SuperpowerExtensions.NewLine).Many();
    private static readonly TextParser<long[]> AvailableParser = Numerics.IntegerInt64.ThenIgnoreOptional(SuperpowerExtensions.NewLine).Many();
    private static readonly TextParser<Model> ModelParser =
        from fresh in FreshRangesParser
        from blank in SuperpowerExtensions.NewLine
        from available in AvailableParser
        select new Model(fresh, available);

    protected override TextParser<Model> Parser => ModelParser;

    [Sample("3-5\n10-14\n16-20\n12-18\n\n1\n5\n8\n11\n17\n32\n", 3)]
    protected override long Part1(Model input) => input.Available.Count(a => input.FreshRanges.Any(r => a >= r.Start && a <= r.End));

    [Sample("3-5\n10-14\n16-20\n12-18\n\n1\n5\n8\n11\n17\n32\n", 14)]
    protected override long Part2(Model input)
    {
        var ranges = new List<Range>();
        
        foreach (var range in input.FreshRanges.OrderBy(x => x.Start))
        {
            if (ranges.Count == 0)
            {
                ranges.Add(range);
                continue;
            }
            
            var last = ranges[^1];
            if (range.Start <= last.End)
            {
                ranges[^1] = last with { End = Math.Max(last.End, range.End) };
            }
            else
            {
                ranges.Add(range);
            }
        }
        
        var total = ranges.Sum(r => r.End - r.Start + 1);
        
        return total;
    }
}