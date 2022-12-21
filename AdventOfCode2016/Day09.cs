namespace AdventOfCode2016;

[Day]
public partial class Day09 : Day<string, long, long>
{
    protected override string Parse(string input) => input;

    [Sample("ADVENT", 6L)]
    [Sample("A(1x5)BC", 7L)]
    [Sample("A(2x2)BCD(2x2)EFG", 11L)]
    [Sample("(6x1)(1x3)A", 6L)]
    [Sample("X(8x2)(3x3)ABCY", 18L)]
    protected override long Part1(string input) => Length(input);

    [Sample("(3x3)XYZ", 9L)]
    [Sample("X(8x2)(3x3)ABCY", 20L)]
    [Sample("(27x12)(20x12)(13x14)(7x10)(1x12)A", 241920L)]
    [Sample("(25x3)(3x3)ABC(2x3)XY(5x2)PQRSTX(18x9)(3x2)TWO(5x7)SEVEN", 445L)]
    protected override long Part2(string input) => Length(input, true);

    private static long Length(string input, bool allowRecursion = false)
    {
        var outputLength = 0L;

        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            switch (c)
            {
                case >= 'A' and <= 'Z': outputLength++; break;
                
                case '(':
                    i++;
                    (i, var n) = ReadInt(input, i);
                    if (input[i] != 'x') throw new InvalidOperationException($"invalid input, expecting 'x' at {i}");
                    i++;
                    (i, var m) = ReadInt(input, i);
                    if (input[i] != ')') throw new InvalidOperationException($"invalid input, expecting ')' at {i}");

                    var substringLength = allowRecursion ? Length(input[(i+1)..(i + n + 1)], true) : n;

                    outputLength += m * substringLength;
                    i += n;
                    
                    break;
                
                default: throw new InvalidOperationException($"unexpected char '{c}'");
            }
        }

        return outputLength;
    }

    private static (int i, int n) ReadInt(string input, int i)
    {
        var n = 0;
        while (char.IsDigit(input[i]))
        {
            n = n * 10 + (input[i++] - '0');
        }
        return (i, n);
    }
}
