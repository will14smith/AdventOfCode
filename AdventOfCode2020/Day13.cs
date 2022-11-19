using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2020;

[Day]
public partial class Day13 : ParseDay<Day13.Model, long, long>
{
    private const string Sample = "939\n7,13,x,x,59,x,31,19\n";

    private static readonly TextParser<long> Bus = Numerics.IntegerInt64.Or(Character.EqualTo('x').Select(_ => -1L));
    private static readonly TextParser<long[]> Buses = Bus.ManyDelimitedBy(Character.EqualTo(','));

    protected override TextParser<Model> Parser =>
        from timeToWait in Numerics.IntegerInt32
        from _ in SuperpowerExtensions.NewLine
        from buses in Buses
        select new Model(timeToWait, buses);

    [Sample(Sample, 295L)]
    protected override long Part1(Model input)
    {
        var bestBus = 0L;
        var bestTime = long.MaxValue;

        foreach (var bus in input.Buses.Where(x => x != -1))
        {
            var timeForBus = bus - input.TimeToWait % bus;
            if (timeForBus < bestTime)
            {
                bestBus = bus;
                bestTime = timeForBus;
            }
        }

        return bestBus * bestTime;
    }

    [Sample(Sample, 1068781L)]
    protected override long Part2(Model input)
    {
        var schedule = input.Buses.Select((x, i) => (Bus: x, Offset: (long)i)).Where(x => x.Bus != -1).ToList();

        return SolvePart2(schedule);
    }

    private static long SolvePart2(IReadOnlyList<(long Bus, long Offset)> schedule)
    {
        var (bus0, offset0) = schedule[0];

        for (var index = 1; index < schedule.Count; index++)
        {
            var (busI, offsetI) = schedule[index];

            // bus0 and busI will meet once in the busN period
            var busN = bus0 * busI;

            // calculate how many times bus0 has to visit before we line up
            var offsetN = offset0;
            while (offsetN < busN && (offsetN + offsetI) % busI != 0)
            {
                offsetN += bus0;
            }

            if (offsetN > busN)
            {
                throw new Exception("buses never align");
            }

            bus0 = busN;
            offset0 = offsetN;
        }

        return offset0;
    }

    
    public record Model(long TimeToWait, IReadOnlyList<long> Buses);
}
