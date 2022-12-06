namespace AdventOfCode2022;

[Day]
public partial class Day06 : Day<string, int, int>
{
    protected override string Parse(string input) => input;

    [Sample("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 7)]
    [Sample("bvwbjplbgvbhsrlpgdmjqwftvncz", 5)]
    [Sample("nppdvjthqldpwncqszvftbrmjlhg", 6)]
    [Sample("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 10)]
    [Sample("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 11)]
    protected override int Part1(string input) => FindStarter(input, 4);

    [Sample("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 19)]
    [Sample("bvwbjplbgvbhsrlpgdmjqwftvncz", 23)]
    [Sample("nppdvjthqldpwncqszvftbrmjlhg", 23)]
    [Sample("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 29)]
    [Sample("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 26)]
    protected override int Part2(string input) => FindStarter(input, 14);

    private static int FindStarter(string input, int length)
    {
        for (var i = length; i < input.Length; i++)
        {
            var chars = input[(i - length)..i];
            if (chars.ToHashSet().Count == length)
            {
                return i;
            }
        }

        throw new Exception("no solution");
    }
}
