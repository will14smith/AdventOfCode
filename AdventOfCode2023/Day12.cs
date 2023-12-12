using System.Runtime.InteropServices;

namespace AdventOfCode2023;

[Day]
public partial class Day12 : LineDay<Day12.Model, long, long>
{
    protected override Model ParseLine(string input)
    {
        var parts = input.Split(' ');

        var springs = parts[0].Select(x => x switch
        {
            '?' => Spring.Unknown,
            '#' => Spring.Damaged,
            '.' => Spring.Operational,
        }).ToArray();
        var damagedGroupSizes = parts[1].Split(',').Select(int.Parse).ToArray();

        return new Model(springs, damagedGroupSizes);
    }

    [Sample("???.### 1,1,3", 1L)]
    [Sample("?###???????? 3,2,1", 10L)]
    protected override long Part1(IEnumerable<Model> input)
    {
        return input.Sum(CountSolutions);
    }
    
    [Sample(".# 1", 1L)]
    [Sample("???.### 1,1,3", 1L)]
    [Sample(".??..??...?##. 1,1,3", 16384L)]
    [Sample("?###???????? 3,2,1", 506250L)]
    protected override long Part2(IEnumerable<Model> input)
    {
        return input.Select(Unfold).Sum(CountSolutions);
    }

    private long CountSolutions(Model input)
    {
        var cache = new Dictionary<(long, long), long>();
        
        var remainingSprings = input.Springs.Span;
        var remainingGroups = input.DamagedGroupSizes.Span;

        return Count(cache, remainingSprings, remainingGroups);
        
        static long Count(Dictionary<(long, long), long> cache, ReadOnlySpan<Spring> remainingSprings, ReadOnlySpan<int> remainingGroups)
        {
            // have we seen this state before?
            var cacheKey = (remainingSprings.Length, remainingGroups.Length);
            if (cache.TryGetValue(cacheKey, out var cachedValue))
            {
                return cachedValue;
            }
            
            // if there are no springs, then it is only valid is there are no damaged groups
            if (remainingSprings.Length == 0)
            {
                return cache[cacheKey] = remainingGroups.Length == 0 ? 1 : 0;
            }
            
            // if there are no damaged groups, then it is only valid is there are no damaged springs
            if (remainingGroups.Length == 0)
            {
                return cache[cacheKey] = AnyDamaged(remainingSprings) ? 0 : 1;
            }

            var count = 0L;

            // try matching the first group against the first springs
            var headGroup = remainingGroups[0];
            if (Matches(remainingSprings, headGroup))
            {
                // skip the spring after the group too since it must be non-damaged for this group to have matched
                var nextRemainingSprings = headGroup == remainingSprings.Length ? new Span<Spring>() : remainingSprings[(headGroup + 1)..];
                count += Count(cache, nextRemainingSprings, remainingGroups[1..]);
            }
            
            // if the first spring wasn't damaged we can skip it and try matching the first group against the next spring
            if (remainingSprings[0] != Spring.Damaged)
            {
                count += Count(cache, remainingSprings[1..], remainingGroups);
            }

            return cache[cacheKey] = count;
        }

        static bool AnyDamaged(ReadOnlySpan<Spring> springs)
        {
            foreach (var spring in springs)
            {
                if (spring == Spring.Damaged) return true;
            }

            return false;
        }
        
        static bool Matches(ReadOnlySpan<Spring> springs, int damagedSize)
        {
            if (springs.Length < damagedSize) return false;
            
            // any operational springs in the group make it invalid
            foreach (var spring in springs[..damagedSize])
            {
                if (spring == Spring.Operational) return false;
            }
            
            // damaged groups must be the largest possible number, so they must not be followed by a damaged spring
            if (damagedSize < springs.Length)
            {
                return springs[damagedSize] != Spring.Damaged;
            }
            
            return true;
        }
    }
    
    private Model Unfold(Model input)
    {
        var newSprings = new Spring[input.Springs.Length * 5 + 4].AsMemory();
        var newGroups = new int[input.DamagedGroupSizes.Length * 5].AsMemory();

        for (var i = 0; i < 5; i++)
        {
            input.Springs.CopyTo(newSprings[((1 + input.Springs.Length) * i)..]);
            input.DamagedGroupSizes.CopyTo(newGroups[(input.DamagedGroupSizes.Length * i)..]);
        }
        
        return new Model(newSprings, newGroups);
    }

    public record Model(ReadOnlyMemory<Spring> Springs, ReadOnlyMemory<int> DamagedGroupSizes);
    public enum Spring : byte
    {
        Unknown,
        Damaged,
        Operational,
    }
}