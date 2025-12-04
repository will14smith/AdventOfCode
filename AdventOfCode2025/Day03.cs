namespace AdventOfCode2025;

[Day]
public partial class Day03 : Day<Day03.Model, long, long>
{
    public record Model(IReadOnlyList<string> Banks);

    protected override Model Parse(string input) => new(input.Split('\n', StringSplitOptions.RemoveEmptyEntries));

    [Sample("987654321111111\n811111111111119\n234234234234278\n818181911112111\n", 357)]
    protected override long Part1(Model input) => input.Banks.Sum(x => MaxJoltage(x, 2));
    
    [Sample("987654321111111\n811111111111119\n234234234234278\n818181911112111\n", 3121910778619)]
    protected override long Part2(Model input) => input.Banks.Sum(x => MaxJoltage(x, 12));

    private long MaxJoltage(ReadOnlySpan<char> bank, int count)
    {
        if (count == 0)
        {
            return 0;
        }
        
        var highestFirstDigit = bank[0];
        var highestFirstDigitIndex = 0;
        
        for(var i = 1; i < bank.Length - count + 1; i++)
        {
            if (bank[i] > highestFirstDigit)
            {
                highestFirstDigit = bank[i];
                highestFirstDigitIndex = i;
            }
        }
        
        return (highestFirstDigit - '0') * (long) Math.Pow(10, count - 1) + MaxJoltage(bank[(highestFirstDigitIndex + 1)..], count - 1);
    }
}