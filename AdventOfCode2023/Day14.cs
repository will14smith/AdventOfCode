namespace AdventOfCode2023;

[Day]
public partial class Day14 : Day<Day14.Model, int, int>
{
    protected override Model Parse(string input) => new (GridParser.ParseChar(input, Parse));
    private static Rock Parse(char input) => input switch
    {
        '.' => Rock.Empty,
        'O' => Rock.Round,
        '#' => Rock.Cube,
    };
    private static string Print(Grid<Rock> map) => map.Print(x => x switch
    {
        Rock.Empty => '.',
        Rock.Round => 'O',
        Rock.Cube => '#',
        _ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
    });


    [Sample("O....#....\nO.OO#....#\n.....##...\nOO.#O....O\n.O.....O#.\nO.#..O.#.#\n..O..#O..O\n.......O..\n#....###..\n#OO..#....", 136)]
    protected override int Part1(Model input) => WeightNorth(TiltNorth(input.Map));
    [Sample("O....#....\nO.OO#....#\n.....##...\nOO.#O....O\n.O.....O#.\nO.#..O.#.#\n..O..#O..O\n.......O..\n#....###..\n#OO..#....", 64)]
    protected override int Part2(Model input)
    {
        var cycle = CycleDetection.Detect(() => input.Map, Apply, Equal);

        const int target = 1_000_000_000;
        var runsRequiredToMeetTargetCycle = cycle.Offset + (target - cycle.Offset) % cycle.Length;

        var targetMap = input.Map;
        for (var i = 0; i < runsRequiredToMeetTargetCycle; i++) { targetMap = Apply(targetMap); }

        return WeightNorth(targetMap);
        
        static Grid<Rock> Apply(Grid<Rock> map) => TiltEast(TiltSouth(TiltWest(TiltNorth(map))));
        static bool Equal(Grid<Rock> map1, Grid<Rock> map2) => Print(map1) == Print(map2);
    }

    // the ordering of Round is important, it MUST be sorted by increasing X & increasing Y
    // this is because the Tilt methods need to process the rocks in a specific order
    private static (Grid<Rock> Static, IReadOnlyList<Position> Round) Prepare(Grid<Rock> map)
    {
        var newMap = Grid.Empty<Rock>(map.Width, map.Height);
        var round = new List<Position>();

        for (var y = 0; y < map.Height; y++)
        for (var x = 0; x < map.Width; x++)
        {
            switch (map[x, y])
            {
                case Rock.Empty: break;
                case Rock.Round: round.Add(new Position(x, y)); break;
                case Rock.Cube: newMap[x, y] = Rock.Cube; break;
                    
                default: throw new ArgumentOutOfRangeException();
            }

        }
        
        return (newMap, round);
    }

    private static Grid<Rock> TiltNorth(Grid<Rock> map)
    {
        var (newMap, round) = Prepare(map);

        foreach (var position in round)
        {
            var highestY = position.Y;
            for (var y2 = position.Y; y2 >= 0; y2--)
            {
                if (newMap[position.X, y2] is Rock.Empty)
                {
                    highestY = y2;
                }
                else
                {
                    break;
                }
            }

            newMap[position.X, highestY] = Rock.Round; 
        }

        return newMap;
    }
    private static Grid<Rock> TiltWest(Grid<Rock> map)
    {
        var (newMap, round) = Prepare(map);

        foreach (var position in round)
        {
            var highestX = position.X;
            for (var x2 = position.X; x2 >= 0; x2--)
            {
                if (newMap[x2, position.Y] is Rock.Empty)
                {
                    highestX = x2;
                }
                else
                {
                    break;
                }
            }

            newMap[highestX, position.Y] = Rock.Round; 
        }

        return newMap;
    }
    
    private static Grid<Rock> TiltSouth(Grid<Rock> map)
    {
        var (newMap, round) = Prepare(map);

        foreach (var position in round.Reverse())
        {
            var highestY = position.Y;
            for (var y2 = position.Y; y2 < map.Height; y2++)
            {
                if (newMap[position.X, y2] is Rock.Empty)
                {
                    highestY = y2;
                }
                else
                {
                    break;
                }
            }
            
            newMap[position.X, highestY] = Rock.Round; 
        }

        return newMap;
    }
    
    private static Grid<Rock> TiltEast(Grid<Rock> map)
    {
        var (newMap, round) = Prepare(map);

        foreach (var position in round.Reverse())
        {
            var highestX = position.X;
            for (var x2 = position.X; x2 < map.Width; x2++)
            {
                if (newMap[x2, position.Y] is Rock.Empty)
                {
                    highestX = x2;
                }
                else
                {
                    break;
                }
            }
            
            newMap[highestX, position.Y] = Rock.Round; 
        }

        return newMap;
    }

    private static int WeightNorth(Grid<Rock> map) => map.Keys().Where(position => map[position] == Rock.Round).Sum(position => map.Height - position.Y);
    
    public record Model(Grid<Rock> Map);
    public enum Rock
    {
        Empty,
        Round,
        Cube,
    }
}