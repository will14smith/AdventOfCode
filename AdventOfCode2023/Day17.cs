using System.Collections;
using System.Collections.Immutable;

namespace AdventOfCode2023;

[Day]
public partial class Day17 : Day<Day17.Model, int, int>
{
    protected override Model Parse(string input) => new (GridParser.ParseInt(input));

    [Sample("2413432311323\n3215453535623\n3255245654254\n3446585845452\n4546657867536\n1438598798454\n4457876987766\n3637877979653\n4654967986887\n4564679986453\n1224686865563\n2546548887735\n4322674655533", 102)]
    protected override int Part1(Model input)
    {
        var start = Position.Identity;
        var end = new Position(input.Map.Width - 1, input.Map.Height - 1);

        var startNode = new GraphNode(start, Direction.Down, 0);

        var distance = new Dictionary<GraphNode, int> { { startNode, 0 } };
        var previous = new Dictionary<GraphNode, GraphNode>();

        var search = new PriorityQueue<(GraphNode, int), int>();
        search.Enqueue((startNode, 0), 0);

        while (search.Count > 0)
        {
            var (node, nodeDistance) = search.Dequeue();

            if (node.Current == end)
            {
                return nodeDistance;
            }
            if (nodeDistance != distance[node]) continue;
            
            foreach (var neighbour in node.Neighbours())
            {
                if (!input.Map.IsValid(neighbour.Current)) continue;
                
                var neighbourDistance = nodeDistance + input.Map[neighbour.Current];
                if (!distance.TryGetValue(neighbour, out var currentDistance) || neighbourDistance < currentDistance)
                {
                    distance[neighbour] = neighbourDistance;
                    previous[neighbour] = node;
                    
                    search.Enqueue((neighbour, neighbourDistance), neighbourDistance);
                }
            }
        }
        
        throw new Exception("no");
    }
    
    [Sample("2413432311323\n3215453535623\n3255245654254\n3446585845452\n4546657867536\n1438598798454\n4457876987766\n3637877979653\n4654967986887\n4564679986453\n1224686865563\n2546548887735\n4322674655533", 94)]
    [Sample("111111111111\n999999999991\n999999999991\n999999999991\n999999999991", 71)]
    protected override int Part2(Model input)
    {
        var start = Position.Identity;
        var end = new Position(input.Map.Width - 1, input.Map.Height - 1);

        var startNode = (start, Direction.Null);
        var distance = new Dictionary<(Position, Direction), int> { { startNode, 0 } };
        var previous = new Dictionary<(Position, Direction), (Position, Direction)>();

        var search = new PriorityQueue<((Position, Direction), int), int>();
        search.Enqueue((startNode, 0), 0);

        while (search.Count > 0)
        {
            var (node, nodeDistance) = search.Dequeue();

            if (node.Item1 == end)
            {
                while (node.Item1 != start)
                {
                    Output.WriteLine($"{node} {distance[node]}");
                    node = previous[node];
                }
                Output.WriteLine($"{node} {distance[node]}");
                
                return nodeDistance;
            }
            if (nodeDistance != distance[node]) continue;
            
            foreach (var (neighbour, edgeDistance) in Neighbours(node))
            {
                if (!input.Map.IsValid(neighbour.Item1)) continue;
                
                var neighbourDistance = nodeDistance + edgeDistance;
                if (!distance.TryGetValue(neighbour, out var currentDistance) || neighbourDistance < currentDistance)
                {
                    distance[neighbour] = neighbourDistance;
                    previous[neighbour] = node;
                    
                    search.Enqueue((neighbour, neighbourDistance), neighbourDistance);
                }
            }
        }
        
        throw new Exception("no");
        
        IEnumerable<((Position, Direction), int)> Neighbours((Position, Direction) node)
        {
            var (position, direction) = node;

            if (direction != Direction.Up && direction != Direction.Down)
            {
                var edgeUp = 0;
                var edgeDown = 0;

                var up = position;
                var down = position;

                for (var dy = 1; dy <= 10; dy++)
                {
                    up += new Position(0, -1);
                    down += new Position(0, 1);

                    edgeUp += input.Map.IsValid(up) ? input.Map[up] : 0;
                    edgeDown += input.Map.IsValid(down) ? input.Map[down] : 0;

                    if (dy >= 4)
                    {
                        yield return ((up, Direction.Up), edgeUp);
                        yield return ((down, Direction.Down), edgeDown);
                    }
                }
            }

            if (direction != Direction.Left && direction != Direction.Right)
            {
                var edgeLeft = 0;
                var edgeRight = 0;

                var left = position;
                var right = position;

                for (var dy = 1; dy <= 10; dy++)
                {
                    left += new Position(-1, 0);
                    right += new Position(1, 0);

                    edgeLeft += input.Map.IsValid(left) ? input.Map[left] : 0;
                    edgeRight += input.Map.IsValid(right) ? input.Map[right] : 0;

                    if (dy >= 4)
                    {
                        yield return ((left, Direction.Left), edgeLeft);
                        yield return ((right, Direction.Right), edgeRight);
                    }
                }
            }
        }
    }

    public record Model(Grid<int> Map);

    public record GraphNode(Position Current, Direction Direction, int DirectionCount)
    {
        public IEnumerable<GraphNode> Neighbours()
        {
            var up = Current + new Position(0, -1);
            var down = Current + new Position(0, 1);
            var left = Current + new Position(-1, 0);
            var right = Current + new Position(1, 0);

            switch (Direction)
            {
                case Direction.Up:
                    if(DirectionCount < 2) yield return new GraphNode(up, Direction.Up, DirectionCount + 1);
                    yield return new GraphNode(left, Direction.Left, 0);
                    yield return new GraphNode(right, Direction.Right, 0);
                    break;
                
                case Direction.Down:
                    if(DirectionCount < 2) yield return new GraphNode(down, Direction.Down, DirectionCount + 1);
                    yield return new GraphNode(left, Direction.Left, 0);
                    yield return new GraphNode(right, Direction.Right, 0);
                    break;

                case Direction.Left:
                    yield return new GraphNode(up, Direction.Up, 0);
                    yield return new GraphNode(down, Direction.Down, 0);
                    if(DirectionCount < 2) yield return new GraphNode(left, Direction.Left, DirectionCount + 1);
                    break;

                case Direction.Right:
                    yield return new GraphNode(up, Direction.Up, 0);
                    yield return new GraphNode(down, Direction.Down, 0);
                    if(DirectionCount < 2) yield return new GraphNode(right, Direction.Right, DirectionCount + 1);
                    break;
            } 
        }
    }
    
    public enum Direction
    {
        Null,
        
        Up,
        Down,
        Left,
        Right,
    }
}