namespace AdventOfCode2024;

[Day]
public partial class Day01 : Day<Day01.Model, int, int>
{
    public record Model(IReadOnlyList<int> Left, IReadOnlyList<int> Right);
    
    protected override Model Parse(string input)
    {
        var lines = input.Split('\n').Select(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)).ToArray();
        
        return new Model(lines.Select(x => int.Parse(x[0])).ToArray(), lines.Select(x => int.Parse(x[1])).ToArray());
    }

    [Sample("3   4\n4   3\n2   5\n1   3\n3   9\n3   3", 11)]
    protected override int Part1(Model input) => input.Left.OrderBy(x => x).Zip(input.Right.OrderBy(x =>x)).Sum(x => Math.Abs(x.First - x.Second));
    [Sample("3   4\n4   3\n2   5\n1   3\n3   9\n3   3", 31)]
    protected override int Part2(Model input) => input.Left.Sum(x => x * input.Right.Count(y => y == x));
}