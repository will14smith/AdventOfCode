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

    [Sample("O....#....\nO.OO#....#\n.....##...\nOO.#O....O\n.O.....O#.\nO.#..O.#.#\n..O..#O..O\n.......O..\n#....###..\n#OO..#....", 136)]
    protected override int Part1(Model input) => WeightNorth(TiltNorth(input.Map));

    private (Grid<Rock> Static, IReadOnlyList<Position> Round) Prepare(Grid<Rock> map)
    {
        var newMap = Grid.Empty<Rock>(map.Width, map.Height);
        var round = new List<Position>();
        
        foreach (var position in map.Keys())
        {
            switch (map[position])
            {
                case Rock.Empty: break;
                    
                case Rock.Round:
                    round.Add(position);
                    break;
                case Rock.Cube: newMap[position] = Rock.Cube; break;
                    
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        return (newMap, round);
    }

    private Grid<Rock> TiltNorth(Grid<Rock> map)
    {
        var (newMap, round) = Prepare(map);

        foreach (var position in round.OrderBy(x => x.Y))
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
    private Grid<Rock> TiltWest(Grid<Rock> map)
    {
        var (newMap, round) = Prepare(map);

        foreach (var position in round.OrderBy(x => x.X))
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
    
    private Grid<Rock> TiltSouth(Grid<Rock> map)
    {
        var (newMap, round) = Prepare(map);

        foreach (var position in round.OrderByDescending(x => x.Y))
        {
            var highestY = position.Y;
            for (int y2 = position.Y; y2 < map.Height; y2++)
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
    
    private Grid<Rock> TiltEast(Grid<Rock> map)
    {
        var (newMap, round) = Prepare(map);

        foreach (var position in round.OrderByDescending(x => x.X))
        {
            var highestX = position.X;
            for (int x2 = position.X; x2 < map.Width; x2++)
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

    private int WeightNorth(Grid<Rock> map)
    {
        var weight = 0;
        foreach (var position in map.Keys())
        {
            if (map[position] == Rock.Round)
            {
                weight += map.Height - position.Y;
            }
        }
        return weight;
    }

    [Sample("O....#....\nO.OO#....#\n.....##...\nOO.#O....O\n.O.....O#.\nO.#..O.#.#\n..O..#O..O\n.......O..\n#....###..\n#OO..#....", 64)]
    protected override int Part2(Model input)
    {
        var x = CycleDetection.Detect(() => input.Map, Apply, Equal);

        var target = 1_000_000_000;
        var runs = x.Offset + (target - x.Offset) % x.Length;

        var y = input.Map;
        for (var i = 0; i < runs; i++)
        {
            y = Apply(y);
            Output.WriteLine($"{i}: {WeightNorth(y)}");
        }

        return WeightNorth(y);
    }

    private Grid<Rock> Apply(Grid<Rock> arg) => TiltEast(TiltSouth(TiltWest(TiltNorth(arg))));

    private bool Equal(Grid<Rock> arg1, Grid<Rock> arg2)
    {
        return Print(arg1) == Print(arg2);
    }
    private bool Equal((int Direction, Grid<Rock> Map) arg1, (int Direction, Grid<Rock> Map) arg2)
    {
        return arg1.Direction == arg2.Direction && Print(arg1.Map) == Print(arg2.Map);
    }

    private string Print(Grid<Rock> map)
    {
        return map.Print(x => x switch
        {
            Rock.Empty => '.',
            Rock.Round => 'O',
            Rock.Cube => '#',
            _ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
        });
    }

    public record Model(Grid<Rock> Map);
    public enum Rock
    {
        Empty,
        Round,
        Cube,
    }
}