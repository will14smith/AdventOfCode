using System.Text;

namespace AdventOfCode2017;

[Day]
public partial class Day10 : Day<Day10.Model, int, string>
{
    protected override Model Parse(string input) => new (input);

    protected override int Part1(Model input)
    {
        var lengths = input.Value.Split(',').Select(byte.Parse).ToList();
        
        var hash = new KnotHash(256);
        hash.Round(lengths);
        return hash.Values[0] * hash.Values[1];
    }

    protected override string Part2(Model input)
    {
        var newInput = Encoding.ASCII.GetBytes(input.Value).ToArray();
        var dense = KnotHash.Standard(newInput);

        return Convert.ToHexString(dense).ToLower();
    }

    public record Model(string Value);
}