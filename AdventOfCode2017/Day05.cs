namespace AdventOfCode2017;

[Day]
public partial class Day05 : Day<Day05.Model, int, int>
{
    protected override Model Parse(string input) => new(input.Split('\n').Select(int.Parse).ToList());

    [Sample("0\n3\n0\n1\n-3", 5)]
    protected override int Part1(Model input)
    {
        var offsets = input.Offsets.ToArray();
        
        var current = 0;
        var steps = 0;
        
        while (true)
        {
            current += offsets[current]++;
            steps++;

            if (current < 0 || current >= offsets.Length)
            {
                return steps;
            }
        }
    }

    [Sample("0\n3\n0\n1\n-3", 10)]
    protected override int Part2(Model input)
    {
        var offsets = input.Offsets.ToArray();

        var current = 0;
        var steps = 0;
        
        while (true)
        {
            var jump = offsets[current];
            if (jump >= 3)
            {
                offsets[current]--;
            }
            else
            {
                offsets[current]++;
            }
            current += jump;
            steps++;

            if (current < 0 || current >= offsets.Length)
            {
                return steps;
            }
        }
    }

    public record Model(IReadOnlyList<int> Offsets);
}
