namespace AdventOfCode.Core;

public static class GridEnumerationExtensions
{
    public static bool IsValid<TElement>(this Grid<TElement> grid, Position position) => position.X >= 0 && position.X < grid.Width && position.Y >= 0 && position.Y < grid.Height;

    public static IEnumerable<Position> Keys<TElement>(this Grid<TElement> grid)
    {
        for (var y = 0; y < grid.Height; y++)
        for (var x = 0; x < grid.Width; x++)
        {
            yield return new Position(x, y);
        }
    }
}