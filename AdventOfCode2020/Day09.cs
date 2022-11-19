using System.Collections.Concurrent;

namespace AdventOfCode2020;

[Day]
public partial class Day09 : LineDay<long, long, long>
{
    protected override long ParseLine(string input) => long.Parse(input);
    
    protected override long Part1(IEnumerable<long> input) => FindFirstInconsistency(input.ToList(), 25);

    protected override long Part2(IEnumerable<long> input) => FindInconsistencyRangeSum(input.ToList(), 400480901);

    private long FindFirstInconsistency(IReadOnlyList<long> data, int size)
    {
        var d = new ConcurrentDictionary<long, int>();

        int i;

        for (i = 0; i < size; i++)
        {
            Add(d, data[i]);
        }

        for (; i < data.Count; i++)
        {
            if (!IsValid(d, data[i]))
            {
                return data[i];
            }

            Add(d, data[i]);
            Remove(d, data[i - size]);
        }

        throw new Exception("no inconsistency");
    }

    private static bool IsValid(ConcurrentDictionary<long, int> d, long v)
    {
        foreach (var k in d.Keys)
        {
            if (v - k != k && d.ContainsKey(v - k))
            {
                return true;
            }
        }

        return false;
    }

    private static void Add(ConcurrentDictionary<long, int> d, long v)
    {
        d.AddOrUpdate(v, k => 1, (k, c) => c + 1);
    }

    private static void Remove(ConcurrentDictionary<long, int> d, long v)
    {
        var newC = d.AddOrUpdate(v, k => 0, (k, c) => c - 1);
        if (newC == 0)
        {
            d.TryRemove(v, out _);
        }
    }

    private static long FindInconsistencyRangeSum(IReadOnlyList<long> data, int target)
    {
        var (start, end) = FindInconsistencyRange(data, target);
        return start + end;
    }

    private static (long, long) FindInconsistencyRange(IReadOnlyList<long> data, int target)
    {
        for (var startIndex = 0; startIndex < data.Count; startIndex++)
        {
            var sum = data[startIndex];

            for (var endIndex = startIndex + 1; endIndex < data.Count; endIndex++)
            {
                sum += data[endIndex];
                if (sum == target)
                {
                    var range = data.Skip(startIndex).Take(endIndex - startIndex + 1).ToList();
                    return (range.Min(), range.Max());
                }
            }
        }

        throw new Exception("no range");
    }
}
