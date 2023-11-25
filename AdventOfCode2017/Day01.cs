namespace AdventOfCode2017;

[Day]
public partial class Day01 : Day<Day01.Model, int, int>
{
    protected override Model Parse(string input) => new(input.Select(x => x - '0').ToList());

    [Sample("1122", 3)]
    [Sample("1111", 4)]
    [Sample("1234", 0)]
    [Sample("91212129", 9)]
    protected override int Part1(Model input) => input.Digits.Select((x, i) => x == input.Digits[(i+1) % input.Digits.Count] ? x : 0).Sum();

    [Sample("1212", 6)]
    [Sample("1221", 0)]
    [Sample("123425", 4)]
    [Sample("123123", 12)]
    [Sample("12131415", 4)]
    protected override int Part2(Model input) => input.Digits.Select((x, i) => x == input.Digits[(i + input.Digits.Count/2) % input.Digits.Count] ? x : 0).Sum();

    public record Model(IReadOnlyList<int> Digits);
}
