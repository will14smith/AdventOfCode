namespace AdventOfCode2024;

[Day]
public partial class Day07 : LineDay<Day07.Model, long, long>
{
    public record Model(long TestValue, IReadOnlyList<long> Values);

    protected override Model ParseLine(string input)
    {
        var parts = input.Split(": ");
        var parts2 = parts[1].Split(" ").Select(long.Parse);
        
        return new Model(long.Parse(parts[0]), parts2.ToArray());
    }

    [Sample("190: 10 19\n3267: 81 40 27\n83: 17 5\n156: 15 6\n7290: 6 8 6 15\n161011: 16 10 13\n192: 17 8 14\n21037: 9 7 18 13\n292: 11 6 16 20", 3749L)]
    protected override long Part1(IEnumerable<Model> input) => input.Where(TryAddMultiply).Sum(x => x.TestValue);

    [Sample("190: 10 19\n3267: 81 40 27\n83: 17 5\n156: 15 6\n7290: 6 8 6 15\n161011: 16 10 13\n192: 17 8 14\n21037: 9 7 18 13\n292: 11 6 16 20", 11387L)]
    protected override long Part2(IEnumerable<Model> input) => input.Where(TryAddMultiplyConcat).Sum(x => x.TestValue);
    
    private static bool TryAddMultiply(Model input) => TryAddMultiply(input, 0, 0);
    private static bool TryAddMultiply(Model input, int index, long accumulator)
    {
        if (index == input.Values.Count)
        {
            return input.TestValue == accumulator;
        }
        
        var value = input.Values[index];
        if (index == 0)
        {
            return TryAddMultiply(input, 1, value);
        }

        if (accumulator > input.TestValue)
        {
            return false;
        }

        if (TryAddMultiply(input, index + 1, value + accumulator))
        {
            return true;
        }

        return TryAddMultiply(input, index + 1, value * accumulator); 
    }
    
    private static bool TryAddMultiplyConcat(Model input) => TryAddMultiplyConcat(input, 0, 0);
    private static bool TryAddMultiplyConcat(Model input, int index, long accumulator)
    {
        if (index == input.Values.Count)
        {
            return input.TestValue == accumulator;
        }
        
        var value = input.Values[index];
        if (index == 0)
        {
            return TryAddMultiplyConcat(input, 1, value);
        }

        if (accumulator > input.TestValue)
        {
            return false;
        }

        if (TryAddMultiplyConcat(input, index + 1, value + accumulator))
        {
            return true;
        }

        if (TryAddMultiplyConcat(input, index + 1, value * accumulator))
        {
            return true;
        }

        return TryAddMultiplyConcat(input, index + 1, long.Parse($"{accumulator}{value}"));
    }

}