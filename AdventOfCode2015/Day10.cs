using System.Text;

namespace AdventOfCode2015;

[Day]
public partial class Day10 : Day<string, int, int>
{
    protected override string Parse(string input) => input;

    [Sample("#1", 2)]
    [Sample("#11", 2)]
    [Sample("#21", 4)]
    [Sample("#1211", 6)]
    [Sample("#111221", 6)]
    protected override int Part1(string input)
    {
        var count = input[0] == '#' ? 1 : 40;
        input = input[0] == '#' ? input[1..] : input;
        
        for (var i = 0; i < count; i++)
        {
            input = Apply(input);
        }

        return input.Length;
    }

    protected override int Part2(string input)
    {
        for (var i = 0; i < 50; i++) { input = Apply(input); }

        return input.Length;
    }

    private static string Apply(string input)
    {
        var output = new StringBuilder();
        
        var runLength = 1;
        var runChar = input[0];
        
        for (var i = 1; i < input.Length; i++)
        {
            var c = input[i];
            if (runChar != c)
            {
                output.Append(runLength);
                output.Append(runChar);

                runLength = 1;
                runChar = c;
            }
            else
            {
                runLength++;
            }
        }
        
        output.Append(runLength);
        output.Append(runChar);

        return output.ToString();
    }
}