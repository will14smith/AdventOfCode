using System.Text;

namespace AdventOfCode2022;

[Day]
public partial class Day25 : LineDay<string, string, string>
{
    private const string Sample = "1=-0-2\n12111\n2=0=\n21\n2=01\n111\n20012\n112\n1=-1=\n1-12\n12\n1=\n122";
    
    protected override string ParseLine(string input) => input;

    [Sample(Sample, "2=-1=0")]
    protected override string Part1(IEnumerable<string> input) => Encode(input.Sum(Decode));
    
    protected override string Part2(IEnumerable<string> input) => "Merry Christmas.";
    
    private static long Decode(string str)
    {
        var value = 0L;

        foreach (var c in str)
        {
            var digit = c switch
            {
                '2' => 2,
                '1' => 1,
                '0' => 0,
                '-' => -1,
                '=' => -2,
            };
            
            value = value * 5 + digit;
        }

        return value;
    }

    private static string Encode(long value)
    {
        var sb = new StringBuilder();

        while (value > 0)
        {
            var digit = (value + 2) % 5;
            sb.Insert(0, digit switch
            {
                0 => '=',
                1 => '-',
                2 => '0',
                3 => '1',
                4 => '2',
            });
            
            value = (value+2)/ 5;
        }
        
        return sb.ToString();
    }
}
