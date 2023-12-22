using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace AdventOfCode.Core;

public class Position3
{
    private readonly Vector128<int> _elems;

    public static readonly IReadOnlyList<Position3> OrthogonalNeighbours = new[]
    {
        new Position3(1, 0, 0),
        new Position3(-1, 0, 0),
        new Position3(0, 1, 0),
        new Position3(0, -1, 0),
        new Position3(0, 0, 1),
        new Position3(0, 0, -1),
    };

    public static readonly Position3 MinValue = new(int.MinValue, int.MinValue, int.MinValue);
    public static readonly Position3 MaxValue = new(int.MaxValue, int.MaxValue, int.MaxValue);

    public int X => _elems[0];
    public int Y => _elems[1];
    public int Z => _elems[2];
    
    public Position3(int x, int y, int z)
    {
        _elems = Vector128.Create(x, y, z, 0);
    }
    private Position3(Vector128<int> elems)
    {
        _elems = elems;
    }

    public static Position3 operator +(Position3 a, Position3 b) => new(Sse2.Add(a._elems, b._elems));
    public static Position3 operator -(Position3 a, Position3 b) => new(Sse2.Subtract(a._elems, b._elems));
    public static Position3 operator *(int a, Position3 b) => new(Avx.MultiplyLow(Vector128.Create(a), b._elems));
    public static Position3 operator *(Position3 a, int b) => b * a;
    
    // these comparisons check is _any_ element matches the operator
    public static bool operator <(Position3 a, Position3 b) => Sse2.MoveMask(Sse2.CompareLessThan(a._elems, b._elems).AsByte()) != 0;
    public static bool operator >(Position3 a, Position3 b) => Sse2.MoveMask(Sse2.CompareGreaterThan(a._elems, b._elems).AsByte()) != 0;
    
    // these comparisons perform the operation element-wise
    public static Position3 Min(Position3 a, Position3 b) => new(Sse41.Min(a._elems, b._elems));
    public static Position3 Max(Position3 a, Position3 b) => new(Sse41.Max(a._elems, b._elems));

    public static Position3 Abs(Position3 a) => new(Ssse3.Abs(a._elems).AsInt32());
    public static int SumElements(Position3 a) => a._elems[0] + a._elems[1] + a._elems[2] + a._elems[3];
    
    public override bool Equals(object? obj) => obj is Position3 other && _elems.Equals(other._elems);
    public override int GetHashCode() => _elems.GetHashCode();

    public override string ToString() => $"{{ X={_elems.GetElement(0)}, Y={_elems.GetElement(1)}, Z={_elems.GetElement(2)} }}";
}

public static class Position3NeighbourExtensions
{
    public static int TaxiDistance(this Position3 position) => position.BlockDistance();
    public static int BlockDistance(this Position3 position) => Position3.SumElements(Position3.Abs(position));
    
    public static IEnumerable<Position3> OrthogonalNeighbours(this Position3 position) => Position3.OrthogonalNeighbours.Select(delta => position + delta);
}
