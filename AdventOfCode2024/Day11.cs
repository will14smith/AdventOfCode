namespace AdventOfCode2024;

[Day]
public partial class Day11 : Day<Day11.Model, long, long>
{
    public record Model(IReadOnlyList<long> Stones);
    
    protected override Model Parse(string input) => new(input.Split(' ').Select(long.Parse).ToArray());
    
    [Sample("125 17", 55312L)]
    protected override long Part1(Model input)
    {
        var cache = new Dictionary<(long Number, long Blinks), long>();
        return input.Stones.Sum(stone => Count(stone, 25L, cache));
    }

    protected override long Part2(Model input)
    {
        var cache = new Dictionary<(long Number, long Blinks), long>();
        return input.Stones.Sum(stone => Count(stone, 75L, cache));
    }

    private static long Count(long stone, long blinks, Dictionary<(long Number, long Blinks), long> cache)
    {
        if (cache.TryGetValue((stone, blinks), out var count))
        {
            return count;
        }

        if (blinks == 0)
        {
            count = 1;
        }
        else
        {
            if (stone == 0)
            {
                count = Count(1, blinks - 1, cache);
            }
            else
            {
                var digits = (long)Math.Floor(Math.Log10(stone)) + 1;
                if (digits % 2 == 0)
                {
                    var factor = (long)Math.Pow(10, digits >> 1);
                    
                    count = Count(stone / factor, blinks - 1, cache) + Count(stone % factor, blinks - 1, cache);
                }
                else
                {
                    count = Count(stone * 2024, blinks - 1, cache);
                }
            }
        }
        
        cache.Add((stone, blinks), count);

        return count;
    }
}