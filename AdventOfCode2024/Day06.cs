namespace AdventOfCode2024;

[Day]
public partial class Day06 : Day<Day06.Model, int, int>
{
    public record Model(Grid<Cell> Grid);
    public enum Cell
    {
        Empty,
        Start,
        Obstacle
    }
    
    protected override Model Parse(string input) => new Model(GridParser.ParseChar(input, x => x switch
    {
        '.' => Cell.Empty,
        '^' => Cell.Start,
        '#' => Cell.Obstacle,
    }));
    
    [Sample("....#.....\n.........#\n..........\n..#.......\n.......#..\n..........\n.#..^.....\n........#.\n#.........\n......#...", 41)]
    protected override int Part1(Model input)
    {
        var visited = GetVisited(input);

        return visited.Count;
    }
    
    [Sample("....#.....\n.........#\n..........\n..#.......\n.......#..\n..........\n.#..^.....\n........#.\n#.........\n......#...", 6)]
    protected override int Part2(Model input)
    {
        var where = GetVisited(input).Where(x => IsLoop(input, x)).ToList();
        
        return GetVisited(input).Count(extra => IsLoop(input, extra));
    }

    private static HashSet<Position> GetVisited(Model input)
    {
        var visited = new HashSet<Position>();
        
        var position = input.Grid.Keys().First(x => input.Grid[x] == Cell.Start);
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

    private bool IsLoop(Model input, Position extraObstacle)
    {
        var visitedStates = new HashSet<(Position Position, Position Heading)>();
        
        var position = input.Grid.Keys().First(x => input.Grid[x] == Cell.Start);
        if (position == extraObstacle) return false; // invalid
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