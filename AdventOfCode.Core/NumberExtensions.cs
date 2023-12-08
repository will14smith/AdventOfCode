using System.Numerics;

namespace AdventOfCode.Core;

public static class NumberExtensions
{
    private static T Abs<T>(T number) where T : INumber<T> => number < T.AdditiveIdentity ? -number : number;

    public static T LowestCommonMultiple<T>(T a, T b) where T : INumber<T>
    {
        return Abs(a * b) / GreatestCommonDenominator(a, b);
    }
    
    public static T GreatestCommonDenominator<T>(T a, T b) where T : INumber<T>
    {
        while (b != T.AdditiveIdentity)
        {
            (a, b) = (b, a % b);
        }
        
        return a;
    }
}