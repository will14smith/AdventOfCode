namespace AdventOfCode2015;

[Day]
public partial class Day11 : Day<string, string, string>
{
    protected override string Parse(string input) => input;
    
    protected override string Part1(string input)
    {
        do
        {
            input = Next(input);
        } while (!Valid(input));

        return input;
    }

    protected override string Part2(string input)
    {
        do
        {
            input = Next(input);
        } while (!Valid(input));

        do
        {
            input = Next(input);
        } while (!Valid(input));

        return input;
    }

    private static string Next(string input) =>
        string.Create(input.Length, input, static (chars, input) =>
        {
            var done = false;
            for (var i = chars.Length - 1; i >= 0; i--)
            {
                if (done)
                {
                    chars[i] = input[i];
                    continue;
                }
                
                if (input[i] == 'z')
                {
                    chars[i] = 'a';
                }
                else
                {
                    chars[i] = (char)(input[i] + 1);
                    done = true;
                }
            }
        });

    private static bool Valid(string input)
    {
        // 3 increasing letter straight (e.g. bcd)
        var foundStraight = false;
        
        for (var i = 0; i < input.Length - 2; i++)
        {
            var c0 = input[i];
            var c1 = input[i+1];
            var c2 = input[i+2];

            if (c0 + 1 == c1 && c0 + 2 == c2)
            {
                foundStraight = true;
                break;
            }
        }

        if (!foundStraight)
        {
            return false;
        }

        // doesn't contain i o l
        for (var i = 0; i < input.Length; i++)
        {
            if (input[i] is 'i' or 'o' or 'l')
            {
                return false;
            }
        }
        
        // 2 different pairs
        char? pair = null;
        for (var i = 0; i < input.Length - 1; i++)
        {
            if (input[i] == input[i + 1])
            {
                i++;

                if (pair == null)
                {
                    pair = input[i];
                }
                else
                {
                    return true;
                }
            }
        }
        
        return false;
    }
}