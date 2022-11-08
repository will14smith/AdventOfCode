using AdventOfCode;
using AdventOfCode.Core;

namespace AdventOfCode2015;

[Day]
public partial class Day01 : Day<IReadOnlyList<int>, int, int>
{
    protected override IEnumerable<(string, int)> Tests1 { get; } = new[]
    {
        ("(()))", -1),
    };
    protected override IEnumerable<(string, int)> Tests2 { get; } = new[]
    {
        ("(()))", 5),
    };

    protected override IReadOnlyList<int> Parse(string input) =>
        input.Select(c => c switch
        {
            '(' => 1,
            ')' => -1,
        }).ToList();

    protected override int Part1(IReadOnlyList<int> input) => input.Sum();

    protected override int Part2(IReadOnlyList<int> input)
    {
        var floor = 0;
        var position = 1;
        
        foreach (var delta in input)
        {
            floor += delta;
            if (floor < 0)
            {
                return position;
            }
            position++;
        }
        
        return position;
    }
}