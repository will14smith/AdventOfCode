namespace AdventOfCode.Core;

public static class CharExtensions
{
    public static bool IsDigit(this char c) => c is >= '0' and <= '9';
    public static bool IsHexChar(this char c) => c.IsDigit() || c is >= 'a' and <= 'f' or >= 'A' and <= 'F';
}