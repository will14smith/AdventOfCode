using System.Text;

namespace AdventOfCode2023;

[Day]
public partial class Day13 : Day<Day13.Model, int, int>
{
    protected override Model Parse(string input) => new (input.Split("\n\n").Select(pattern => GridParser.ParseBool(pattern, '#')).ToArray());

    [Sample("#.##..##.\n..#.##.#.\n##......#\n##......#\n..#.##.#.\n..##..##.\n#.#.##.#.\n\n#...##..#\n#....#..#\n..##..###\n#####.##.\n#####.##.\n..##..###\n#....#..#", 405)]
    protected override int Part1(Model input) => input.Patterns.Select(x => FindMirrors(x, 0)).Select(x => x.First()).Sum(x => x.Vertical ? x.Position : 100 * x.Position);

    [Sample("#.##..##.\n..#.##.#.\n##......#\n##......#\n..#.##.#.\n..##..##.\n#.#.##.#.\n\n#...##..#\n#....#..#\n..##..###\n#####.##.\n#####.##.\n..##..###\n#....#..#", 400)]
    protected override int Part2(Model input) => input.Patterns.Select(x => FindMirrors(x, 1)).Select(x => x.First()).Sum(x => x.Vertical ? x.Position : 100 * x.Position);

    private static IEnumerable<(bool Vertical, int Position)> FindMirrors(Grid<bool> pattern, int expectedDifferences)
    {
        var columns = ExtractColumns(pattern);
        for (var x = 1; x < pattern.Width; x++)
        {
            var size = Math.Min(x, pattern.Width - x);

            var columnsL = columns[(x - size)..x];
            var columnsR = columns[x..(x + size)];

            columnsL.Reverse();

            var l = string.Join("", columnsL);
            var r = string.Join("", columnsR);

            var differences = l.Zip(r).Sum(z => z.First == z.Second ? 0 : 1);
            if (differences == expectedDifferences) yield return (true, x);
        }
        
        var rows = ExtractRows(pattern);
        for (var y = 1; y < pattern.Height; y++)
        {
            var size = Math.Min(y, pattern.Height - y);

            var rowsU = rows[(y - size)..y];
            var rowsD = rows[y..(y + size)];

            rowsU.Reverse();
            
            var u = string.Join("", rowsU);
            var d = string.Join("", rowsD);

            var differences = u.Zip(d).Sum(z => z.First == z.Second ? 0 : 1);
            if (differences == expectedDifferences) yield return (false, y);
        }
    }

    private static List<string> ExtractColumns(Grid<bool> pattern)
    {
        var columns = new List<string>();
        for (var x = 0; x < pattern.Width; x++)
        {
            var str = new StringBuilder(pattern.Height);
            for (var y = 0; y < pattern.Height; y++)
            {
                str.Append(pattern[x, y] ? '#' : '.');
            }
            columns.Add(str.ToString());
        }

        return columns;
    }  
    private static List<string> ExtractRows(Grid<bool> pattern)
    {
        var rows = new List<string>();
        for (var y = 0; y < pattern.Height; y++)
        {
            var str = new StringBuilder(pattern.Width);
            for (var x = 0; x < pattern.Width; x++)
            {
                str.Append(pattern[x, y] ? '#' : '.');
            }
            rows.Add(str.ToString());
        }

        return rows;
    }
    
    public record Model(IReadOnlyList<Grid<bool>> Patterns);
}