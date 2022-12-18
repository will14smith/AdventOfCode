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

    public Position3(int x, int y, int z)
    {
        _elems = Vector128.Create(x, y, z, 0);
    }
    private Position3(Vector128<int> elems)
    {
        _elems = elems;
    }

    public static Position3 operator +(Position3 a, Position3 b) => new(Sse2.Add(a._elems, b._elems));
    // these comparisons check is _any_ element matches the operator
    public static bool operator <(Position3 a, Position3 b) => Sse2.MoveMask(Sse2.CompareLessThan(a._elems, b._elems).AsByte()) != 0;
    public static bool operator >(Position3 a, Position3 b) => Sse2.MoveMask(Sse2.CompareGreaterThan(a._elems, b._elems).AsByte()) != 0;
    
    // these comparisons perform the operation element-wise
    public static Position3 Min(Position3 a, Position3 b) => new(Sse41.Min(a._elems, b._elems));
    public static Position3 Max(Position3 a, Position3 b) => new(Sse41.Max(a._elems, b._elems));

    public override bool Equals(object? obj) => obj is Position3 other && _elems.Equals(other._elems);
    public override int GetHashCode() => _elems.GetHashCode();

    public override string ToString() => $"{{ X={_elems.GetElement(0)}, Y={_elems.GetElement(1)}, Z={_elems.GetElement(2)} }}";
}

public static class Position3NeighbourExtensions
{
    public static IEnumerable<Position3> OrthogonalNeighbours(this Position3 position) => Position3.OrthogonalNeighbours.Select(delta => position + delta);
}
