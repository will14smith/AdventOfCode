namespace AdventOfCode2022;

[Day]
public partial class Day08 : Day<Day08.Model, int, int>
{
    private const string Sample = "30373\n25512\n65332\n33549\n35390";
    
    protected override Model Parse(string input) => new(GridParser.ParseInt(input));

    [Sample(Sample, 21)]
    protected override int Part1(Model input) => CountVisibleOutside(input.Trees);
    [Sample(Sample, 8)]
    protected override int Part2(Model input) => CountVisibleFrom(input.Trees);

    private int CountVisibleFrom(Grid<int> trees) => trees.Keys().Max(x => CountVisibleFrom(trees, x));

    private int CountVisibleFrom(Grid<int> trees, Position centre)
    {
        var score = 1;
        var deltas = new[] { new Position(0, 1), new Position(0, -1), new Position(1, 0), new Position(-1, 0) };
        
        var centreTree = trees[centre];

        foreach (var delta in deltas)
        {
            var count = 0;
            
            var position = centre + delta;
            while (trees.IsValid(position))
            {
                var tree = trees[position];
                
                position += delta;
                count++;
                
                if (tree >= centreTree)
                {
                    break;
                }
            }

            score *= count;
        }

        return score;
    }

    private int CountVisibleOutside(Grid<int> trees)
    {
        var startPoints = new List<(Position Position, Position Delta)>();
        for (int x = 0; x < trees.Width; x++)
        {
            startPoints.Add((new Position(x, 0), new Position(0, 1)));
            startPoints.Add((new Position(x, trees.Height - 1), new Position(0, -1)));
        }
        for (int y = 0; y < trees.Height; y++)
        {
            startPoints.Add((new Position(0, y), new Position(1, 0)));
            startPoints.Add((new Position(trees.Width - 1, y), new Position(-1, 0)));
        }

        return startPoints.SelectMany(x => FindVisibleOutside(trees, x.Position, x.Delta)).Distinct().Count();
    }

    private IReadOnlySet<Position> FindVisibleOutside(Grid<int> trees, Position start, Position delta)
    {
        var position = start;

        var previous = -1;
        var visible = new HashSet<Position>();
        
        while (trees.IsValid(position))
        {
            var tree = trees[position];
            if (tree > previous)
            {
                previous = tree;
                visible.Add(position);
            }
            
            position += delta;
        }

        return visible;
    }

    public record Model(Grid<int> Trees);
}
