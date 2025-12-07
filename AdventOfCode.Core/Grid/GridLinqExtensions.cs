namespace AdventOfCode.Core;

public static class GridLinqExtensions
{
    public static Position First<TElement>(this Grid<TElement> grid, Func<TElement, bool> predicate)
    {
        for (var y = 0; y < grid.Height; y++)
        for (var x = 0; x < grid.Width; x++)
        {
            var element = grid[x, y];
            if (predicate(element))
            {
                return new Position(x, y);
            }
        }

        throw new InvalidOperationException("No element satisfies the condition in predicate.");
    }
    
    public static Grid<TSelect> Select<TElement, TSelect>(this Grid<TElement> grid, Func<TElement, TSelect> selector)
    {
        var elementsLength = grid._elements.Length;
        var newGrid = new Grid<TSelect>(new TSelect[elementsLength], grid.Width);
        
        Parallel.For(0, elementsLength, i =>
        { 
            newGrid._elements[i] = selector(grid._elements[i]);
        });
        
        return newGrid;
    }
    public static Grid<TSelect> Select<TElement, TSelect>(this Grid<TElement> grid, Func<TElement, Grid<TElement>, Position, TSelect> selector)
    {
        var elementsLength = grid._elements.Length;
        var newGrid = new Grid<TSelect>(new TSelect[elementsLength], grid.Width);

        Parallel.For(0, grid.Height, y =>
        {
            var i = y * grid.Width;
            
            for (var x = 0; x < grid.Width; x++)
            {
                newGrid._elements[i] = selector(grid._elements[i], grid, new Position(x, y));
                i++;
            }
        });
        
        return newGrid;
    }

    public static int Count<TElement>(this Grid<TElement> grid, Func<TElement, bool> predicate) => grid._elements.Count(predicate);
}