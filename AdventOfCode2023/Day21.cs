namespace AdventOfCode2023;

[Day]
public partial class Day21 : Day<Day21.Model, int, int>
{
    protected override Model Parse(string input)
    {
        var map = GridParser.ParseChar(input, x => x);
        
        return new Model(map);
    }

    [Sample("...........\n.....###.#.\n.###.##..#.\n..#.#...#..\n....#.#....\n.##..S####.\n.##..#...#.\n.......##..\n.##.#.####.\n.##..##.##.\n...........", 0)]
    protected override int Part1(Model input)
    {
        var rocks = input.Map.Keys().Where(x => input.Map[x] == '#').ToHashSet();
        var start = input.Map.Keys().First(x => input.Map[x] == 'S');
        
        var locations = new HashSet<Position> { start };

        for (var i = 0; i < 50; i++)
        {
            locations = locations.SelectMany(x => x.OrthogonalNeighbours()).ToHashSet();
            locations.ExceptWith(rocks);
        }

        return locations.Count;
    }

    [Sample("...........\n.....###.#.\n.###.##..#.\n..#.#...#..\n....#.#....\n.##..S####.\n.##..#...#.\n.......##..\n.##.#.####.\n.##..##.##.\n...........", 0)]
    protected override int Part2(Model input)
    {        
        var rocks = input.Map.Keys().Where(x => input.Map[x] == '#').ToHashSet();
        var start = input.Map.Keys().First(x => input.Map[x] == 'S');

        var cycleStart = FindSteadyState(input, rocks, start);
        Output.WriteLine(cycleStart.ToString());
        
        var cycleBottomMiddle = FindSteadyState(input, rocks, new Position((input.Map.Width - 1) / 2, input.Map.Height - 1));
        Output.WriteLine(cycleBottomMiddle.ToString());
        var cycleTopMiddle = FindSteadyState(input, rocks, new Position((input.Map.Width - 1) / 2, 0));
        Output.WriteLine(cycleTopMiddle.ToString());
        var cycleLeftMiddle = FindSteadyState(input, rocks, new Position(0, (input.Map.Height - 1) / 2));
        Output.WriteLine(cycleLeftMiddle.ToString());
        var cycleRightMiddle = FindSteadyState(input, rocks, new Position(input.Map.Width - 1, (input.Map.Height - 1) / 2));
        Output.WriteLine(cycleRightMiddle.ToString());

        var cycleTopLeft = FindSteadyState(input, rocks, new Position(0, 0));
        Output.WriteLine(cycleTopLeft.ToString());
        var cycleTopRight = FindSteadyState(input, rocks, new Position(input.Map.Width - 1,0 ));
        Output.WriteLine(cycleTopRight.ToString());
        var cycleBottomLeft = FindSteadyState(input, rocks, new Position(0, input.Map.Height - 1));
        Output.WriteLine(cycleBottomLeft.ToString());
        var cycleBottomRight = FindSteadyState(input, rocks, new Position(input.Map.Width - 1, input.Map.Height - 1));
        Output.WriteLine(cycleBottomRight.ToString());

        // after 66 cycle: start 4 new grids
        // after 132 cycles
        
        return 0;
    }

    private (int Cycle, int Even, int Odd) FindSteadyState(Model input, IReadOnlySet<Position> rocks, Position start)
    {
        var locations = new HashSet<Position> { start };

        var prevCounts = (-1, -1); 
        
        for (var i = 1; i < 500; i++)
        {
            locations = locations.SelectMany(x => x.OrthogonalNeighbours()).Where(x => input.Map.IsValid(x)).ToHashSet();
            locations.ExceptWith(rocks);

            if (locations.Count == prevCounts.Item1)
            {
                var (even, odd) = prevCounts;
                if (i % 2 == 1) (even, odd) = (odd, even);
                
                return (i - 2, even, odd);
            }
            
            prevCounts = (prevCounts.Item2, locations.Count);
        }

        throw new Exception("no");
    }
    
    public record Model(Grid<char> Map);
}