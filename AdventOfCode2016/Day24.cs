namespace AdventOfCode2016;

[Day]
public partial class Day24 : Day<Day24.Model, int, int>
{
    private const int Wall = -1;
    private const int Open = -2;

    private const string Sample = "###########\n#0.1.....2#\n#.#######.#\n#4.......3#\n###########";
    
    protected override Model Parse(string input) => new(GridParser.ParseChar(input, c => c switch
    {
        '#' => Wall,
        '.' => Open,
        >= '0' and <= '9' => c - '0'
    }));

    [Sample(Sample, 14)]
    protected override int Part1(Model input) => Solve(input, false);
    
    protected override int Part2(Model input) => Solve(input, true);

    private static int Solve(Model input, bool returnToStart)
    {
        var robots = input.Map.Keys().Where(x => input.Map[x] >= 0).ToDictionary(x => input.Map[x], x => x);
        var distances = robots.SelectMany(a => robots.Select(b => FindDistance(input, a, b))).ToDictionary(x => (x.A, x.B), x => x.Distance);

        var min = int.MaxValue;
        foreach (var permutation in Permutations.Get(robots.Keys.ToList()))
        {
            var dist = 0;

            var permutationList = permutation.ToList();
            if (permutationList[0] != 0) continue;
            
            for (var i = 1; i < permutationList.Count; i++)
            {
                dist += distances[(permutationList[i - 1], permutationList[i])];
            }

            if (returnToStart)
            {
                dist += distances[(permutationList[^1], 0)];
            }

            min = Math.Min(min, dist);
        }

        return min;
    }
    
    private static (int A, int B, int Distance) FindDistance(Model input, KeyValuePair<int, Position> a, KeyValuePair<int, Position> b) =>
        (a.Key, b.Key, OptimisedSearch.Solve((Position: a.Value, Distance: 0), x => x.Position == b.Value, x =>
        {
            return x.Position.OrthogonalNeighbours().Where(n => input.Map.IsValid(n) && input.Map[n] != Wall).Select(n => (n, x.Distance + 1));
        }, _ => false, x => x.Position, x => x.Distance).Distance);

    
    public record Model(Grid<int> Map);
}
