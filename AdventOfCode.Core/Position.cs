namespace AdventOfCode.Core;

public record Position(int X, int Y)
{
    public static readonly Position Identity = new(0, 0);
    public static Position operator +(Position a, Position b) => new(a.X + b.X, a.Y + b.Y);

    public IEnumerable<Position> Neighbours
    {
        get
        {
            for(var y = -1; y <= 1; y++)
            for(var x = -1; x <= 1; x++)
                yield return new Position(X + x, Y + y);
        }
    }
    public IEnumerable<Position> StrictNeighbours
    {
        get
        {
            for(var y = -1; y <= 1; y++)
            for(var x = -1; x <= 1; x++)
                if(x != 0 || y != 0)
                    yield return new Position(X + x, Y + y);
        }
    }
    public IEnumerable<Position> OrthogonalNeighbours
    {
        get
        {
            yield return this with { X = X + 1 };
            yield return this with { X = X - 1 };
            yield return this with { Y = Y + 1 };
            yield return this with { Y = Y - 1 };
        }
    }
}