using System.Buffers;

namespace AdventOfCode2020;

[Day]
public partial class Day15 : Day<Day15.Model, int, int>
{
    protected override Model Parse(string input) => new(input.Split(',').Select(int.Parse).ToList());

    [Sample("0,3,6", 436)]
    protected override int Part1(Model input) => Solve(input.Values, 2020);
    [Sample("0,3,6", 175594)]
    protected override int Part2(Model input) => Solve(input.Values, 30000000);

    private static int Solve(IEnumerable<int> input, int count)
    {
        var tracker = ArrayPool<int>.Shared.Rent(count);
        Array.Clear(tracker, 0, count);

        var i = 0;
        var last = 0;
            
        foreach (var arg in input)
        {
            if (i > 0)
            {
                tracker[last] = i;
            }

            last = arg;
            i++;
        }

        for (; i < count; i++)
        {
            var num = 0;
            var lastIndex = tracker[last];
            if (lastIndex != 0)
            {
                num = i - lastIndex;
            }

            tracker[last] = i;
            last = num;
        }

        ArrayPool<int>.Shared.Return(tracker);
            
        return last;
    }
    
    public record Model(IReadOnlyList<int> Values);
}
