namespace AdventOfCode.Core;

public class Grid<TElement>
{
    private readonly TElement[] _elements;
    
    public int Width { get; }
    public int Height { get; }

    public static Grid<TElement> Empty(int width, int height) => new(new TElement[width * height], width);

    internal Grid(TElement[] elements, int width)
    {
        _elements = elements;
        Width = width;
        Height = _elements.Length / Width;
    }

    public TElement this[Position position]
    {
        get => this[position.X, position.Y];
        set => this[position.X, position.Y] = value;
    }

    public TElement this[int x, int y]
    {
        get => _elements[y * Width + x];
        set => _elements[y * Width + x] = value;
    }

    public TElement Get(Position position) => this[position];

    public bool IsValid(Position position) => position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;
    
    public Grid<TSelect> Select<TSelect>(Func<TElement, TSelect> selector)
    {
        var newGrid = new Grid<TSelect>(new TSelect[_elements.Length], Width);
        
        Parallel.For(0, _elements.Length, i =>
        { 
            newGrid._elements[i] = selector(_elements[i]);
        });
        
        return newGrid;
    }
    public Grid<TSelect> Select<TSelect>(Func<TElement, Grid<TElement>, Position, TSelect> selector)
    {
        var newGrid = new Grid<TSelect>(new TSelect[_elements.Length], Width);

        Parallel.For(0, Height, y =>
        {
            var i = y * Width;
            
            for (var x = 0; x < Width; x++)
            {
                newGrid._elements[i] = selector(_elements[i], this, new Position(x, y));
                i++;
            }
        });
        
        return newGrid;
    }

    public int Count(Func<TElement, bool> predicate) => _elements.Count(predicate);
}

public static class GridParser
{
    public static Grid<bool> ParseBool(string input, char on)
    {
        var lines = input.Split('\n');
        var width = lines[0].Length;

        var grid = new bool[width * lines.Length];
        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (var x = 0; x < line.Length; x++)
            {
                grid[y * width + x] = line[x] == on;
            }
        }
        
        return new Grid<bool>(grid, width);
    } 
}