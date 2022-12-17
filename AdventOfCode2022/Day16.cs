using System.Collections;
using System.Collections.Immutable;
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
        
        var search = new PriorityQueue<State, int>();
        search.Enqueue(new State("AA", 29, 0, ImmutableHashSet<string>.Empty), 0);

        var maxState = search.Peek();
        
        while (search.Count > 0)
        {
            var state = search.Dequeue();
            if (state.PressureReleased > maxState.PressureReleased)
            {
                maxState = state;
            }
            
            foreach (var next in Next(graph, state))
            {
                if (next.TimeLeft <= 0)
                {
                    continue;
                }
                
                search.Enqueue(next, -next.PressureReleased);
            }
        }

        return maxState.PressureReleased; 
    }
    
    [Sample(Sample, 1707)]
    protected override int Part2(IEnumerable<Input> input)
    {
        throw new NotImplementedException();
    }

    private static IEnumerable<State> Next(Graph graph, State state)
    {
        foreach (var destination in ValidDestinations(graph, state))
        {
            var distanceToDestination = graph.Distances[(state.Location, destination)];
            var timeRemaining = state.TimeLeft - distanceToDestination;

            var gain = ExpectedGain(graph, destination, timeRemaining);

            yield return new State(destination, timeRemaining - 1, state.PressureReleased + gain,
                state.Open.Add(destination));
        }
    }

    private static IEnumerable<string> ValidDestinations(Graph graph, State state) => graph.Nodes.Where(x => x.Value > 0 && !state.Open.Contains(x.Key)).Select(x => x.Key);
    private static int ExpectedGain(Graph graph, string target, int timeRemaining) => graph.Nodes[target] * timeRemaining;

    private static Graph BuildGraph(IReadOnlyCollection<Input> input)
    {
        var nodes = input.ToDictionary(x => x.Name, x => x.FlowRate);
        var edges = input.ToDictionary(x => x.Name, x => x.LeadsTo);

        var distances = new Dictionary<(string, string), int>();
        
        foreach (var (node, _) in nodes)
        {
            foreach (var edge in edges[node])
            {
                distances[(node, edge)] = 1;
            }

            distances[(node, node)] = 0;
        }

        var numberOfNodes = nodes.Count;
        var nodesNames = nodes.Keys.ToArray(); 
        
        for (var k = 0; k < numberOfNodes; k++)
        {
            for (var i = 0; i < numberOfNodes; i++)
            {
                for (var j = 0; j < numberOfNodes; j++)
                {
                    var distIJ = distances.TryGetValue((nodesNames[i], nodesNames[j]), out var a) ? a : int.MaxValue;
                    var distIK = distances.TryGetValue((nodesNames[i], nodesNames[k]), out var b) ? b : int.MaxValue;
                    var distKJ = distances.TryGetValue((nodesNames[k], nodesNames[j]), out var c) ? c : int.MaxValue;

                    var indirect = distIK == int.MaxValue || distKJ == int.MaxValue ? int.MaxValue : distIK + distKJ;
                    if (indirect != int.MaxValue && distIJ > indirect)
                    {
                        distances[(nodesNames[i], nodesNames[j])] = indirect;
                    }
                }
            }
        }
        
        return new Graph(nodes, edges, distances);
    }
    
    public record Input(string Name, int FlowRate, IReadOnlyList<string> LeadsTo);
    public record Graph(IReadOnlyDictionary<string, int> Nodes, IReadOnlyDictionary<string, IReadOnlyList<string>> Edges, IReadOnlyDictionary<(string, string), int> Distances);
    public record State(string Location, int TimeLeft, int PressureReleased, ImmutableHashSet<string> Open);
}
