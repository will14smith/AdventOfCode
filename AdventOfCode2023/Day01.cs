namespace AdventOfCode2023;

[Day]
public partial class Day01 : LineDay<Day01.Model, int, int>
{
    public record Model(string Line);

    [Sample("1abc2\npqr3stu8vwx\na1b2c3d4e5f\ntreb7uchet", 142)]
    protected override int Part1(IEnumerable<Model> input) => Solve(input, false);

    [Sample("two1nine\neightwothree\nabcone2threexyz\nxtwone3four\n4nineeightseven2\nzoneight234\n7pqrstsixteen", 281)]
    protected override int Part2(IEnumerable<Model> input) => Solve(input, true);

    private int Solve(IEnumerable<Model> input, bool includeWords) => input
        .Select(x => GetDigits(x.Line, includeWords).ToArray())
        .Sum(x => x[0] * 10 + x[^1]);

    private IEnumerable<int> GetDigits(string input, bool includeWords)
    {
        for (var i = 0; i < input.Length; i++)
        {
            if (char.IsDigit(input[i]))
            {
                yield return input[i] - '0';
                continue;
            }

            if (!includeWords)
            {
                continue;
            }

            var substr = input[i..];
            if (substr.StartsWith("one")) yield return 1;
            else if (substr.StartsWith("two")) yield return 2;
            else if (substr.StartsWith("three")) yield return 3;
            else if (substr.StartsWith("four")) yield return 4;
            else if (substr.StartsWith("five")) yield return 5;
            else if (substr.StartsWith("six")) yield return 6;
            else if (substr.StartsWith("seven")) yield return 7;
            else if (substr.StartsWith("eight")) yield return 8;
            else if(substr.StartsWith("nine")) yield return 9;
        }
    }

    protected override Model ParseLine(string input) => new(input);
}