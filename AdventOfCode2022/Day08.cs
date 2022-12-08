namespace AdventOfCode2022;

[Day]
public partial class Day08 : Day<Day08.Model, int, int>
{
    private const string Sample = "30373\n25512\n65332\n33549\n35390";
    
    protected override Model Parse(string input) => new(GridParser.ParseInt(input));

    [Sample(Sample, 21)]
    protected override int Part1(Model input) => CountVisibleFromOutside(input.Trees);
    [Sample(Sample, 8)]
    protected override int Part2(Model input) => FindMaxScenic(input.Trees);
    
    private static int CountVisibleFromOutside(Grid<int> trees)
    {
        var visible = new HashSet<Position>();

        for (var x = 0; x < trees.Width; x++)
        {
            visible.UnionWith(FindVisibleInSpan(trees, GetSpanWithX(x, 0, trees.Height - 1)));
            visible.UnionWith(FindVisibleInSpan(trees, GetSpanWithX(x, 0, trees.Height - 1).Reverse()));
        }
        
        for (var y = 0; y < trees.Height; y++)
        {
            visible.UnionWith(FindVisibleInSpan(trees, GetSpanWithY(y, 0, trees.Width - 1)));
            visible.UnionWith(FindVisibleInSpan(trees, GetSpanWithY(y, 0, trees.Width - 1).Reverse()));
        }

        return visible.Count;
    }

    private static IEnumerable<Position> FindVisibleInSpan(Grid<int> trees, IEnumerable<Position> span)
    {
        var height = -1;
        foreach (var position in span)
        {
            var tree = trees[position];
            if (tree <= height)
            {
                continue;
            }
            
            yield return position;
            height = tree;
        }
    }

    private static int FindMaxScenic(Grid<int> trees) => trees.Keys().Max(x => ScoreScenic(trees, x));

    private static int ScoreScenic(Grid<int> trees, Position centre)
    {
        var up = GetSpanWithX(centre.X, 0, centre.Y).Reverse();
        var down = GetSpanWithX(centre.X, centre.Y, trees.Height - 1);
        var left = GetSpanWithY(centre.Y, 0, centre.X).Reverse();
        var right = GetSpanWithY(centre.Y, centre.X, trees.Width - 1);

        var sightLines = new[] { up, down, left, right };
        return sightLines.Select(x => CountSightLineInSpan(trees, x)).Aggregate(1, (a, b) => a * b);
    }

    private static int CountSightLineInSpan(Grid<int> trees, IEnumerable<Position> span)
    {
        int? height = null;
        var count = 0;

        foreach (var position in span)
        {
            if (height == null)
            {
                height = trees[position];
                continue;
            }
            
            count++;

            if (height <= trees[position])
            {
                break;
            }
        }

        return count;
    }
    
    private static IEnumerable<Position> GetSpanWithX(int x, int startY, int endY) => Enumerable.Range(startY, endY - startY + 1).Select(y => new Position(x, y));
    private static IEnumerable<Position> GetSpanWithY(int y, int startX, int endX) => Enumerable.Range(startX, endX - startX + 1).Select(x => new Position(x, y));

    public record Model(Grid<int> Trees);
}
