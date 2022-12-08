namespace AdventOfCode.Core;

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
    
    public static Grid<int> ParseInt(string input)
    {
        var lines = input.Split('\n');
        var width = lines[0].Length;

        var grid = new int[width * lines.Length];
        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (var x = 0; x < line.Length; x++)
            {
                grid[y * width + x] = line[x] - '0';
            }
        }
        
        return new Grid<int>(grid, width);
    } 

}