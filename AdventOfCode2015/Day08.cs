using System.Text;

namespace AdventOfCode2015;

[Day]
public partial class Day08 : LineDay<string, int, int>
{
    protected override string ParseLine(string input) => input;

    [Sample("\"\"", 2)]
    [Sample("\"abc\"", 2)]
    [Sample("\"aaa\\\"aaa\"", 3)]
    [Sample("\"\\x27\"", 5)]
    protected override int Part1(IEnumerable<string> input)
    {
        return input.Sum(line => line.Length - Decode(line).Length);
    }
    
    [Sample("\"\"", 4)]
    [Sample("\"abc\"", 4)]
    [Sample("\"aaa\\\"aaa\"", 6)]
    [Sample("\"\\x27\"", 5)]
    protected override int Part2(IEnumerable<string> input)
    {
        return input.Sum(line => Encode(line).Length - line.Length);
    }

    private static string Encode(string line)
    {
        var output = new StringBuilder();

        output.Append('"');
        
        foreach (var c in line)
        {
            switch (c)
            {
                case '"': output.Append("\\\""); break;
                case '\\': output.Append("\\\\"); break;
                default: output.Append(c); break;
            }
        }
        
        output.Append('"');
        return output.ToString();
    }
    
    private static string Decode(string input)
    {
        var output = new StringBuilder();
        
        for (var i = 1; i < input.Length - 1; i++)
        {
            var c = input[i];
            if (c == '\\')
            {
                var c2 = input[++i];
                switch (c2)
                {
                    case '\\':
                        output.Append('\\');
                        break;
                    case '"':
                        output.Append('"');
                        break;
                    case 'x':
                        i += 2;
                        // TODO actually decode it...
                        output.Append('.');
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                output.Append(c);
            }
        }
        
        return output.ToString();
    }
}