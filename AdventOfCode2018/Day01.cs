namespace AdventOfCode2018;

[Day]
public partial class Day01 : Day<Day01.Model, int, int>
{
    public record Model(IReadOnlyList<int> Frequencies);

    protected override Model Parse(string input) => new(input.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());

    [Sample("+1\n-2\n+3\n+1\n", 3)]
    protected override int Part1(Model input) => input.Frequencies.Sum();
    [Sample("+1\n-2\n+3\n+1\n", 2)]
    protected override int Part2(Model input)
    {
        var frequency = 0;
        var frequencies = new HashSet<int> { 0 };
        
        while (true)
        {
            foreach (var change in input.Frequencies)
            {
                frequency += change;
                if (!frequencies.Add(frequency))
                {
                    return frequency;
                }
            }
        }
    }
}
