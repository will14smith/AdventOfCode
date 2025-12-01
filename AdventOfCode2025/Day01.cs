namespace AdventOfCode2025;

[Day]
public partial class Day01 : Day<Day01.Model, int, int>
{
    public record Model;

    protected override Model Parse(string input) => new();

    [Sample("", 1)]
    protected override int Part1(Model input) => 0;
    [Sample("", 1)]
    protected override int Part2(Model input) => 0;
}