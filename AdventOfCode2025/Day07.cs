namespace AdventOfCode2025;

[Day]
public partial class Day07 : Day<Day07.Model, int, int>
{
    public record Model;

    protected override Model Parse(string input) => new();

    [Sample("", 1)]
    protected override int Part1(Model input) => 0;
    [Sample("", 1)]
    protected override int Part2(Model input) => 0;
}