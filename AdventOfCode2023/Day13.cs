using System.Text;

namespace AdventOfCode2023;

[Day]
public partial class Day13 : Day<Day13.Model, int, int>
{
    protected override Model Parse(string input) => new (input.Split("\n\n").Select(pattern => GridParser.ParseBool(pattern, '#')).ToArray());

    [Sample("#.##..##.\n..#.##.#.\n##......#\n##......#\n..#.##.#.\n..##..##.\n#.#.##.#.\n\n#...##..#\n#....#..#\n..##..###\n#####.##.\n#####.##.\n..##..###\n#....#..#", 405)]
    protected override int Part1(Model input) => input.Patterns.Select(FindMirrors).Select(x => x.Single()).Sum(x => x.Vertical ? x.Position : 100 * x.Position);

    [Sample("#.##..##.\n..#.##.#.\n##......#\n##......#\n..#.##.#.\n..##..##.\n#.#.##.#.\n\n#...##..#\n#....#..#\n..##..###\n#####.##.\n#####.##.\n..##..###\n#....#..#", 400)]
    protected override int Part2(Model input) => input.Patterns.Select(FindDifferentMirrorsWithSmudge).Select(x => x.First()).Sum(x => x.Vertical ? x.Position : 100 * x.Position);

    private static IEnumerable<(bool Vertical, int Position)> FindMirrors(Grid<bool> pattern)
    {
        var columns = ExtractColumns(pattern);
        for (var x = 1; x < pattern.Width; x++)
        {
            var size = Math.Min(x, pattern.Width - x);

            var columnsL = columns[(x - size)..x];
            var columnsR = columns[x..(x + size)];

            columnsL.Reverse();
            if (columnsL.SequenceEqual(columnsR)) yield return (true, x);
        }
        
        var rows = ExtractRows(pattern);
        for (var y = 1; y < pattern.Height; y++)
        {
            var size = Math.Min(y, pattern.Height - y);

            var rowsU = rows[(y - size)..y];
            var rowsD = rows[y..(y + size)];

            rowsU.Reverse();
            if (rowsU.SequenceEqual(rowsD)) yield return (false, y);
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

    private static IEnumerable<(bool Vertical, int Position)> FindDifferentMirrorsWithSmudge(Grid<bool> arg)
    {
        var original = FindMirrors(arg).Single();
        
        foreach (var position in arg.Keys())
        {
            arg[position] = !arg[position];
            var results = FindMirrors(arg).Where(x => x != original);
            foreach (var result in results)
            {
                yield return result;
            }
            arg[position] = !arg[position];
        }
    }

    public record Model(IReadOnlyList<Grid<bool>> Patterns);
}