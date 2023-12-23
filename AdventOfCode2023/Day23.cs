using System.Collections.Immutable;
using FluentAssertions.Equivalency;

namespace AdventOfCode2023;

[Day]
public partial class Day23 : Day<Day23.Model, int, int>
{
    protected override Model Parse(string input) => new (GridParser.ParseChar(input, x => x switch
    {
        '.' => Cell.Empty,
        '#' => Cell.Forest,
        '^' => Cell.SteepUp,
        'v' => Cell.SteepDown,
        '<' => Cell.SteepLeft,
        '>' => Cell.SteepRight,
    }));

    [Sample("#.#####################\n#.......#########...###\n#######.#########.#.###\n###.....#.>.>.###.#.###\n###v#####.#v#.###.#.###\n###.>...#.#.#.....#...#\n###v###.#.#.#########.#\n###...#.#.#.......#...#\n#####.#.#.#######.#.###\n#.....#.#.#.......#...#\n#.#####.#.#.#########v#\n#.#...#...#...###...>.#\n#.#.#v#######v###.###v#\n#...#.>.#...>.>.#.###.#\n#####v#.#.###v#.#.###.#\n#.....#...#...#.#.#...#\n#.#########.###.#.#.###\n#...###...#...#...#.###\n###.###.#.###v#####v###\n#...#...#.#.>.>.#.>.###\n#.###.###.#.###.#.#v###\n#.....###...###...#...#\n#####################.#", 94)]
    protected override int Part1(Model input) =>
        FindMaxPath(BuildGraph(input.Map, p =>
        {
            return input.Map[p] switch
            {
                Cell.Empty => p.OrthogonalNeighbours(),

                Cell.SteepUp => new[] { p + new Position(0, -1) },
                Cell.SteepDown => new[] { p + new Position(0, 1) },
                Cell.SteepLeft => new[] { p + new Position(-1, 0) },
                Cell.SteepRight => new[] { p + new Position(1, 0) },
            };
        }));

    [Sample("#.#####################\n#.......#########...###\n#######.#########.#.###\n###.....#.>.>.###.#.###\n###v#####.#v#.###.#.###\n###.>...#.#.#.....#...#\n###v###.#.#.#########.#\n###...#.#.#.......#...#\n#####.#.#.#######.#.###\n#.....#.#.#.......#...#\n#.#####.#.#.#########v#\n#.#...#...#...###...>.#\n#.#.#v#######v###.###v#\n#...#.>.#...>.>.#.###.#\n#####v#.#.###v#.#.###.#\n#.....#...#...#.#.#...#\n#.#########.###.#.#.###\n#...###...#...#...#.###\n###.###.#.###v#####v###\n#...#...#.#.>.>.#.>.###\n#.###.###.#.###.#.#v###\n#.....###...###...#...#\n#####################.#", 154)]
    protected override int Part2(Model input) => FindMaxPath(BuildGraph(input.Map, p => p.OrthogonalNeighbours()));

    private static int FindMaxPath(Graph graph)
    {
        var maxPath = FindPath(ImmutableList<(Position, int)>.Empty.Add((graph.Start, 0)), graph.End).MaxBy(x => x.Sum(y => y.Item2));

        return maxPath.Sum(y => y.Item2);
        
        IEnumerable<ImmutableList<(Position, int)>> FindPath(ImmutableList<(Position, int)> path, Position target)
        {
            var (current, _) = path.Last();
            if (current == target) return new[] { path };

            var neighbours = graph.Edges[current];

            return neighbours.Where(x => path.All(y => y.Item1 != x.Item1)).SelectMany(x => FindPath(path.Add(x), target));
        }
    }

    private static Graph BuildGraph(Grid<Cell> map, Func<Position, IEnumerable<Position>> neighboursSelector)
    {
        var nodes = map.Keys()
            .Where(x => map[x] != Cell.Forest)
            .Where(x => x.OrthogonalNeighbours().Where(y => map.IsValid(y)).Count(y => map[y] != Cell.Forest) > 2)
            .ToHashSet();
        
        var start = Enumerable.Range(0, map.Width).Select(x => new Position(x, 0)).First(p => map[p] == Cell.Empty);
        var end = Enumerable.Range(0, map.Width).Select(x => new Position(x, map.Height - 1)).First(p => map[p] == Cell.Empty);

        nodes.Add(start);
        nodes.Add(end);

        var edges = new Dictionary<Position, IReadOnlyCollection<(Position, int)>>();
        
        foreach (var node in nodes)
        {
            var others = new List<(Position, int)>();
            var seen = new HashSet<Position>();
            seen.Add(node);
            var search = new Queue<(Position, int)>();
            search.Enqueue((node, 0));
            while (search.Count > 0)
            {
                var (position, distance) = search.Dequeue();
                
                foreach (var neighbour in neighboursSelector(position))
                {
                    if (!seen.Add(neighbour)) continue;

                    if (!map.IsValid(neighbour)) continue;
                    if (map[neighbour] == Cell.Forest) continue;

                    if (distance > 0 && nodes.Contains(neighbour))
                    {
                        others.Add((neighbour, distance + 1));
                    }
                    else
                    {
                        search.Enqueue((neighbour, distance + 1));
                    }
                }
                
            }

            edges[node] = others;
        }

        return new Graph(start, end, nodes, edges);
    }
    
    private record Graph(
        Position Start,
        Position End,
        IReadOnlySet<Position> Nodes, 
        IReadOnlyDictionary<Position, IReadOnlyCollection<(Position Node, int Distance)>> Edges);
    
    public record Model(Grid<Cell> Map);

    public enum Cell
    {
        Empty,
        Forest,
        SteepUp,
        SteepDown,
        SteepLeft,
        SteepRight,
    }
}