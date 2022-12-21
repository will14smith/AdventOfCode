using System.Collections.Concurrent;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2022;

[Day]
public partial class Day16 : ParseLineDay<Day16.Input, int, int>
{
    private const string Sample = "Valve AA has flow rate=0; tunnels lead to valves DD, II, BB\nValve BB has flow rate=13; tunnels lead to valves CC, AA\nValve CC has flow rate=2; tunnels lead to valves DD, BB\nValve DD has flow rate=20; tunnels lead to valves CC, AA, EE\nValve EE has flow rate=3; tunnels lead to valves FF, DD\nValve FF has flow rate=0; tunnels lead to valves EE, GG\nValve GG has flow rate=0; tunnels lead to valves FF, HH\nValve HH has flow rate=22; tunnel leads to valve GG\nValve II has flow rate=0; tunnels lead to valves AA, JJ\nValve JJ has flow rate=21; tunnel leads to valve II";

    private static readonly TextParser<string> ValueNameParser = Span.Regex("[A-Z]{2}").Select(x => x.ToStringValue());

    protected override TextParser<Input> LineParser =>
        from name in Span.EqualTo("Valve ").IgnoreThen(ValueNameParser)
        from flowRate in Span.EqualTo(" has flow rate=").IgnoreThen(Numerics.IntegerInt32)
        from leadsTo in Span.Regex("; tunnels? leads? to valves? ").IgnoreThen(ValueNameParser.ManyDelimitedBy(Span.EqualTo(", ")))
        select new Input(name, flowRate, leadsTo);

    [Sample(Sample, 1651)]
    protected override int Part1(IEnumerable<Input> input)
    {
        var inputList = input.ToList();
        var graph = BuildGraph(inputList);

        var initialState = new State(graph.Nodes["AA"], 30, 0, new NodeFlags(0));
        return DFS(graph, initialState);
    }
    
    [Sample(Sample, 1707)]
    protected override int Part2(IEnumerable<Input> input)
    {
        var inputList = input.ToList();
        var graph = BuildGraph(inputList);
        
        var initialState = new State(graph.Nodes["AA"], 26, 0, new NodeFlags(0));

        var nodesWithFlow = graph.Flow.Select(x => x.Node).ToList();
        var nodeCombinations = Combinations.Get(nodesWithFlow).Select(ToIndex);
        
        var subgraphScores = new ConcurrentBag<(NodeFlags, int)>();
        nodeCombinations.AsParallel().ForAll(x => subgraphScores.Add((x, DFS(graph, initialState with { Open = new NodeFlags(~x.Nodes) }))));
        
        var orderedSubgraphScores = subgraphScores.OrderByDescending(x => x.Item2).ToList();
        
        var best = 0;
        foreach (var x in orderedSubgraphScores)
        {
            foreach (var y in orderedSubgraphScores)
            {
                var maybe = x.Item2 + y.Item2;
                if (maybe < best)
                {
                    break;
                }
        
                if (maybe > best && (x.Item1.Nodes & y.Item1.Nodes) == 0)
                {
                    best = maybe;
                }
            }
        }
        
        return best;
    }

    private static NodeFlags ToIndex(IEnumerable<int> nodes)
    {
        var flags = 0L;

        foreach (var node in nodes)
        {
            flags |= 1L << node;
        }
        
        return new NodeFlags(flags);
    }
    
    private static int DFS(Graph graph, State state)
    {
        var current = state.PressureReleased;
        var max = current;

        var currentLocation = state.Location;
        var distances = graph.Distances;
        var flows = graph.Flow;
        var openNodes = state.Open.Nodes;
        var timeLeft = state.TimeLeft;

        for (var i = 0; i < flows.Length; i++)
        {
            var (index, flow) = flows[i];
            var nodeMask = 1L << index;

            if ((openNodes & nodeMask) != 0)
            {
                continue;
            }

            var distanceToDestination = distances[currentLocation, index];
            var timeRemaining = timeLeft - distanceToDestination - 1;

            if (timeRemaining <= 0)
            {
                continue;
            }

            var gain = flow * timeRemaining;

            var dfs = DFS(graph, new State(index, timeRemaining, current + gain, new NodeFlags(openNodes | nodeMask)));
            max = Math.Max(max, dfs);
        }

        return max;
    }

    private static Graph BuildGraph(IReadOnlyCollection<Input> input)
    {
        var names = input.Select((x, i) => (Key: i, x.Name)).ToDictionary(x => x.Key, x => x.Name);
        var ids = input.Select((x, i) => (Key: i, x.Name)).ToDictionary(x => x.Name, x => x.Key);
        
        var flow = input.Where(x => x.FlowRate > 0).Select(x => (ids[x.Name], x.FlowRate)).ToArray();
        
        var edges = input.ToDictionary(x => x.Name, x => x.LeadsTo);

        var numberOfNodes = names.Count;

        var distances = new int[numberOfNodes, numberOfNodes];
        FillToInfinity(distances);
        
        foreach (var (node, id) in ids)
        {
            foreach (var edge in edges[node])
            {
                distances[id, ids[edge]] = 1;
            }

            distances[id, id] = 0;
        }
        
        for (var k = 0; k < numberOfNodes; k++)
        {
            for (var i = 0; i < numberOfNodes; i++)
            {
                for (var j = 0; j < numberOfNodes; j++)
                {
                    var distIJ = distances[i, j];
                    var distIK = distances[i, k];
                    var distKJ = distances[k, j];

                    var indirect = distIK == int.MaxValue || distKJ == int.MaxValue ? int.MaxValue : distIK + distKJ;
                    if (indirect != int.MaxValue && distIJ > indirect)
                    {
                        distances[i, j] = indirect;
                    }
                }
            }
        }

        return new Graph(ids, flow, distances);
    }
    
    private static unsafe void FillToInfinity(int[,] distances)
    {
        fixed (int* p = &distances[0, 0])
        {
            new Span<int>(p, distances.Length).Fill(int.MaxValue);
        }
    }

    public record Input(string Name, int FlowRate, IReadOnlyList<string> LeadsTo);
    public record Graph(IReadOnlyDictionary<string, int> Nodes, (int Node, int Flow)[] Flow, int[,] Distances);
    public record State(int Location, int TimeLeft, int PressureReleased, NodeFlags Open);
    
    public record NodeFlags(long Nodes);
}
