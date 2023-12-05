namespace AdventOfCode2023;

[Day]
public partial class Day05 : Day<Day05.Model, long, long>
{
    protected override Model Parse(string input)
    {
        var lines = input.Split('\n');

        var seeds = lines[0].Split(' ').Skip(1).Select(long.Parse).ToArray();

        var ranges = new List<IReadOnlyList<Range>>();
        var currentRanges = new List<Range>();
        
        foreach (var s in lines.Skip(3))
        {
            if (s.Length == 0) continue;
            if (!char.IsDigit(s[0]))
            {
                ranges.Add(currentRanges);
                currentRanges = new List<Range>();
                continue;
            }

            var parts = s.Split(' ').Select(long.Parse).ToArray();
            var range = new Range(parts[0], parts[1], parts[2]);
            
            currentRanges.Add(range);
        }
        
        ranges.Add(currentRanges);

        return new Model(seeds, ranges);
    }

    [Sample("seeds: 79 14 55 13\n\nseed-to-soil map:\n50 98 2\n52 50 48\n\nsoil-to-fertilizer map:\n0 15 37\n37 52 2\n39 0 15\n\nfertilizer-to-water map:\n49 53 8\n0 11 42\n42 0 7\n57 7 4\n\nwater-to-light map:\n88 18 7\n18 25 70\n\nlight-to-temperature map:\n45 77 23\n81 45 19\n68 64 13\n\ntemperature-to-humidity map:\n0 69 1\n1 0 69\n\nhumidity-to-location map:\n60 56 37\n56 93 4", 35L)]
    protected override long Part1(Model input) => input.Ranges.Aggregate(input.Seeds, ApplyRanges).Min();

    private IReadOnlyList<long> ApplyRanges(IReadOnlyList<long> values, IReadOnlyList<Range> ranges)
    {
        var newValues = new List<long>();
        foreach (var value in values)
        {
            var found = false;
            foreach (var range in ranges)
            {
                if (value < range.Source) continue;
                if (value > range.SourceLast) continue;
                
                newValues.Add(value - range.Source + range.Destination);
                found = true;
                break;
            }

            if (!found)
            {
                newValues.Add(value);
            }
        }
        return newValues;
    }

    [Sample("seeds: 79 14 55 13\n\nseed-to-soil map:\n50 98 2\n52 50 48\n\nsoil-to-fertilizer map:\n0 15 37\n37 52 2\n39 0 15\n\nfertilizer-to-water map:\n49 53 8\n0 11 42\n42 0 7\n57 7 4\n\nwater-to-light map:\n88 18 7\n18 25 70\n\nlight-to-temperature map:\n45 77 23\n81 45 19\n68 64 13\n\ntemperature-to-humidity map:\n0 69 1\n1 0 69\n\nhumidity-to-location map:\n60 56 37\n56 93 4", 46L)]
    protected override long Part2(Model input)
    {
        IReadOnlyList<SeedRange> seedRanges = input.Seeds.BatchesOf2().Select(x => new SeedRange(x.A, x.B)).ToArray();

        return input.Ranges.Aggregate(seedRanges, ApplyRanges).Select(x => x.Start).Min();
    }

    private IReadOnlyList<SeedRange> ApplyRanges(IReadOnlyList<SeedRange> values, IReadOnlyList<Range> ranges)
    {
        var newValues = new List<SeedRange>();

        var search = new Queue<SeedRange>(values);
        while (search.Count > 0)
        {
            var value = search.Dequeue();
            
            var found = false;
            foreach (var range in ranges)
            {
                if (value.Last < range.Source) continue;
                if (value.Start > range.SourceLast) continue;

                // split value range so that anything before the matched range is queued to retry
                if (value.Start < range.Source)
                {
                    var (beforeExclusive, afterInclusive) = value.SplitAt(range.Source);
                    search.Enqueue(beforeExclusive);
                    value = afterInclusive;
                }
                
                // split value range so that anything after the matched range is queued to retry
                if (value.Last > range.SourceLast)
                {
                    var (beforeExclusive, afterInclusive) = value.SplitAt(range.SourceLast + 1);
                    search.Enqueue(afterInclusive);
                    value = beforeExclusive;
                }

                var newStart = value.Start - range.Source + range.Destination;
                
                newValues.Add(value with { Start = newStart });
                found = true;
                break;
            }

            if (!found)
            {
                newValues.Add(value);
            }

        }

        return newValues;
    }

    public record Model(IReadOnlyList<long> Seeds, IReadOnlyList<IReadOnlyList<Range>> Ranges);

    public record Range(long Destination, long Source, long Length)
    {
        public long SourceLast => Source + Length - 1;
    }

    public record SeedRange(long Start, long Length)
    {
        public (SeedRange BeforeExclusive, SeedRange AfterInclusive) SplitAt(long value)
        {
            if (value <= Start) throw new ArgumentOutOfRangeException(nameof(value));
            if (value > Last) throw new ArgumentOutOfRangeException(nameof(value));

            var beforeExclusive = new SeedRange(Start, value - Start);
            var afterInclusive = new SeedRange(value, Length - beforeExclusive.Length);
            
            return (beforeExclusive, afterInclusive);
        }
        
        public long Last => Start + Length - 1;
    }
}