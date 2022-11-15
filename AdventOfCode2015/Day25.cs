using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2015;

[Day]
public partial class Day25 : ParseDay<Day25.Model, long, long>
{
    protected override TextParser<Model> Parser => Span.EqualTo("To continue, please consult the code grid in the manual.  Enter the code at row ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(", column ")).Then(Numerics.IntegerInt32)
        .Select(x => new Model(x.Item1, x.Item2));

    protected override long Part1(Model input)
    {
        var offset = ModelToOffset(input);

        return GenerateCodeAtOffset(offset);
    }
    
    protected override long Part2(Model input) => 0;

    private static long ModelToOffset(Model model)
    {
        var col = model.Column;
        var row = model.Row;
     
        // y = 1: (n * (n+1)) / 2
        // y = 2: (n * (n+3)) / 2
        // y = 3: (n * (n+5)) / 2 + 1
        // y = 4: (n * (n+7)) / 2 + 3
        // y = 5: (n * (n+9)) / 2 + 6 
        // y = 6: (n * (n+11)) / 2 + 10
        
        return col * (col + row * 2 - 1) / 2 + (row - 1) * (row - 2) / 2;
    }

    private static long GenerateCodeAtOffset(long offset)
    {
        // f(0) = 20151125
        // f(n) = f(n-1) * 252533 mod 33554393
        // f(n) = 20151125 * 252533^n mod 33554393

        var b = 252533L;
        var e = offset - 1;
        const long m = 33554393L;

        var r = 1L;
        while (e > 0)
        {
            if (e % 2 == 1)
            {
                r = r * b % m;
            }
            e >>= 1;
            b = b * b % m;
        }
        
        return 20151125L * r % m;
    }

    
    public record Model(int Row, int Column);
}