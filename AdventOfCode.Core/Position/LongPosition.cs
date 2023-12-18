namespace AdventOfCode.Core;

public record struct LongPosition(long X, long Y)
{
    public static readonly LongPosition Identity = new(0, 0);
    
    public static LongPosition operator +(LongPosition a, LongPosition b) => new(a.X + b.X, a.Y + b.Y);
    public static LongPosition operator -(LongPosition a, LongPosition b) => new(a.X - b.X, a.Y - b.Y);
    public static LongPosition operator *(LongPosition a, int scale) => new(a.X * scale, a.Y * scale);
    public static LongPosition operator *(int scale, LongPosition a) => new(scale * a.X, scale * a.Y);
    public static LongPosition operator /(LongPosition a, int scale) => new(a.X / scale, a.Y / scale);
    
    public static LongPosition operator -(LongPosition a) => new(-a.X, -a.Y);
}