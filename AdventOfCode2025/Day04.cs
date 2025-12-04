namespace AdventOfCode2025;

[Day]
public partial class Day04 : Day<Day04.Model, int, int>
{
    public record Model(Grid<bool> Map);

    protected override Model Parse(string input) => new(GridParser.ParseBool(input.Trim(), '@'));

    [Sample("..@@.@@@@.\n@@@.@.@.@@\n@@@@@.@.@@\n@.@@@@..@.\n@@.@@@@.@@\n.@@@@@@@.@\n.@.@.@.@@@\n@.@@@.@@@@\n.@@@@@@@@.\n@.@.@@@.@.\n", 13)]
    protected override int Part1(Model input) => input.Map.Keys().Count(position => input.Map[position] && IsAccessible(input.Map, position));

    [Sample("..@@.@@@@.\n@@@.@.@.@@\n@@@@@.@.@@\n@.@@@@..@.\n@@.@@@@.@@\n.@@@@@@@.@\n.@.@.@.@@@\n@.@@@.@@@@\n.@@@@@@@@.\n@.@.@@@.@.\n", 43)]
    protected override int Part2(Model input)
    {
        var count = 0;
        
        while (true)
        {
            var toRemove = input.Map.Keys().Where(position => input.Map[position] && IsAccessible(input.Map, position)).ToList();
            if (toRemove.Count == 0)
            {
                break;
            }
        
            count += toRemove.Count;
            
            foreach (var position in toRemove)
            {
                input.Map[position] = false;
            }
        }
        
        return count;
    }

    private static bool IsAccessible(Grid<bool> map, Position position)
    {
        var occupiedNeighbours = position.StrictNeighbours().Count(x => map.IsValid(x) && map[x]);

        return occupiedNeighbours < 4;
    }
}