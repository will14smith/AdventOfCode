namespace AdventOfCode2024;

[Day]
public partial class Day10 : Day<Day10.Model, int, int>
{
    public record Model(Grid<int> Map);
    protected override Model Parse(string input) => new(GridParser.ParseInt(input));
    
    [Sample("89010123\n78121874\n87430965\n96549874\n45678903\n32019012\n01329801\n10456732", 36)]
    protected override int Part1(Model input) => input.Map.Keys().Where(key => input.Map[key] == 0).Sum(position => CountReachable(input.Map, position));

    private static int CountReachable(Grid<int> map, Position start)
    {
        var queue = new Queue<Position>();
        queue.Enqueue(start);

        var visited = new HashSet<Position>();
        
        while (queue.Count > 0)
        {
            var position  = queue.Dequeue();
            if (map[position] == 9)
            {
                visited.Add(position);
                continue;
            }
            
            foreach (var neighbour in position.OrthogonalNeighbours().Where(map.IsValid))
            {
                if (map[neighbour] == map[position] + 1)
                {
                    queue.Enqueue(neighbour);
                }
            }
        }

        return visited.Count;
    }
    
    [Sample("89010123\n78121874\n87430965\n96549874\n45678903\n32019012\n01329801\n10456732", 81)]
    protected override int Part2(Model input) => input.Map.Keys().Where(key => input.Map[key] == 0).Sum(position => CountRoutes(input.Map, position));

    private static int CountRoutes(Grid<int> map, Position start)
    {
        var queue = new Queue<Position>();
        queue.Enqueue(start);

        var visited = new List<Position>();
        
        while (queue.Count > 0)
        {
            var position = queue.Dequeue();
            if (map[position] == 9)
            {
                visited.Add(position);
                continue;
            }
            
            foreach (var neighbour in position.OrthogonalNeighbours().Where(map.IsValid))
            {
                if (map[neighbour] == map[position] + 1)
                {
                    queue.Enqueue(neighbour);
                }
            }
        }

        return visited.Count;
    }
}