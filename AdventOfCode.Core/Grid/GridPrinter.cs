using System.Diagnostics.Contracts;
using System.Text;

namespace AdventOfCode.Core;

public static class GridPrinter
{
    [Pure]
    public static string Print<T>(this Grid<T> grid, Func<T, char> format)
    {
        var sb = new StringBuilder();

        var w = grid.Width;
        var h = grid.Height;

        for (var y = 0; y < h; y++)
        {
            for (var x = 0; x < w; x++)
            {
                sb.Append(format(grid[x, y]));
            }

            sb.Append('\n');
        }

        return sb.ToString();
    }
}