using System.Numerics;

namespace AdventOfCode.Core;

public readonly struct BigRational :
    IAdditiveIdentity<BigRational, BigRational>,
    IAdditionOperators<BigRational, BigRational, BigRational>,
    ISubtractionOperators<BigRational, BigRational, BigRational>,
    IMultiplicativeIdentity<BigRational, BigRational>,
    IMultiplyOperators<BigRational, BigRational, BigRational>,
    IDivisionOperators<BigRational, BigRational, BigRational>
{
    private readonly BigInteger? _denominator;

    public BigRational(BigInteger numerator, BigInteger denominator)
    {
        if (denominator.IsZero) throw new ArgumentException("cannot have zero denominator", nameof(denominator));
        
        var gcd = NumberExtensions.GreatestCommonDenominator(numerator, denominator);
        
        Numerator = numerator / gcd;
        _denominator = denominator / gcd;
    }

    public BigInteger Numerator { get; }
    public BigInteger Denominator => _denominator ?? BigInteger.One;

    public bool IsZero => Numerator.IsZero;

    public override string ToString()
    {
        return Denominator.IsOne ? Numerator.ToString() : IsZero ? "0" : $"{Numerator} / {Denominator}";
    }
    
    public static implicit operator BigRational(long value) => new(value, BigInteger.One);
    
    public static BigRational AdditiveIdentity { get; } = new(0, 1);
    public static BigRational operator +(BigRational left, BigRational right) => new(left.Numerator * right.Denominator + right.Numerator * left.Denominator, left.Denominator * right.Denominator);

    public static BigRational operator -(BigRational left, BigRational right) => new(left.Numerator * right.Denominator - right.Numerator * left.Denominator, left.Denominator * right.Denominator);
    
    public static BigRational MultiplicativeIdentity { get; } = new(1, 1);
    public static BigRational operator *(BigRational left, BigRational right) => new(left.Numerator * right.Numerator, left.Denominator * right.Denominator);

    public static BigRational operator /(BigRational left, BigRational right) => new(left.Numerator * right.Denominator, left.Denominator * right.Numerator);
}