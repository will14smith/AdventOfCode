namespace AdventOfCode2025;

[Day]
public partial class Day12 : Day<Day12.Model, int, int>
{
    public record Model;

    protected override Model Parse(string input) => new();

    [Sample("", 1)]
    protected override int Part1(Model input) => 0;
    [Sample("", 1)]
    protected override int Part2(Model input) => 0;
}