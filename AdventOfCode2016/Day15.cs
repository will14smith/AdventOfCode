using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2016;

[Day]
public partial class Day15 : ParseLineDay<Day15.Model, int, int>
{
    private const string Sample = "Disc #1 has 5 positions; at time=0, it is at position 4.\nDisc #2 has 2 positions; at time=0, it is at position 1.";
    
    protected override TextParser<Model> LineParser =>
        from disk in Span.EqualTo("Disc #").IgnoreThen(Numerics.IntegerInt32)
        from positions in Span.EqualTo(" has ").IgnoreThen(Numerics.IntegerInt32)
        from position in Span.EqualTo(" positions; at time=0, it is at position ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo("."))
        select new Model(disk, positions, position);
    
    [Sample(Sample, 5)]
    protected override int Part1(IEnumerable<Model> input) => Solve(input.ToList());

    [Sample(Sample, 85)]
    protected override int Part2(IEnumerable<Model> input)
    {
        var disks = input.ToList();
        disks.Add(new Model(disks.Count + 1, 11, 0));
        return Solve(disks);
    }

    private static int Solve(IReadOnlyCollection<Model> disks)
    {
        // find `t` where
        //    disk-n at t+n is 0 - (c+t+n % p) == 0

        var t = 0;
        while (true)
        {
            if (disks.All(x => (x.Current + x.Id + t) % x.Positions == 0))
            {
                return t;
            }

            t++;
        }
    }

    public record Model(int Id, int Positions, int Current);
}
