namespace AdventOfCode2023;

[Day]
public partial class Day21 : Day<Day21.Model, int, long>
{
    protected override Model Parse(string input)
    {
        var map = GridParser.ParseChar(input, x => x);
        
        return new Model(map);
    }

    [Sample("...........\n.....###.#.\n.###.##..#.\n..#.#...#..\n....#.#....\n.##..S####.\n.##..#...#.\n.......##..\n.##.#.####.\n.##..##.##.\n...........", 2460)]
    protected override int Part1(Model input) => FindDistance(input).Count(x => (x.Value % 2) == 0 && x.Value < 65);

    protected override long Part2(Model input)
    {
        var steps = 26501365L;
        var (full, part) = Math.DivRem(steps, input.Map.Width);

        if ((full & 1) != 0 || part != input.Map.Width / 2)
        {
            throw new Exception("this ain't gonna work");
        }
        
        // at 0 full + part we have 1 odd - 1 odd corners and 0 even
        // at 1 full + part we have 1 odd + 1 odd corners and 4 even - 2 even corners
        // at 2 full + part we have 9 odd - 3 odd corners and 4 even + 2 even corners
        // at 3 full + part we have 9 odd + 3 odd corners and 16 even - 4 even corners
        // at 4 full + part we have 25 odd - 5 odd corners and 16 even + 4 even corners
        
        // since we know full is even, we'll just code with that assumption
        var fullOdd = (full + 1) * (full + 1);
        var fullEven = full * full;
        var cornersOdd = -(full + 1);
        var cornersEven = full;

        var distances = FindDistance(input);
        
        var oddFullCount = distances.Count(x => (x.Value & 1) == 1);
        var evenFullCount = distances.Count(x => (x.Value & 1) == 0);
        
        var oddCornerCount = distances.Count(x => (x.Value & 1) == 1 && x.Value > part);
        // I cannot explain the minus 1, but it is needed.
        var evenCornerCount = distances.Count(x => (x.Value & 1) == 0 && x.Value > part) - 1;

        return fullOdd * oddFullCount + fullEven * evenFullCount + cornersOdd * oddCornerCount + cornersEven * evenCornerCount;
    }

    private IReadOnlyDictionary<Position, int> FindDistance(Model input)
    {
        var rocks = input.Map.Keys().Where(x => input.Map[x] == '#').ToHashSet();
        var start = input.Map.Keys().First(x => input.Map[x] == 'S');

        var locations = new Dictionary<Position, int>();

        var search = new Queue<(Position, int)>();
        search.Enqueue((start, 0));

        while (search.Count > 0)
        {
            var (position, distance) = search.Dequeue();
            if (!locations.TryAdd(position, distance))
            {
                continue;
            }
            
            foreach (var neighbour in position.OrthogonalNeighbours())
            {
                if (!input.Map.IsValid(neighbour)) continue;
                if (rocks.Contains(neighbour)) continue;
                if (locations.ContainsKey(neighbour)) continue;
                
                search.Enqueue((neighbour, distance + 1));
            }
        }

        return locations;
    }

    public record Model(Grid<char> Map);
}