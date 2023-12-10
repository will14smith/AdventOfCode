namespace AdventOfCode2023;

[Day]
public partial class Day10 : Day<Day10.Model, int, int>
{
    protected override Model Parse(string input) => new(GridParser.ParseChar(input, Parse));
    private Cell Parse(char input) =>
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
        };

    [Sample(".....\n.S-7.\n.|.|.\n.L-J.\n.....", 4)]
    [Sample("..F7.\n.FJ|.\nSJ.L7\n|F--J\nLJ...", 8)]
    [Sample("-L|F7\n7S-7|\nL|7||\n-L-J|\nL|-JF", 4)]
    [Sample("7-F7-\n.FJ|7\nSJLL7\n|F--J\nLJ.LJ", 8)]
    protected override int Part1(Model input)
    {
        var grid = input.Grid;
        var loop = new HashSet<Position>();

        var start = grid.Keys().First(x => grid[x] == Cell.Start);
        var search = new Queue<Position>();
        search.Enqueue(start);
        
        while (search.Count > 0)
        {
            var pos = search.Dequeue();
            if (!loop.Add(pos))
            {
                continue;
            }

            var outs = grid[pos] switch
            {
                Cell.Start => (North: true, South: true, East: true, West: true),

                Cell.NorthSouth => (North: true, South: true, East: false, West: false),
                Cell.EastWest => (North: false, South: false, East: true, West: true),
                Cell.NorthEast => (North: true, South: false, East: true, West: false),
                Cell.NorthWest => (North: true, South: false, East: false, West: true),
                Cell.SouthWest => (North: false, South: true, East: false, West: true),
                Cell.SouthEast => (North: false, South: true, East: true, West: false),
            };

            if (outs.North)
            {
                var north = pos + new Position(0, -1);
                if (grid.IsValid(north) && grid[north] is Cell.NorthSouth or Cell.SouthEast or Cell.SouthWest)
                {
                    search.Enqueue(north);
                }
            }

            if (outs.South)
            {
                var south = pos + new Position(0, 1);
                if (grid.IsValid(south) && grid[south] is Cell.NorthSouth or Cell.NorthEast or Cell.NorthWest)
                {
                    search.Enqueue(south);
                }
            }

            if (outs.East)
            {
                var east = pos + new Position(1, 0);
                if (grid.IsValid(east) && grid[east] is Cell.EastWest or Cell.NorthWest or Cell.SouthWest)
                {
                    search.Enqueue(east);
                }
            }

            if (outs.West)
            {
                var west = pos + new Position(-1, 0);
                if (grid.IsValid(west) && grid[west] is Cell.EastWest or Cell.NorthEast or Cell.SouthEast)
                {
                    search.Enqueue(west);
                }
            }
        }
        
        return loop.Count / 2;
    }
    
    // [Sample(".....\n.S-7.\n.|.|.\n.L-J.\n.....", 1)]
    // [Sample("...........\n.S-------7.\n.|F-----7|.\n.||.....||.\n.||.....||.\n.|L-7.F-J|.\n.|..|.|..|.\n.L--J.L--J.\n...........", 4)]
    // [Sample("..........\n.S------7.\n.|F----7|.\n.||....||.\n.||....||.\n.|L-7F-J|.\n.|..||..|.\n.L--JL--J.\n..........", 4)]
    [Sample(".F----7F7F7F7F-7....\n.|F--7||||||||FJ....\n.||.FJ||||||||L7....\nFJL7L7LJLJ||LJ.L-7..\nL--J.L7...LJS7F-7L7.\n....F-J..F7FJ|L7L7L7\n....L7.F7||L7|.L7L7|\n.....|FJLJ|FJ|F7|.LJ\n....FJL-7.||.||||...\n....L---J.LJ.LJLJ...", 8)]
    protected override int Part2(Model input)
    {
        var grid = input.Grid;
        var loop = new HashSet<Position>();

        var startNorth = false;
        var startSouth = false;
        var startEast = false;
        var startWest = false;
        
        {
            var start = grid.Keys().First(x => grid[x] == Cell.Start);
            var search = new Queue<Position>();
            search.Enqueue(start);

            while (search.Count > 0)
            {
                var pos = search.Dequeue();
                if (!loop.Add(pos))
                {
                    continue;
                }

                var outs = grid[pos] switch
                {
                    Cell.Start => (North: true, South: true, East: true, West: true),

                    Cell.NorthSouth => (North: true, South: true, East: false, West: false),
                    Cell.EastWest => (North: false, South: false, East: true, West: true),
                    Cell.NorthEast => (North: true, South: false, East: true, West: false),
                    Cell.NorthWest => (North: true, South: false, East: false, West: true),
                    Cell.SouthWest => (North: false, South: true, East: false, West: true),
                    Cell.SouthEast => (North: false, South: true, East: true, West: false),
                };

                if (outs.North)
                {
                    var north = pos + new Position(0, -1);
                    if (grid.IsValid(north) && grid[north] is Cell.NorthSouth or Cell.SouthEast or Cell.SouthWest)
                    {
                        if (pos == start) startNorth = true;
                        search.Enqueue(north);
                    }
                }

                if (outs.South)
                {
                    var south = pos + new Position(0, 1);
                    if (grid.IsValid(south) && grid[south] is Cell.NorthSouth or Cell.NorthEast or Cell.NorthWest)
                    {
                        if (pos == start) startSouth = true;
                        search.Enqueue(south);
                    }
                }

                if (outs.East)
                {
                    var east = pos + new Position(1, 0);
                    if (grid.IsValid(east) && grid[east] is Cell.EastWest or Cell.NorthWest or Cell.SouthWest)
                    {
                        if (pos == start) startEast = true;
                        search.Enqueue(east);
                    }
                }

                if (outs.West)
                {
                    var west = pos + new Position(-1, 0);
                    if (grid.IsValid(west) && grid[west] is Cell.EastWest or Cell.NorthEast or Cell.SouthEast)
                    {
                        if (pos == start) startWest = true;
                        search.Enqueue(west);
                    }
                }
            }
        }

        var cleanGrid = Grid<Cell>.Empty(grid.Width, grid.Height);
        foreach (var position in loop)
        {
            var cell = grid[position];

            if (cell == Cell.Start)
            {
                if (startNorth && startSouth) cell = Cell.NorthSouth;
                else if (startNorth && startEast) cell = Cell.NorthEast;
                else if (startNorth && startWest) cell = Cell.NorthWest;
                else if (startSouth && startEast) cell = Cell.SouthEast;
                else if (startSouth && startWest) cell = Cell.SouthWest;
                else if (startEast && startWest) cell = Cell.EastWest;
                else throw new NotImplementedException();
            }
            
            cleanGrid[position] = cell;
        }
        
        var expandedGrid = Grid<bool>.Empty(grid.Width*3, grid.Height*3);
        foreach (var position in loop)
        {
            var cell = cleanGrid[position];
            
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
        
        // total area - pipe area - accessible area
        var totalArea = grid.Keys().Count();
        var pipeArea = loop.Count;
        var accessibleArea = 0;
        
        foreach (var position in grid.Keys())
        {
            if(loop.Contains(position)) continue;
            
            // from position, can reach edge?
            if (CanExitGrid(position*3 + new Position(1,1), expandedGrid)) accessibleArea++;
        }

        return totalArea - pipeArea - accessibleArea;
    }

    private static bool CanExitGrid(Position position, Grid<bool> grid)
    {
        var seen = new HashSet<Position>();
        var search = new Queue<Position>();
        search.Enqueue(position);
        
        while (search.Count > 0)
        {
            var pos = search.Dequeue();
            if (grid[pos]) continue;
            
            if (!seen.Add(pos))
            {
                continue;
            }
            
            foreach (var neighbour in pos.OrthogonalNeighbours())
            {
                if (!grid.IsValid(neighbour)) return true;
                if(!grid[neighbour]) search.Enqueue(neighbour);
            }
        }

        return false;
    }

    public record Model(Grid<Cell> Grid);

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