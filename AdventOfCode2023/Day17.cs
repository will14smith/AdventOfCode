namespace AdventOfCode2023;

[Day]
public partial class Day17 : Day<Day17.Model, int, int>
{
    protected override Model Parse(string input) => new (GridParser.ParseInt(input));

    [Sample("2413432311323\n3215453535623\n3255245654254\n3446585845452\n4546657867536\n1438598798454\n4457876987766\n3637877979653\n4654967986887\n4564679986453\n1224686865563\n2546548887735\n4322674655533", 102)]
    protected override int Part1(Model input) => Solve(input, 1, 3);

    [Sample("2413432311323\n3215453535623\n3255245654254\n3446585845452\n4546657867536\n1438598798454\n4457876987766\n3637877979653\n4654967986887\n4564679986453\n1224686865563\n2546548887735\n4322674655533", 94)]
    [Sample("111111111111\n999999999991\n999999999991\n999999999991\n999999999991", 71)]
    protected override int Part2(Model input) => Solve(input, 4, 10);

    private static int Solve(Model input, int minTravel, int maxTravel)
    {
        var start = Position.Identity;
        var end = new Position(input.Map.Width - 1, input.Map.Height - 1);

        var startNode = new GraphNode(start, Position.Identity);
        var distance = new Dictionary<GraphNode, int> { { startNode, 0 } };

        var search = new PriorityQueue<(GraphNode, int), int>();
        search.Enqueue((startNode, 0), 0);

        while (search.Count > 0)
        {
            var (node, nodeDistance) = search.Dequeue();

            if (node.Current == end)
            {
                return nodeDistance;
            }

            if (nodeDistance != distance[node])
            {
                continue;
            }
            
            foreach (var (neighbour, edgeDistance) in node.Neighbours(input.Map, minTravel, maxTravel))
            {
                var neighbourDistance = nodeDistance + edgeDistance;
                if (!distance.TryGetValue(neighbour, out var currentDistance) || neighbourDistance < currentDistance)
                {
                    distance[neighbour] = neighbourDistance;
                    search.Enqueue((neighbour, neighbourDistance), neighbourDistance);
                }
            }
        }
        
        throw new Exception("no");
    }

    public record Model(Grid<int> Map);

    public record GraphNode(Position Current, Position Heading)
    {
        public IEnumerable<(GraphNode, int)> Neighbours(Grid<int> map, int minTravel, int maxTravel)
        {
            var output = Enumerable.Empty<(GraphNode, int)>();
            
            // don't allow continuing in same direction or reversing
            if (Heading.Y == 0)
            {
                output = output.Concat(Traverse(map, minTravel, maxTravel, new Position(0, -1)));
                output = output.Concat(Traverse(map, minTravel, maxTravel, new Position(0, 1)));
            }

            if (Heading.X == 0)
            {
                output = output.Concat(Traverse(map, minTravel, maxTravel, new Position(-1, 0)));
                output = output.Concat(Traverse(map, minTravel, maxTravel, new Position(1, 0)));
            }

            return output;
        }
        
        private IEnumerable<(GraphNode, int)> Traverse(Grid<int> map, int minTravel, int maxTravel, Position heading)
        {
            var position = Current;
            var distance = 0;
            
            for (var dy = 1; dy <= maxTravel; dy++)
            {
                position += heading;
                if(!map.IsValid(position)) yield break;
                    
                distance += map[position];

                if (dy >= minTravel)
                {
                    yield return (new GraphNode(position, heading), distance);
                }
            }
        }
    }
}