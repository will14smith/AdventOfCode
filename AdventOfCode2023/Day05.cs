using System.Text.RegularExpressions;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2023;

[Day]
public partial class Day05 : ParseDay<Day05.Model, long, long>
{
    private const string Sample = "seeds: 79 14 55 13\n\nseed-to-soil map:\n50 98 2\n52 50 48\n\nsoil-to-fertilizer map:\n0 15 37\n37 52 2\n39 0 15\n\nfertilizer-to-water map:\n49 53 8\n0 11 42\n42 0 7\n57 7 4\n\nwater-to-light map:\n88 18 7\n18 25 70\n\nlight-to-temperature map:\n45 77 23\n81 45 19\n68 64 13\n\ntemperature-to-humidity map:\n0 69 1\n1 0 69\n\nhumidity-to-location map:\n60 56 37\n56 93 4";
    
    private static readonly TextParser<long[]> SeedsParser = Span.EqualTo("seeds: ").IgnoreThen(Numerics.IntegerInt64.ManyDelimitedBy(Character.EqualTo(' '))).ThenIgnore(Character.EqualTo('\n'));
    private static readonly TextParser<Range> RangeParser = Numerics.IntegerInt64.AtLeastOnceDelimitedBy(Character.EqualTo(' ')).ThenIgnore(Character.EqualTo('\n').Optional()).Select(x => new Range(x[0], x[1], x[2]));
    private static readonly TextParser<Range[]> MappingParser = Span.Regex(@"\n[a-z]+\-to\-[a-z]+ map:\n", RegexOptions.Multiline).IgnoreThen(RangeParser.Many());
    
    protected override TextParser<Model> Parser { get; } = SeedsParser.Then(MappingParser.AtLeastOnce()).Select(x => new Model(x.Item1, x.Item2));

    [Sample(Sample, 35L)]
    protected override long Part1(Model input) => input.Mappings.Aggregate(input.Seeds, ApplyMappings).Min();

    private static IReadOnlyList<long> ApplyMappings(IReadOnlyList<long> values, IReadOnlyList<Range> mappings) => values.Select(value => ApplyMappings(value, mappings)).ToList();
    private static long ApplyMappings(long value, IReadOnlyList<Range> mappings)
    {
        foreach (var mapping in mappings)
        {
            if (value < mapping.Source) continue;
            if (value > mapping.SourceLast) continue;

            return value - mapping.Source + mapping.Destination;
        }

        return value;
    }

    [Sample(Sample, 46L)]
    protected override long Part2(Model input)
    {
        IReadOnlyList<SeedRange> seedRanges = input.Seeds.BatchesOf2().Select(x => new SeedRange(x.A, x.B)).ToArray();

        return input.Mappings.Aggregate(seedRanges, ApplyMappings).Select(x => x.Start).Min();
    }

    private static IReadOnlyList<SeedRange> ApplyMappings(IReadOnlyList<SeedRange> values, IReadOnlyList<Range> mappings)
    {
        var newValues = new List<SeedRange>();

        var search = new Queue<SeedRange>(values);
        while (search.Count > 0)
        {
            var value = search.Dequeue();
            newValues.Add(ApplyMappings(value, mappings, search));
        }

        return newValues;
    }
    private static SeedRange ApplyMappings(SeedRange value, IReadOnlyList<Range> mappings, Queue<SeedRange> search)
    {
        foreach (var mapping in mappings)
        {
            if (value.Last < mapping.Source) continue;
            if (value.Start > mapping.SourceLast) continue;

            // split value range so that anything before the matched range is queued to retry
            if (value.Start < mapping.Source)
            {
                var (beforeExclusive, afterInclusive) = value.SplitAt(mapping.Source);
                search.Enqueue(beforeExclusive);
                value = afterInclusive;
            }

            // split value range so that anything after the matched range is queued to retry
            if (value.Last > mapping.SourceLast)
            {
                var (beforeExclusive, afterInclusive) = value.SplitAt(mapping.SourceLast + 1);
                search.Enqueue(afterInclusive);
                value = beforeExclusive;
            }

            var newStart = value.Start - mapping.Source + mapping.Destination;

            return value with { Start = newStart };
        }

        return value;
    }

    public record Model(IReadOnlyList<long> Seeds, IReadOnlyList<IReadOnlyList<Range>> Mappings);

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