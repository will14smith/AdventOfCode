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
    
    private static bool TryAddMultiply(Model input) => TryAddMultiply(input.Values, input.Values.Count - 1, input.TestValue);

    private static bool TryAddMultiply(IReadOnlyList<long> values, int index, long accumulator)
    {
        if (index == 0)
        {
            return accumulator == values[0];
        }
        
        var value = values[index];
        if (value > accumulator)
        {
            return false;
        }
        
        var quotient = Math.DivRem(accumulator, value, out var remainder);
        if (remainder == 0)
        {
            if (TryAddMultiply(values, index - 1, quotient))
            {
                return true;
            }
        }
        
        return TryAddMultiply(values, index - 1, accumulator - value); 
    }


    private static bool TryAddMultiplyConcat(Model input) => TryAddMultiplyConcat(input.Values, input.Values.Count - 1, input.TestValue);
    private static bool TryAddMultiplyConcat(IReadOnlyList<long> values, int index, long accumulator)
    {
        if (index == 0)
        {
            return accumulator == values[0];
        }
        
        var value = values[index];
        if (value > accumulator)
        {
            return false;
        }
        
        var quotient = Math.DivRem(accumulator, value, out var remainder);
        if (remainder == 0)
        {
            if (TryAddMultiplyConcat(values, index - 1, quotient))
            {
                return true;
            }
        }

        var accumulatorString = accumulator.ToString();
        var valueString = value.ToString();
        if (accumulatorString.Length > valueString.Length && accumulatorString.EndsWith(valueString))
        {
            if (TryAddMultiplyConcat(values, index - 1, long.Parse(accumulatorString.Remove(accumulatorString.Length - valueString.Length))))
            {
                return true;
            }
        }
        
        return TryAddMultiplyConcat(values, index - 1, accumulator - value); 
    }

}