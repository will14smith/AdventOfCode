namespace AdventOfCode2025;

[Day]
public partial class Day02 : Day<Day02.Model, long, long>
{
    public record Model(IReadOnlyList<Range> Ranges);
    public record Range(long Start, long End);

    protected override Model Parse(string input) => new(input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => {
        var parts = x.Split('-');
        return new Range(long.Parse(parts[0]), long.Parse(parts[1]));
    }).ToList());

    [Sample("11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124", 1227775554)]
    protected override long Part1(Model input) => input.Ranges.Sum(SumInvalidNumbers1);
    
    [Sample("11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124", 4174379265)]
    protected override long Part2(Model input) => input.Ranges.Sum(SumInvalidNumbers2);
    
    private static long SumInvalidNumbers1(Range range)
    {
        var sum = 0L;

        for (var id = range.Start; id <= range.End; id++)
        {
            var str = id.ToString();
            if (str.Length % 2 == 1)
            {
                continue;
            }
            
            if(str[..(str.Length / 2)] == str[(str.Length / 2)..])
            {
                sum += id;
            }
        }

        return sum;
    }
    
    private static long SumInvalidNumbers2(Range range)
    {
        var sum = 0L;

        for (var id = range.Start; id <= range.End; id++)
        {
            var str = id.ToString();
            
            // this _could_ be a lot more efficient, but eh
            for (var length = 1; length <= str.Length / 2; length++)
            {
                var slice = str[..length];
                var repeated = string.Concat(Enumerable.Repeat(slice, str.Length / length));
                if (repeated == str)
                {
                    sum += id;
                    break;
                }
            }
        }

        return sum;
    }

}