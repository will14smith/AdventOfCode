namespace AdventOfCode2023;

[Day]
public partial class Day09 : LineDay<Day09.Model, long, long>
{
    protected override Model ParseLine(string input) => new(input.Split(' ').Select(long.Parse).ToArray());

    [Sample("0 3 6 9 12 15\n1 3 6 10 15 21\n10 13 16 21 30 45", 114L)]
    protected override long Part1(IEnumerable<Model> input)
    {
        var result = 0L;

        foreach (var model in input)
        {
            var stack = GetAllDeltas(model.Values);

            var next = 0L;
            while (stack.Count > 0)
            {
                var delta = stack.Pop();
                next += delta[^1];
            }

            result += next;
        }
        
        return result;
    }
    
    [Sample("10 13 16 21 30 45", 5L)]
    protected override long Part2(IEnumerable<Model> input)
    {
        var result = 0L;

        foreach (var model in input)
        {
            var stack = GetAllDeltas(model.Values);
            
            var next = 0L;
            while (stack.Count > 0)
            {
                var delta = stack.Pop();
                next = delta[0] - next;
            }

            result += next;
        }
        
        return result;
    }
    
    private static Stack<IReadOnlyList<long>> GetAllDeltas(IReadOnlyList<long> values)
    {
        var stack = new Stack<IReadOnlyList<long>>();

        while (values.Any(x => x != 0))
        {
            stack.Push(values);
            values = GetDeltas(values);
        }

        return stack;
    }

    private static IReadOnlyList<long> GetDeltas(IReadOnlyList<long> input)
    {
        var output = new long[input.Count - 1];
        for (var i = 0; i < input.Count - 1; i++)
        {
            output[i] = input[i + 1] - input[i];
        }
        return output;
    }
    
    public record Model(IReadOnlyList<long> Values);
}