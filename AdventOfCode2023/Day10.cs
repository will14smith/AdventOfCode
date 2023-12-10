namespace AdventOfCode2023;

[Day]
public partial class Day10 : Day<Day10.Model, int, int>
{
    protected override Model Parse(string input) => new(GridParser.ParseChar(input, Parse));
    private static Cell Parse(char input) =>
        input switch
        {
            '.' => Cell.Ground,
            'S' => Cell.Start,

            '|' => Cell.NorthSouth,
            '-' => Cell.EastWest,
            'L' => Cell.NorthEast,
            'J' => Cell.NorthWest,
            '7' => Cell.SouthWest,
            'F' => Cell.SouthEast,
            
            _ => throw new ArgumentOutOfRangeException(nameof(input), input, null)
        };

    [Sample(".....\n.S-7.\n.|.|.\n.L-J.\n.....", 4)]
    [Sample("..F7.\n.FJ|.\nSJ.L7\n|F--J\nLJ...", 8)]
    [Sample("-L|F7\n7S-7|\nL|7||\n-L-J|\nL|-JF", 4)]
    [Sample("7-F7-\n.FJ|7\nSJLL7\n|F--J\nLJ.LJ", 8)]
    protected override int Part1(Model input) => new PipeBuilder(input.Grid).Build().Count / 2;
    
    [Sample(".....\n.S-7.\n.|.|.\n.L-J.\n.....", 1)]
    [Sample("...........\n.S-------7.\n.|F-----7|.\n.||.....||.\n.||.....||.\n.|L-7.F-J|.\n.|..|.|..|.\n.L--J.L--J.\n...........", 4)]
    [Sample("..........\n.S------7.\n.|F----7|.\n.||....||.\n.||....||.\n.|L-7F-J|.\n.|..||..|.\n.L--JL--J.\n..........", 4)]
    [Sample(".F----7F7F7F7F-7....\n.|F--7||||||||FJ....\n.||.FJ||||||||L7....\nFJL7L7LJLJ||LJ.L-7..\nL--J.L7...LJS7F-7L7.\n....F-J..F7FJ|L7L7L7\n....L7.F7||L7|.L7L7|\n.....|FJLJ|FJ|F7|.LJ\n....FJL-7.||.||||...\n....L---J.LJ.LJLJ...", 8)]
    protected override int Part2(Model input)
    {
        var grid = input.Grid;

        var builder = new PipeBuilder(grid);
        var pipe = builder.Build();

        // remove distractions
        var cleanGrid = BuildCleanGrid(grid, pipe, builder.StartNeighbours);
        // expand 1x1 pipes to 3x3 pipes to allow fitting between
        var expandedGrid = BuildExpandedGrid(cleanGrid, pipe);
        // find all the cells that can reach the outside
        var outsideExpandedGrid = FloodFill(expandedGrid, new Position(0, 0));
        // collapse back to a 1x1 grid
        var outsideArea = grid.Keys().Count(position => outsideExpandedGrid[position * 3 + new Position(1, 1)]);

        // total area - pipe area - outside area
        return grid.Keys().Count() - pipe.Count - outsideArea;
    }

    private static Grid<Cell> BuildCleanGrid(Grid<Cell> grid, IEnumerable<Position> pipe, Neighbours start)
    {
        var cleanGrid = Grid<Cell>.Empty(grid.Width, grid.Height);
        
        foreach (var position in pipe)
        {
            var cell = grid[position];
            if (cell == Cell.Start)
            {
                cell = start switch
                {
                    { North: true, South: true } => Cell.NorthSouth,
                    { North: true, East: true } => Cell.NorthEast,
                    { North: true, West: true } => Cell.NorthWest,
                    { South: true, East: true } => Cell.SouthEast,
                    { South: true, West: true } => Cell.SouthWest,
                    { East: true, West: true } => Cell.EastWest,
                    _ => throw new NotImplementedException()
                };
            }
            
            cleanGrid[position] = cell;
        }

        return cleanGrid;
    }
    
    private static Grid<bool> BuildExpandedGrid(Grid<Cell> grid, IReadOnlySet<Position> loop)
    {
        var expandedGrid = Grid<bool>.Empty(grid.Width * 3, grid.Height * 3);
        
        foreach (var position in loop)
        {
            var cell = grid[position];
            
            switch (cell)
            {
                case Cell.Ground: break;
                case Cell.Start: break;
                
                case Cell.NorthSouth:
                    expandedGrid[position * 3 + new Position(1, 0)] = true;
                    expandedGrid[position * 3 + new Position(1, 1)] = true;
                    expandedGrid[position * 3 + new Position(1, 2)] = true;
                    break;
                
                case Cell.EastWest:
                    expandedGrid[position * 3 + new Position(0, 1)] = true;
                    expandedGrid[position * 3 + new Position(1, 1)] = true;
                    expandedGrid[position * 3 + new Position(2, 1)] = true;
                    break;
                
                case Cell.NorthEast:
                    expandedGrid[position * 3 + new Position(1, 0)] = true;
                    expandedGrid[position * 3 + new Position(1, 1)] = true;
                    expandedGrid[position * 3 + new Position(2, 1)] = true;
                    break;
                
                case Cell.NorthWest:
                    expandedGrid[position * 3 + new Position(1, 0)] = true;
                    expandedGrid[position * 3 + new Position(1, 1)] = true;
                    expandedGrid[position * 3 + new Position(0, 1)] = true;
                    break;
                
                case Cell.SouthWest:
                    expandedGrid[position * 3 + new Position(0, 1)] = true;
                    expandedGrid[position * 3 + new Position(1, 1)] = true;
                    expandedGrid[position * 3 + new Position(1, 2)] = true;
                    break;
                
                case Cell.SouthEast:
                    expandedGrid[position * 3 + new Position(1, 1)] = true;
                    expandedGrid[position * 3 + new Position(2, 1)] = true;
                    expandedGrid[position * 3 + new Position(1, 2)] = true;
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }
        }

        return expandedGrid;
    }
    
    private Grid<bool> FloodFill(Grid<bool> grid, Position start)
    {
        var filledGrid = Grid<bool>.Empty(grid.Width, grid.Height);

        var search = new Queue<Position>();
        search.Enqueue(start);

        while (search.Count > 0)
        {
            var position = search.Dequeue();
            if (filledGrid[position]) continue;
            filledGrid[position] = true;

            if (grid[position]) continue;
            
            foreach (var neighbour in position.OrthogonalNeighbours())
            {
                if (!filledGrid.IsValid(neighbour)) continue;
                if(!grid[neighbour]) search.Enqueue(neighbour);
            }
        }
        
        return filledGrid;
    }

    public record Model(Grid<Cell> Grid);
    public record Neighbours(bool North, bool South, bool East, bool West);
    
    public enum Cell
    {
        Ground,
        Start,
        
        NorthSouth,
        EastWest,
        NorthEast,
        NorthWest,
        SouthWest,
        SouthEast,
    }
}