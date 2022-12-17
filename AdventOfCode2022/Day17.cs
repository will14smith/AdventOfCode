namespace AdventOfCode2022;

[Day]
public partial class Day17 : Day<Day17.Model, int, long>
{
    private const string Sample = @">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";

    private static readonly IReadOnlyList<RockShape> RockShapes = new[] { "####", ".#.\n###\n.#.", "..#\n..#\n###", "#\n#\n#\n#", "##\n##" }
        .Select(x => new RockShape(x.Split('\n').Reverse().Select(line => line.Select(c => c == '#').ToArray()).ToArray())).ToArray();

    protected override Model Parse(string input) => new(input.Select(x => x == '<' ? Direction.Left : Direction.Right).ToArray());

    [Sample(Sample, 3068)]
    protected override int Part1(Model input)
    {
        const int rockCount = 2022;
        
        var map = Map.Initial;
        var windIndex = 0;

        for (var rockIndex = 0; rockIndex < rockCount; rockIndex++)
        {
            var rockShape = RockShapes[rockIndex % RockShapes.Count];
            
            windIndex = DropRock(input, windIndex, rockShape, map);
        }
        
        return map.HighestRock;
    }
    
    [Sample(Sample, 1514285714288L)]
    protected override long Part2(Model input)
    {
        var (cycleOffset, cycleLength) = CycleDetection.Detect(() => State.New, Apply, EqualEnough);

        var stateAfterOffset = State.New;
        for (var i = 0; i < cycleOffset; i++)
        {
            stateAfterOffset = Apply(stateAfterOffset);
        }
        
        var stateAfterFirstCycle = State.New;
        for (var i = 0; i < cycleOffset+cycleLength; i++)
        {
            stateAfterFirstCycle = Apply(stateAfterFirstCycle);
        }

        var height = 0L;
        var remaining = 1000000000000L;

        height += stateAfterOffset.Map.HighestRock;
        remaining -= cycleOffset;

        var bulkDrop = remaining / cycleLength;
        height += bulkDrop * (stateAfterFirstCycle.Map.HighestRock - stateAfterOffset.Map.HighestRock);
        remaining %= cycleLength;
        
        var stateAfterFirstCycleAndRemainder = State.New;
        for (var i = 0; i < cycleOffset+cycleLength+remaining; i++)
        {
            stateAfterFirstCycleAndRemainder = Apply(stateAfterFirstCycleAndRemainder);
        }

        height += stateAfterFirstCycleAndRemainder.Map.HighestRock - stateAfterFirstCycle.Map.HighestRock;

        return height;

        State Apply(State state)
        {
            var (map, wind, rock) = state;
            
            var rockShape = RockShapes[rock];
            rock = (rock + 1) % RockShapes.Count;

            wind = DropRock(input, wind, rockShape, map);

            return new State(map, wind, rock);
        }
        
        bool EqualEnough(State a, State b) => GetStateSummary(a) == GetStateSummary(b);

        (int WindIndex, int RockIndex, string TopNRows) GetStateSummary(State state)
        {
            const int N = 64;
            var (map, wind, rock) = state;
            
            var topNRows = string.Join(";", map.Rocks.Where(x => x.Y >= map.HighestRock - N).Select(p => $"{p.X},{(map.HighestRock - p.Y) % (N +1)}"));

            return (wind, rock, topNRows);
        }
    }
    
    private static Position[] MaterialiseRock(Map map, RockShape rockShape)
    {
        var baseY = map.HighestRock + 1 + 3;

        var rock = new List<Position>();

        for (var y = 0; y < rockShape.Cells.Length; y++)
        {
            var line = rockShape.Cells[y];
            for (var x = 0; x < line.Length; x++)
            {
                var cell = line[x];

                if (cell)
                {
                    rock.Add(new Position(x + 2, y + baseY));
                }
            }
        }
        
        return rock.ToArray();
    }
    
    private static int DropRock(Model input, int windIndex, RockShape rockShape, Map map)
    {
        var rock = MaterialiseRock(map, rockShape);

        while (true)
        {
            var direction = input.Wind[windIndex];
            windIndex = (windIndex + 1) % input.Wind.Count;
            
            var pushedRock = ApplyForce(rock, new Position(direction == Direction.Left ? -1 : 1, 0));
            if (!map.Intersects(pushedRock))
            {
                rock = pushedRock;
            }

            var droppedRock = ApplyForce(rock, new Position(0, -1));

            if (!map.Intersects(droppedRock))
            {
                rock = droppedRock;
                continue;
            }

            map.Add(rock);
            break;
        }

        return windIndex;
    }
    
    private static Position[] ApplyForce(Position[] rock, Position force)
    {
        var newRock = new Position[rock.Length];

        for (var i = 0; i < rock.Length; i++)
        {
            newRock[i] = rock[i] + force;
        }
        
        return newRock;
    }
    
    public record Model(IReadOnlyList<Direction> Wind);
    public enum Direction
    {
        Left,
        Right
    }

    public record RockShape(bool[][] Cells);

    public record Map(HashSet<Position> RockSet, List<Position> Rocks)
    {
        public static Map Initial => new(new HashSet<Position>(), new List<Position>());

        public int HighestRock { get; private set; }

        public bool Intersects(Position[] rock)
        {
            return rock.Any(p => p.Y <= 0 || p.X is < 0 or >= 7 || RockSet.Contains(p));
        }

        public void Add(Position[] rock)
        {
            RockSet.UnionWith(rock);
            Rocks.AddRange(rock);
            HighestRock = Math.Max(HighestRock, rock.Max(x => x.Y));
        }
    }
    
    public record State(Map Map, int Wind, int Rock)
    {
        public static State New => new(Map.Initial, 0, 0);
    }
}