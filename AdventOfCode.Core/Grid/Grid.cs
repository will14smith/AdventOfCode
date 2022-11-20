namespace AdventOfCode.Core;

public static class Grid
{
    public static Grid<TElement> Empty<TElement>(int width, int height) => new(new TElement[width * height], width);
} 

public class Grid<TElement>
{
    internal readonly TElement[] _elements;
    
    public int Width { get; }
    public int Height { get; }

    public static Grid<TElement> Empty(int width, int height) => new(new TElement[width * height], width);

    public Grid(TElement[] elements, int width)
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
}