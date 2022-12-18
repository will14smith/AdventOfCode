namespace AdventOfCode2016;

[Day]
public partial class Day07 : LineDay<string, int, int>
{
    protected override string ParseLine(string input) => input;

    [Sample("abba[mnop]qrst", 1)]
    [Sample("abcd[bddb]xyyx", 0)]
    [Sample("aaaa[qwer]tyui", 0)]
    [Sample("ioxxoj[asdfgh]zxcvbn", 1)]
    protected override int Part1(IEnumerable<string> input) => input.Count(SupportsTls);

    [Sample("aba[bab]xyz", 1)]
    [Sample("xyx[xyx]xyx", 0)]
    [Sample("aaa[kek]eke", 1)]
    [Sample("zazbz[bzb]cdb", 1)]
    protected override int Part2(IEnumerable<string> input) => input.Count(SupportsSsl);
    
    private bool SupportsTls(string input)
    {
        var brackets = 0;
        
        var supported = false;

        for (var i = 0; i < input.Length; i++)
        {
            switch (input[i])
            {
                case '[':
                    brackets++;
                    break;
                case ']':
                    brackets--;
                    break;
                default:
                {
                    if(i >= 3)
                    {
                        if (input[i - 3] == input[i] && input[i - 2] == input[i - 1] && input[i-1] != input[i])
                        {
                            if (brackets > 0)
                            {
                                return false;
                            }

                            supported = true;
                        }
                    }

                    break;
                }
            }
        }

        return supported;
    }
    
    private bool SupportsSsl(string input)
    {
        var ab = new HashSet<(char, char)>();

        var brackets = 0;
        for (var i = 0; i < input.Length; i++)
        {
            switch (input[i])
            {
                case '[':
                    brackets++;
                    break;
                case ']':
                    brackets--;
                    break;
                default:
                    if (brackets == 0 & i >= 2)
                    {
                        if (input[i - 2] == input[i] && input[i - 1] != input[i])
                        {
                            ab.Add((input[i], input[i-1]));
                        }
                    }
                    break;
            }
        }
        
        brackets = 0;
        for (var i = 0; i < input.Length; i++)
        {
            switch (input[i])
            {
                case '[':
                    brackets++;
                    break;
                case ']':
                    brackets--;
                    break;
                default:
                    if (brackets > 0 & i >= 2)
                    {
                        if (input[i - 2] == input[i] && input[i - 1] != input[i])
                        {
                            if (ab.Contains((input[i - 1], input[i])))
                            {
                                return true;
                            }
                        }
                    }
                    break;
            }
        }

        return false;
    }
}
