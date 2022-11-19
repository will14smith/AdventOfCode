using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2020;

[Day]
public partial class Day03 : ParseLineDay<Day03.Line, int, int>
{
    private const string Sample = "..##.......\n#...#...#..\n.#....#..#.\n..#.#...#.#\n.#...##..#.\n..#.##.....\n.#.#.#....#\n.#........#\n#.##...#...\n#...##....#\n.#..#...#.#";

    private static readonly IReadOnlyCollection<(int X, int Y)> Slopes = new[] {(1, 1), (3, 1), (5, 1), (7, 1), (1, 2)};
    
    private static readonly TextParser<bool> TreeParser = Character.In('.', '#').Select(c => c == '#');
    protected override TextParser<Line> LineParser => TreeParser.Many().Select(x => new Line(x));

    [Sample(Sample, 7)]
    protected override int Part1(IEnumerable<Line> enumerable) => CountTreesHit(enumerable.ToList());
    
    [Sample(Sample, 336)]
    protected override int Part2(IEnumerable<Line> enumerable) => CountTreesHitAllSlopes(enumerable.ToList(), Slopes);

    private static int CountTreesHitAllSlopes(IReadOnlyList<Line> lines, IEnumerable<(int X, int Y)> slopes)
    {
        var treesHit = slopes.Select(slope => CountTreesHit(lines, slope.X, slope.Y)).ToList();
        return treesHit.Aggregate(1, (a, x) => a * x);
    }
    
    private static int CountTreesHit(IReadOnlyList<Line> lines, int dx = 3, int dy = 1)
    {
        var treesHit = 0;

        var x = 0;

        for (var y = 0; y < lines.Count; y += dy)
        {
            if (lines[y].HasTreeAtX(x))
            {
                treesHit++;
            }

            x += dx;
        }

        return treesHit;
    }
    
    public record Line(IReadOnlyList<bool> Trees)
    {
        public bool HasTreeAtX(int x) => Trees[x % Trees.Count];
    }
}
