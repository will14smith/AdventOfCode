namespace AdventOfCode2025;

[Day]
public partial class Day07 : Day<Day07.Model, int, long>
{
    public enum Cell
    {
        Empty,
        Start,
        Splitter,
    }
    public record Model(Grid<Cell> Map);

    protected override Model Parse(string input) => new(GridParser.ParseChar(input, c => c switch
    {
        '.' => Cell.Empty,
        'S' => Cell.Start,
        '^' => Cell.Splitter,
        
        _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
    }));

    [Sample(".......S.......\n...............\n.......^.......\n...............\n......^.^......\n...............\n.....^.^.^.....\n...............\n....^.^...^....\n...............\n...^.^...^.^...\n...............\n..^...^.....^..\n...............\n.^.^.^.^.^...^.\n...............\n", 21)]
    protected override int Part1(Model input)
    {
        var map = input.Map;
        var start = map.First(x => x == Cell.Start);

        var splits = 0;
        
        var currentBeams = new HashSet<Position> { start };

        while (true)
        {
            var nextBeams = currentBeams.Select(x => x + new Position(0, 1)).Where(x => map.IsValid(x)).ToHashSet();
            if (nextBeams.Count == 0)
            {
                break;
            }
            
            var beamsHittingSplitter = nextBeams.Where(x => map[x] == Cell.Splitter).ToList();
            foreach (var beam in beamsHittingSplitter)
            {
                nextBeams.Remove(beam);
                nextBeams.Add(beam + new Position(-1, 0));
                nextBeams.Add(beam + new Position(1, 0));
                
                splits++;
            }
            
            currentBeams = nextBeams;
        }

        return splits;
    }

    [Sample(".......S.......\n...............\n.......^.......\n...............\n......^.^......\n...............\n.....^.^.^.....\n...............\n....^.^...^....\n...............\n...^.^...^.^...\n...............\n..^...^.....^..\n...............\n.^.^.^.^.^...^.\n...............\n", 40)]
    protected override long Part2(Model input)
    {
        var map = input.Map;
        var start = map.First(x => x == Cell.Start);
        
        return CountTimelines(map, start, new Dictionary<Position, long>());
    }

    private static long CountTimelines(Grid<Cell> map, Position position, Dictionary<Position, long> cache)
    {
        if (cache.TryGetValue(position, out var cached))
        {
            return cached;
        }

        if (!map.IsValid(position))
        {
            return 1;
        }

        var count = map[position] switch
        {
            Cell.Empty or Cell.Start => 
                CountTimelines(map, position + new Position(0, 1), cache),
            Cell.Splitter => 
                CountTimelines(map, position + new Position(-1, 0), cache) +
                CountTimelines(map, position + new Position(1, 0), cache),
            
            _ => throw new ArgumentOutOfRangeException()
        };

        return cache[position] = count;
    }
}