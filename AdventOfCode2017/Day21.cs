namespace AdventOfCode2017;

[Day]
public partial class Day21 : Day<Day21.Model, int, int>
{
    protected override Model Parse(string input)
    {
        var rules = input
            .Split('\n')
            .Select(line => line.Split(" => "))
            .Select(parts => new Rule(ParseGrid(parts[0]), ParseGrid(parts[1])))
            .ToList();

        return new Model(rules);
    }
    private static Grid<bool> ParseGrid(string input)
    {
        var parts = input.Split('/');
        var grid = parts.SelectMany(x => x).Select(x => x == '#').ToArray();
        return new Grid<bool>(grid, parts.Length);
    }

    protected override int Part1(Model input) => Solve(input, 5);

    protected override int Part2(Model input) => Solve(input, 18);

    private int Solve(Model input, int iterations)
    {
        var grid = new Grid<bool>(new[]
        {
            false, true, false,
            false, false, true,
            true, true, true
        }, 3);

        // group rules by number of active cells so we can reduce the number of checks needed
        var rule2 = input.Rules.Where(x => x.Match.Width == 2).GroupBy(x => x.Match.Count(y => y)).ToDictionary(x => x.Key, x => x.ToList());
        var rule3 = input.Rules.Where(x => x.Match.Width == 3).GroupBy(x => x.Match.Count(y => y)).ToDictionary(x => x.Key, x => x.ToList());

        for (var i = 0; i < iterations; i++)
        {
            if (grid.Width % 2 == 0)
            {
                grid = Enhance(grid, rule2, 2);
            }
            else if (grid.Width % 3 == 0)
            {
                grid = Enhance(grid, rule3, 3);
            }
            else
            {
                throw new Exception("no");
            }
        }

        return grid.Count(x => x);
    }

    private Grid<bool> Enhance(Grid<bool> grid, IReadOnlyDictionary<int, List<Rule>> rules, int size)
    {
        var newGrid = Grid.Empty<bool>(grid.Width / size * (size + 1), grid.Height / size * (size + 1));
        
        for (var y = 0; y < grid.Height; y += size)
        {
            for (var x = 0; x < grid.Width; x += size)
            {
                // create a sub-grid for the block being enhanced
                var block = Grid.Empty<bool>(size, size);
                for (var y2 = 0; y2 < size; y2++)
                {
                    for (var x2 = 0; x2 < size; x2++)
                    {
                        block[x2, y2] = grid[x + x2, y + y2];
                    }
                }

                var rule = FindRule(rules[block.Count(z => z)], block);

                // apply the rule
                var newX = x / size * (size + 1);
                var newY = y / size * (size + 1);
                for (var y2 = 0; y2 < size+1; y2++)
                {
                    for (var x2 = 0; x2 < size+1; x2++)
                    {
                        newGrid[newX + x2, newY + y2] = rule.Replacement[x2, y2];
                    }
                }

            } 
        }

        return newGrid;
    }

    private Rule FindRule(IEnumerable<Rule> rules, Grid<bool> block) => rules.First(rule => IsMatch(rule.Match, block));
    private bool IsMatch(Grid<bool> match, Grid<bool> block)
    {
        return Original(match, block)
            || FlipX(match, block)
            || FlipY(match, block)
            || Rotate90(match, block)
            || Rotate180(match, block)
            || Rotate270(match, block)
            || Rotate270FlipX(match, block)
            || Rotate270FlipY(match, block); 

        static bool Original(Grid<bool> match, Grid<bool> block) { for (var y = 0; y < match.Height; y++) { for (var x = 0; x < match.Width; x++) { if (match[x, y] != block[x, y]) { return false; } } } return true; }
        static bool FlipX(Grid<bool> match, Grid<bool> block) { for (var y = 0; y < match.Height; y++) { for (var x = 0; x < match.Width; x++) { if (match[x, y] != block[block.Width - 1 - x, y]) { return false; } } } return true; }
        static bool FlipY(Grid<bool> match, Grid<bool> block) { for (var y = 0; y < match.Height; y++) { for (var x = 0; x < match.Width; x++) { if (match[x, y] != block[x, block.Height - 1 - y]) { return false; } } } return true; }
        static bool Rotate90(Grid<bool> match, Grid<bool> block) { for (var y = 0; y < match.Height; y++) { for (var x = 0; x < match.Width; x++) { if (match[x, y] != block[block.Height - 1 - y, x]) { return false; } } } return true; }
        static bool Rotate180(Grid<bool> match, Grid<bool> block) { for (var y = 0; y < match.Height; y++) { for (var x = 0; x < match.Width; x++) { if (match[x, y] != block[block.Width - 1 - x, block.Height - 1 - y]) { return false; } } } return true; }
        static bool Rotate270(Grid<bool> match, Grid<bool> block) { for (var y = 0; y < match.Height; y++) { for (var x = 0; x < match.Width; x++) { if (match[x, y] != block[y, block.Width - 1 - x]) { return false; } } } return true; }
        static bool Rotate270FlipX(Grid<bool> match, Grid<bool> block) { for (var y = 0; y < match.Height; y++) { for (var x = 0; x < match.Width; x++) { if (match[x, y] != block[y, x]) { return false; } } } return true; }
        static bool Rotate270FlipY(Grid<bool> match, Grid<bool> block) { for (var y = 0; y < match.Height; y++) { for (var x = 0; x < match.Width; x++) { if (match[x, y] != block[block.Height - 1 - y, block.Width - 1 - x]) { return false; } } } return true; }
    }

    public record Model(IReadOnlyList<Rule> Rules);

    public record Rule(Grid<bool> Match, Grid<bool> Replacement)
    {
        public override string ToString() => Match.Print(x => x ? '#' : '.');
    }
}
