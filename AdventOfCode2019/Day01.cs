namespace AdventOfCode2019;

[Day]
public partial class Day01 : LineDay<Day01.Model, int, int>
{
    protected override Model ParseLine(string input) => new(int.Parse(input));

    [Sample("12", 2)]
    [Sample("14", 2)]
    [Sample("1969", 654)]
    [Sample("100756", 33583)]
    protected override int Part1(IEnumerable<Model> input) => input.Select(x => x.Value).Sum(FuelCalculator);

    [Sample("14", 2)]
    [Sample("1969", 966)]
    [Sample("100756", 50346)]
    protected override int Part2(IEnumerable<Model> input) => input.Sum(RecursiveFuelCalculator);

    private static int FuelCalculator(int mass) => mass / 3 - 2;
    private static int RecursiveFuelCalculator(Model input)
    {
        var sum = 0;
        
        var value = input.Value;
        while (true)
        {
            value = FuelCalculator(value);
            if (value < 0) break;
            
            sum += value;
        }

        return sum;
    }

    public record Model(int Value);
}
