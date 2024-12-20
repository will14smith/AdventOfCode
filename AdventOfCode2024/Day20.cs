using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace AdventOfCode2024;

[Day]
public partial class Day20 : Day<Day20.Model, int, int>
{
    public record Model(Grid<Cell> Grid);

    public enum Cell
    {
        Empty,
        Wall,
        Start,
        End,
    }
    
    protected override Model Parse(string input) =>
        new(GridParser.ParseChar(input, x => x switch
        {
            '.' => Cell.Empty,
            '#' => Cell.Wall,
            'S' => Cell.Start,
            'E' => Cell.End,
        }));

    [Sample("###############\n#...#...#.....#\n#.#.#.#.#.###.#\n#S#...#.#.#...#\n#######.#.#.###\n#######.#.#...#\n#######.#.###.#\n###..E#...#...#\n###.#######.###\n#...###...#...#\n#.#####.#.###.#\n#.#...#.#.#...#\n#.#.#.#.#.#.###\n#...#...#...###\n###############", 0)]
    protected override int Part1(Model input) => Solve(input, 2);

    [Sample("###############\n#...#...#.....#\n#.#.#.#.#.###.#\n#S#...#.#.#...#\n#######.#.#.###\n#######.#.#...#\n#######.#.###.#\n###..E#...#...#\n###.#######.###\n#...###...#...#\n#.#####.#.###.#\n#.#...#.#.#...#\n#.#.#.#.#.#.###\n#...#...#...###\n###############", 0)]
    protected override int Part2(Model input) => Solve(input, 20);

    private static int Solve(Model input, int cheatDistance)
    {
        var start = input.Grid.Keys().First(x => input.Grid[x] == Cell.Start);
        var end = input.Grid.Keys().First(x => input.Grid[x] == Cell.End);

        var path = new List<Position>([start]);
        var visited = new HashSet<Position>([start]);
        
        var current = start;
        while (current != end)
        {
            current = current.OrthogonalNeighbours().First(x => input.Grid[x] != Cell.Wall && visited.Add(x));
            path.Add(current);
        }

        var indexedPath = path.Index().ToDictionary(x => x.Item, x => x.Index);

        var saving = new ConcurrentBag<int>();
        Parallel.ForEach(path, position =>
        {
            var positionIndex = indexedPath[position];
            
            foreach (var (nextPosition, distance) in FindCheats(input.Grid, position, cheatDistance))
            {
                if (!indexedPath.TryGetValue(nextPosition, out var nextPositionIndex))
                {
                    continue;
                }

                if (nextPositionIndex > positionIndex + distance)
                {
                    saving.Add(nextPositionIndex - positionIndex - distance);
                }
            }
        });
        
        return saving.Count(x => x >= 100);
    }
    
    private static IEnumerable<(Position, int)> FindCheats(Grid<Cell> grid, Position position, int distance)
    {
        for (var y = -distance; y <= distance; y++)
        for (var x = -distance; x <= distance; x++)
        {
            var nextDistance = Math.Abs(x) + Math.Abs(y);
            if (nextDistance > distance)
            {
                continue;
            }
            
            var nextPosition = position + new Position(x, y);
            if (grid.IsValid(nextPosition) && grid[nextPosition] != Cell.Wall)
            {
                yield return (nextPosition, nextDistance);
            }
        }
    }
}