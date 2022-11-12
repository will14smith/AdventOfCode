namespace AdventOfCode2015;

[Day]
public partial class Day05 : Day<string[], int, int>
{
    protected override string[] Parse(string input) => input.Split('\n');

    [Sample("ugknbfddgicrmopn", 1)]
    [Sample("aaa", 1)]
    [Sample("jchzalrnumimnmhp", 0)]
    [Sample("haegwjzuvuyypxyu", 0)]
    [Sample("dvszwmarrgswjxmb", 0)]
    protected override int Part1(string[] input) => input.Count(IsNice1);

    [Sample("xyxy", 1)]
    [Sample("aaa", 0)]
    [Sample("qjhvhtzxzqqjkmpb", 1)]
    [Sample("xxyxx", 1)]
    [Sample("uurcxstgmygtbstg", 0)]
    [Sample("ieodomkazucvgmuy", 0)]
    protected override int Part2(string[] input) => input.Count(IsNice2);


    private static bool IsNice1(string input)
    {
        var vowels = 0;
        var hasDouble = false;
        char last = '\0';

        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];

            if (c is 'a' or 'e' or 'i' or 'o' or 'u' ) vowels++;
           
            if (c == last) hasDouble = true;
            else if (last == 'a' && c == 'b') return false;
            else if (last == 'c' && c == 'd') return false;
            else if (last == 'p' && c == 'q') return false;
            else if (last == 'x' && c == 'y') return false;
            
            last = c;
        }
        
        return vowels >= 3 && hasDouble;
    }

    private static bool IsNice2(string input)
    {
        var foundPair = false;
        var foundSpaced = false;
        
        for (var i = 0; i < input.Length - 2; i++)
        {
            var a = input[i];
            var b = input[i + 1];
            
            if (input[i+2] == a)
            {
                foundSpaced = true;
            }

            for (var j = i + 2; j < input.Length - 1; j++)
            {
                if (input[j] == a && input[j + 1] == b)
                {
                    foundPair = true;
                }
            }
        }
        
        return foundPair && foundSpaced;
    }
}