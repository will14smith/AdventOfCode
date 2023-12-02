using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace AdventOfCode.Core;

public class LongPosition3
{
    private readonly Vector256<long> _elems;

    public static readonly IReadOnlyList<LongPosition3> OrthogonalNeighbours = new[]
    {
        new LongPosition3(1, 0, 0),
        new LongPosition3(-1, 0, 0),
        new LongPosition3(0, 1, 0),
        new LongPosition3(0, -1, 0),
        new LongPosition3(0, 0, 1),
        new LongPosition3(0, 0, -1),
    };

    public static readonly LongPosition3 MinValue = new(long.MinValue, long.MinValue, long.MinValue);
    public static readonly LongPosition3 MaxValue = new(long.MaxValue, long.MaxValue, long.MaxValue);

    public long X => _elems[0];
    public long Y => _elems[1];
    public long Z => _elems[2];
    
    public LongPosition3(long x, long y, long z)
    {
        _elems = Vector256.Create(x, y, z, 0);
    }
    private LongPosition3(Vector256<long> elems)
    {
        _elems = elems;
    }

    public static LongPosition3 operator +(LongPosition3 a, LongPosition3 b) => new(Avx2.Add(a._elems, b._elems));
    public static LongPosition3 operator *(int a, LongPosition3 b) => new(Vector256.Multiply(b._elems, a));
    public static LongPosition3 operator *(LongPosition3 a, int b) => b * a;
    
    
    public static LongPosition3 Abs(LongPosition3 a) => new(Vector256.Abs(a._elems));
    public static long SumElements(LongPosition3 a) => a._elems[0] + a._elems[1] + a._elems[2] + a._elems[3];
    
    public override bool Equals(object? obj) => obj is LongPosition3 other && _elems.Equals(other._elems);
    public override int GetHashCode() => _elems.GetHashCode();

    public override string ToString() => $"{{ X={_elems.GetElement(0)}, Y={_elems.GetElement(1)}, Z={_elems.GetElement(2)} }}";
}

public static class LongPosition3NeighbourExtensions
{
    public static long TaxiDistance(this LongPosition3 position) => position.BlockDistance();
    public static long BlockDistance(this LongPosition3 position) => LongPosition3.SumElements(LongPosition3.Abs(position));
    
    public static IEnumerable<LongPosition3> OrthogonalNeighbours(this LongPosition3 position) => LongPosition3.OrthogonalNeighbours.Select(delta => position + delta);
}
