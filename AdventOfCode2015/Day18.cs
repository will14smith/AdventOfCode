namespace AdventOfCode2015;

[Day]
public partial class Day18 : Day<Grid<bool>, int, int>
{
    protected override Grid<bool> Parse(string input) => GridParser.ParseBool(input, '#');

    protected override int Part1(Grid<bool> input)
    {
        for (int i = 0; i < 100; i++)
        {
            input = Apply(input);
        }

        return input.Count(x => x);
    }
    
    protected override int Part2(Grid<bool> input)
    {
        input = Broken(input);
        
        for (int i = 0; i < 100; i++)
        {
            input = Apply(input);
            input = Broken(input);
        }

        return input.Count(x => x);
    }
    
    private static Grid<bool> Apply(Grid<bool> grid)
    {
        return grid.Select(static (cell, grid, position) =>
        {
            var neighbours = position.StrictNeighbours().Where(grid.IsValid).Select(grid.Get).Count(x => x);
            return cell ? neighbours is 2 or 3 : neighbours is 3;
        });
    }
    
    private static Grid<bool> Broken(Grid<bool> grid)
    {
        grid[0, 0] = true;
        grid[grid.Width - 1, 0] = true;
        grid[0, grid.Height - 1] = true;
        grid[grid.Width - 1, grid.Height - 1] = true;

        return grid;
    }
}