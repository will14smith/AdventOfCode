namespace AdventOfCode2023;

[Day]
public partial class Day09 : LineDay<Day09.Model, long, long>
{
    protected override Model ParseLine(string input) => new(input.Split(' ').Select(long.Parse).ToArray());

    [Sample("0 3 6 9 12 15\n1 3 6 10 15 21\n10 13 16 21 30 45", 114L)]
    protected override long Part1(IEnumerable<Model> input) => input.Sum(GetNext);

    [Sample("10 13 16 21 30 45", 5L)]
    protected override long Part2(IEnumerable<Model> input) => input.Select(Reverse).Sum(GetNext);

    private static Model Reverse(Model input) => new(input.Values.Reverse().ToArray());

    private static long GetNext(Model model)
    {
        var current = model.Values;

        var result = 0L;
        while (current.Any(x => x != 0))
        {
            result += current[^1];
            current = GetDeltas(current);
        }

        return result;
    }
    
    private static IReadOnlyList<long> GetDeltas(IReadOnlyList<long> input)
    {
        var output = new long[input.Count - 1];
        for (var i = 0; i < input.Count - 1; i++)
        {
            output[i] = input[i+1] - input[i];
        }
        return output;
    }
    
    public record Model(IReadOnlyList<long> Values);
}