using System.Collections.Immutable;

namespace AdventOfCode2024;

[Day]
public partial class Day16 : Day<Day16.Model, int, int>
{
    public record Model(Grid<Cell> Map);
    public enum Cell
    {
        Empty,
        Wall,
        Start,
        End,
    }
    
    protected override Model Parse(string input) => new(GridParser.ParseChar(input, x => x switch
    {
        '.' => Cell.Empty,
        '#' => Cell.Wall,
        'S' => Cell.Start,
        'E' => Cell.End,
    }));
    
    [Sample("###############\n#.......#....E#\n#.#.###.#.###.#\n#.....#.#...#.#\n#.###.#####.#.#\n#.#.#.......#.#\n#.#.#####.###.#\n#...........#.#\n###.#.#####.#.#\n#...#.....#.#.#\n#.#.#.###.#.#.#\n#.....#...#.#.#\n#.###.#.#.#.#.#\n#S..#.....#...#\n###############", 7036)]
    [Sample("#################\n#...#...#...#..E#\n#.#.#.#.#.#.#.#.#\n#.#.#.#...#...#.#\n#.#.#.#.###.#.#.#\n#...#.#.#.....#.#\n#.#.#.#.#.#####.#\n#.#...#.#.#.....#\n#.#.#####.#.###.#\n#.#.#.......#...#\n#.#.###.#####.###\n#.#.#...#.....#.#\n#.#.#.#####.###.#\n#.#.#.........#.#\n#.#.#.#########.#\n#S#.............#\n#################", 11048)]
    protected override int Part1(Model input)
    {
        var start = input.Map.Keys().First(x => input.Map[x] == Cell.Start);
        var end = input.Map.Keys().First(x => input.Map[x] == Cell.End);

        var initialState = (start, new Position(1, 0));
        
        return Score(input.Map, initialState, end);
    }

    private int Score(Grid<Cell> map, (Position Position, Position Heading) initialState, Position end)
    {
        var openSet = new PriorityQueue<(Position, Position), int>();
        openSet.Enqueue(initialState, H(initialState.Position));
        
        var cameFrom = new Dictionary<(Position, Position), (Position, Position)>();
        var gScore = new Dictionary<(Position, Position), int>
        {
            [initialState] = 0
        };

        while (openSet.Count > 0)
        {
            var currentState = openSet.Dequeue();
            if (currentState.Item1 == end)
            {
                return gScore[currentState];
            }
            
            foreach (var (next, cost) in NextState(map, currentState))
            {
                var newGScore = gScore[currentState] + cost;
                var previousGScore = gScore.GetValueOrDefault(next, int.MaxValue);

                if (newGScore < previousGScore)
                {
                    cameFrom[next] = currentState;
                    gScore[next] = newGScore;
                    openSet.Enqueue(next, newGScore + H(next.Item1));
                }

            }
        }
        
        throw new Exception("no solution");
        
        int H(Position p) => (end - p).TaxiDistance();
    }

    private IEnumerable<((Position, Position), int)> NextState(Grid<Cell> map, (Position Position, Position Heading) state)
    {
        var forward = (state.Position + state.Heading, state.Heading);
        if (map[forward.Item1] != Cell.Wall)
        {
            yield return (forward, 1);
        }

        yield return ((state.Position, state.Heading.RotateCW(90)), 1000);
        yield return ((state.Position, state.Heading.RotateCCW(90)), 1000);
    }
    
    [Sample("###############\n#.......#....E#\n#.#.###.#.###.#\n#.....#.#...#.#\n#.###.#####.#.#\n#.#.#.......#.#\n#.#.#####.###.#\n#...........#.#\n###.#.#####.#.#\n#...#.....#.#.#\n#.#.#.###.#.#.#\n#.....#...#.#.#\n#.###.#.#.#.#.#\n#S..#.....#...#\n###############", 45)]
    [Sample("#################\n#...#...#...#..E#\n#.#.#.#.#.#.#.#.#\n#.#.#.#...#...#.#\n#.#.#.#.###.#.#.#\n#...#.#.#.....#.#\n#.#.#.#.#.#####.#\n#.#...#.#.#.....#\n#.#.#####.#.###.#\n#.#.#.......#...#\n#.#.###.#####.###\n#.#.#...#.....#.#\n#.#.#.#####.###.#\n#.#.#.........#.#\n#.#.#.#########.#\n#S#.............#\n#################", 64)]
    protected override int Part2(Model input)
    {
        var best = Part1(input);
        
        var start = input.Map.Keys().First(x => input.Map[x] == Cell.Start);
        var end = input.Map.Keys().First(x => input.Map[x] == Cell.End);

        var initialState = (start, new Position(1, 0));
        var initialPath = ImmutableList<(Position, Position)>.Empty;
        var initialVisited = ImmutableHashSet<(Position, Position)>.Empty;

        var paths = FindPaths(initialState, initialPath, initialVisited, best);
        return paths.SelectMany(x => x).Select(x => x.Item1).ToHashSet().Count;
        
        IEnumerable<ImmutableList<(Position, Position)>> FindPaths((Position Position, Position Heading) state, ImmutableList<(Position, Position)> currentPath, ImmutableHashSet<(Position, Position)> currentVisited, int remainingScore)
        {
            if (currentVisited.Contains(state))
            {
                yield break;
            }
            if (remainingScore < 0)
            {
                yield break;
            }
            
            if (state.Item1 == end)
            {
                yield return currentPath;
                yield break;
            }

            // TODO: probably just pre-calculate & cache all the scores instead of starting from scratch
            if (Score(input.Map, state, end) > remainingScore)
            {
                yield break;
            }
            
            foreach (var (next, cost) in NextState(input.Map, state))
            {
                var paths = FindPaths(next, currentPath.Add(next), currentVisited.Add(state), remainingScore - cost);
                foreach (var path in paths)
                {
                    yield return path;
                }
            }
        }
    }
}