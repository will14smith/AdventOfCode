namespace AdventOfCode.Core;

public static class PositionRotationExtensions
{
    private static readonly IReadOnlyDictionary<int, (int, int)> Trig = new Dictionary<int, (int Cos, int Sin)>
    {
        { 0, (1, 0) },
        { 90, (0, -1) },
        { 180, (-1, 0) },
        { 270, (0, 1) },
    };

    public static Position RotateCW(this Position position, int degrees)
    {
        var (cosDelta, sinDelta) = Trig[degrees];

        return new Position(
            position.X * cosDelta - position.Y * sinDelta,
            position.X * sinDelta + position.Y * cosDelta);
    }
    public static Position RotateCCW(this Position position, int degrees)
    {
        var (cosDelta, sinDelta) = Trig[360 - degrees];

        return new Position(
            position.X * cosDelta - position.Y * sinDelta,
            position.X * sinDelta + position.Y * cosDelta);
    }
}