using System.Numerics;
using System.Text;

namespace AdventOfCode2016;

[Day]
public partial class Day18 : Day<Day18.Model, int, int>
{
    private const string Sample = @".^^.^.^^^^";
    
    protected override Model Parse(string input) => new(input);

    [Sample(Sample, 38)]
    protected override int Part1(Model input) => Solve(input, input.Row == Sample ? 10 : 40);
    protected override int Part2(Model input) => Solve(input, 400_000);

    private static int Solve(Model input, int rows)
    {
        var width = input.Row.Length;

        var previous = UInt128.Zero;
        for (var x = 0; x < width; x++)
        {
            previous |= input.Row[x] == '^' ? UInt128.One << x : UInt128.Zero;
        }

        var count = UInt128.PopCount(previous);

        var mask = (UInt128.One << (width)) - UInt128.One;
        for (var y = 1; y < rows; y++)
        {
            var next = ((previous >> 1) ^ (previous << 1)) & mask;
            count += UInt128.PopCount(next);
            previous = next;
        }

        return (width * rows) - (int)count;
    }

    public record Model(string Row);
}
