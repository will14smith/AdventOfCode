namespace AdventOfCode2015;

[Day]
public partial class Day24 : LineDay<long, long, long>
{
    protected override long ParseLine(string input) => long.Parse(input);
    
    [Sample("1\n2\n3\n4\n5\n7\n8\n9\n10\n11", 99L)]
    protected override long Part1(IEnumerable<long> input) => Solve(input, 3);

    [Sample("1\n2\n3\n4\n5\n7\n8\n9\n10\n11", 44L)]
    protected override long Part2(IEnumerable<long> input) => Solve(input, 4);
    
    private static long Solve(IEnumerable<long> input, int parts)
    {
        var sizes = input.OrderBy(x => x).ToArray();

        var sum = sizes.Sum();
        var groupTarget = sum / parts;

        for (var len = 1; len < sizes.Length; len++)
        {
            var combinations = Combinations.Get(sizes, len)
                .Select(x => x.ToArray())
                .Where(x => x.Sum() == groupTarget)
                .ToArray();

            if (!combinations.Any())
            {
                continue;
            }

            return combinations.Min(x => x.Aggregate(1L, (a, b) => a * b));
        }

        throw new Exception("no solution");
    }

}