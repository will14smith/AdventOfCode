namespace AdventOfCode.Core;

public record struct Position(int X, int Y)
{
    public static readonly Position Identity = new(0, 0);
    
    public static Position operator +(Position a, Position b) => new(a.X + b.X, a.Y + b.Y);
    public static Position operator -(Position a, Position b) => new(a.X - b.X, a.Y - b.Y);
    public static Position operator *(Position a, int scale) => new(a.X * scale, a.Y * scale);
    public static Position operator *(int scale, Position a) => new(scale * a.X, scale * a.Y);
    public static Position operator /(Position a, int scale) => new(a.X / scale, a.Y / scale);
    
    public static Position operator -(Position a) => new(-a.X, -a.Y);
}

public static class PositionLengthExtensions
{
    public static int TaxiDistance(this Position position) => position.BlockDistance();
    public static int BlockDistance(this Position position) => Math.Abs(position.X) + Math.Abs(position.Y);
}