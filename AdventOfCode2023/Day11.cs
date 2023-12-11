namespace AdventOfCode2023;

[Day]
public partial class Day11 : Day<Day11.Model, long, long>
{
    protected override Model Parse(string input) => new (GridParser.ParseBool(input, '#'));

    [Sample("...#......\n.......#..\n#.........\n..........\n......#...\n.#........\n.........#\n..........\n.......#..\n#...#.....", 374L)]
    protected override long Part1(Model input) => Solve(input.Map, 2);

    [Sample("...#......\n.......#..\n#.........\n..........\n......#...\n.#........\n.........#\n..........\n.......#..\n#...#.....", 82000210L)]
    protected override long Part2(Model input) => Solve(input.Map, 1_000_000);

    private static long Solve(Grid<bool> map, int factor)
    {
        var empty = FindEmpty(map);
        
        var originalGalaxies = map.Keys().Where(x => map[x]).ToArray();
        var expandedGalaxies = originalGalaxies.Select(g =>
        {
            var emptyRowsBefore = empty.Rows.Count(y => y < g.Y);
            var emptyColsBefore = empty.Cols.Count(x => x < g.X);
            
            return new Position(g.X + emptyColsBefore * (factor - 1), g.Y + emptyRowsBefore * (factor - 1));
        }).ToArray();
        
        var distance = 0L;
        for (var i = 0; i < expandedGalaxies.Length; i++)
        {
            for (var j = i + 1; j < expandedGalaxies.Length; j++)
            {
                distance += (expandedGalaxies[i] - expandedGalaxies[j]).BlockDistance();
            }
        }

        return distance;
    }
    
    private static (IReadOnlyCollection<int> Rows, IReadOnlyCollection<int> Cols) FindEmpty(Grid<bool> map)
    {
        var emptyRows = new HashSet<int>();
        var emptyCols = new HashSet<int>();

        for (var y = 0; y < map.Height; y++)
        {
            var isEmpty = Enumerable.Range(0, map.Width).All(x => !map[new Position(x, y)]);
            if (isEmpty) emptyRows.Add(y);
        }
        
        for (var x = 0; x < map.Width; x++)
        {
            var isEmpty = Enumerable.Range(0, map.Height).All(y => !map[new Position(x, y)]);
            if (isEmpty) emptyCols.Add(x);
        }
        
        return (emptyRows, emptyCols);
    }
    
    public record Model(Grid<bool> Map);
}