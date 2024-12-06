namespace AdventOfCode2024;

[Day]
public partial class Day06 : Day<Day06.Model, int, int>
{
    public record Model(Grid<Cell> Grid, Position Start);
    public enum Cell
    {
        Empty,
        Start,
        Obstacle
    }
    
    protected override Model Parse(string input)
    {
        var grid = GridParser.ParseChar(input, x => x switch
        {
            '.' => Cell.Empty,
            '^' => Cell.Start,
            '#' => Cell.Obstacle,
        });
        
        return new Model(grid, grid.Keys().First(x => grid[x] == Cell.Start));
    }

    [Sample("....#.....\n.........#\n..........\n..#.......\n.......#..\n..........\n.#..^.....\n........#.\n#.........\n......#...", 41)]
    protected override int Part1(Model input) => GetVisited(input).Count;

    [Sample("....#.....\n.........#\n..........\n..#.......\n.......#..\n..........\n.#..^.....\n........#.\n#.........\n......#...", 6)]
    protected override int Part2(Model input) => GetVisited(input).Where(x => x != input.Start).AsParallel().Count(extra => IsLoop(input, extra));

    private static HashSet<Position> GetVisited(Model input)
    {
        var visited = new HashSet<Position>();
        
        var position = input.Start;
        var heading = new Position(0, -1);
        
        while (input.Grid.IsValid(position))
        {
            visited.Add(position);

            var next = position + heading;
            while (input.Grid.IsValid(next) && input.Grid[next] == Cell.Obstacle)
            {
                heading = heading.RotateCCW(90);   
                next = position + heading;
            }
            
            position = next;
        }

        return visited;
    }

    private static bool IsLoop(Model input, Position extraObstacle)
    {
        var visitedStates = new HashSet<(Position Position, Position Heading)>();
        
        var position = input.Start;
        var heading = new Position(0, -1);
        
        while (input.Grid.IsValid(position))
        {
            if (!visitedStates.Add((position, heading)))
            {
                return true;
            }

            var next = position + heading;
            while (input.Grid.IsValid(next) && (input.Grid[next] == Cell.Obstacle || next == extraObstacle))
            {
                heading = heading.RotateCCW(90);   
                next = position + heading;
            }
            
            position = next;
        }

        return false;
    }
}