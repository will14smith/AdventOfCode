namespace AdventOfCode2022;

[Day]
public partial class Day24 : Day<Day24.Model, int, int>
{
    private const string Sample = "#.######\n#>>.<^<#\n#.<..<<#\n#>v.><>#\n#<^v^^>#\n######.#";
    
    protected override Model Parse(string input) => new(GridParser.ParseChar(input, CharToCell));
    
    [Sample(Sample, 18)]
    protected override int Part1(Model input)
    {
        return Traverse(input.MapSize, input.ToList(), false).Time;
    }
    
    [Sample(Sample, 54)]
    protected override int Part2(Model input)
    {
        var a = Traverse(input.MapSize, input.ToList(), false);
        var b = Traverse(input.MapSize, a.Blizzards, true);
        var c = Traverse(input.MapSize, b.Blizzards, false);

        return a.Time + b.Time + c.Time;
    }
    
    private static (IReadOnlyList<Blizzard> Blizzards, int Time) Traverse(Position mapSize, IReadOnlyList<Blizzard> initialBlizzards, bool reverse)
    {
        var blizzardsOverTime = new List<IReadOnlyList<Blizzard>> { initialBlizzards };
        var occupiedOverTime = new List<IReadOnlySet<Position>> { initialBlizzards.Select(x => x.Position).ToHashSet() };

        var start = new Position(1, 0);
        var end = new Position(mapSize.X - 2, mapSize.Y - 1);
        if (reverse) (start, end) = (end, start);
        
        var search = new Queue<(Position, int)>();
        search.Enqueue((start, 0));

        var seen = new HashSet<(Position, int)>();

        while (search.Count > 0)
        {
            var (pos, time) = search.Dequeue();
            if (pos == end)
            {
                return (blizzardsOverTime[time], time);
            }

            foreach (var nextPos in NextPositions(pos, time))
            {
                if (nextPos == end)
                {
                    return (blizzardsOverTime[time + 1], time + 1);
                }

                if (seen.Add((nextPos, time + 1)))
                {
                    search.Enqueue((nextPos, time + 1));
                }
            }
        }

        IEnumerable<Position> NextPositions(Position position, int time)
        {
            var nextTime = time + 1;
            if (nextTime >= blizzardsOverTime.Count)
            {
                blizzardsOverTime.Add(NextBlizzards(mapSize, blizzardsOverTime[^1]));
                occupiedOverTime.Add(blizzardsOverTime[^1].Select(x => x.Position).ToHashSet());
            }

            var nextOccupied = occupiedOverTime[nextTime];

            var nextPositions = position.OrthogonalNeighbours().Append(position);
            return nextPositions.Where(p =>
            {
                if (p.X <= 0 || p.X >= mapSize.X - 1) return false;
                if (p.Y < 0 || p.Y > mapSize.Y - 1) return false;
                if ((p.Y == 0 && p.X != 1) || (p.Y == mapSize.Y - 1 && p.X != mapSize.X - 2)) return false;

                return !nextOccupied.Contains(p);
            });
        }

        throw new Exception("no solution");
    }

    public static IReadOnlyList<Blizzard> NextBlizzards(Position size, IReadOnlyList<Blizzard> blizzards)
    {
        var nextBlizzards = new List<Blizzard>(blizzards.Count);

        foreach (var blizzard in blizzards)
        {
            var nextPosition = blizzard.Position + blizzard.Velocity;
            if (nextPosition.X == 0) nextPosition = nextPosition with { X = size.X - 2 };
            if (nextPosition.Y == 0) nextPosition = nextPosition with { Y = size.Y - 2 };
            if (nextPosition.X == size.X - 1) nextPosition = nextPosition with { X = 1 };
            if (nextPosition.Y == size.Y - 1) nextPosition = nextPosition with { Y = 1 };
            
            nextBlizzards.Add(blizzard with { Position = nextPosition });
        }
        
        return nextBlizzards;
    }

    public record Blizzard(Position Position, Position Velocity);

    public record Model(Grid<Cell> Map)
    {
        public Position MapSize => new(Map.Width, Map.Height);
     
        public IReadOnlyList<Blizzard> ToList()
        {
            return Map.Keys().Select(p => Map[p] switch
            {
                Cell.Wall => null,
                Cell.Empty => null,
                Cell.Up => new Blizzard(p, new Position(0, -1)),
                Cell.Down => new Blizzard(p, new Position(0, 1)),
                Cell.Left => new Blizzard(p, new Position(-1, 0)),
                Cell.Right => new Blizzard(p, new Position(1, 0)),
            }).Where(x => x != null).ToList();
        }
    }
    public enum Cell
    {
        Wall,
        Empty,
        Up,
        Down,
        Left,
        Right
    }
    private static Cell CharToCell(char c)
    {
        return c switch
        {
            '#' => Cell.Wall,
            '.' => Cell.Empty,
            '^' => Cell.Up,
            'v' => Cell.Down,
            '<' => Cell.Left,
            '>' => Cell.Right,
        };
    }
}
