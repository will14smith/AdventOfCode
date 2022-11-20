namespace AdventOfCode.Core;

public static class PositionNeighbourExtensions
{
    public static IEnumerable<Position> Neighbours(this Position position)
    {
        for (var y = -1; y <= 1; y++)
        for (var x = -1; x <= 1; x++)
            yield return new Position(position.X + x, position.Y + y);
    }
    public static IEnumerable<Position> StrictNeighbours(this Position position)
    {
        for(var y = -1; y <= 1; y++)
        for(var x = -1; x <= 1; x++)
            if(x != 0 || y != 0)
                yield return new Position(position.X + x, position.Y + y);
    }
    public static IEnumerable<Position> OrthogonalNeighbours(this Position position)
    {
        yield return position with { X = position.X + 1 };
        yield return position with { X = position.X - 1 };
        yield return position with { Y = position.Y + 1 };
        yield return position with { Y = position.Y - 1 };
    }
}