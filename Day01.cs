using AdventOfCode2015.Utilities;

namespace AdventOfCode2015;

public class Day01 : Test
{
    public Day01(ITestOutputHelper output) : base(1, output) { }
    
    [Fact]
    public void Part1()
    {
        Run("sample", "))(((", Parse, SolvePart1).Should().Be(1);
        Run("actual", Parse, SolvePart1);
    }
    
    [Fact]
    public void Part2()
    {
        Run("sample", "()())", Parse, SolvePart2).Should().Be(5);
        Run("actual", Parse, SolvePart2);
    }

    private static int SolvePart1(IEnumerable<int> input) => input.Sum();
    private static int SolvePart2(IEnumerable<int> input)
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

    public static IEnumerable<int> Parse(string input) => input.Select(ParseChar);
    public static int ParseChar(char input) => input switch
    {
        '(' => 1,
        ')' => -1,
    };
}