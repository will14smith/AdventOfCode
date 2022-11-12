namespace AdventOfCode2015;

[Day]
public partial class Day01 : Day<IReadOnlyList<int>, int, int>
{
    protected override IReadOnlyList<int> Parse(string input) =>
        input.Select(c => c switch
        {
            '(' => 1,
            ')' => -1,
        }).ToList();

    [Sample("(()))", -1)]
    protected override int Part1(IReadOnlyList<int> input) => input.Sum();

    [Sample("(()))", 5)]
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