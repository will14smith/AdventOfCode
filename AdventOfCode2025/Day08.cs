namespace AdventOfCode2025;

[Day]
public partial class Day08 : Day<Day08.Model, long, long>
{
    public record Model(IReadOnlyList<Position3> JunctionBoxes);

    protected override Model Parse(string input) => new(input.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split(',')).Select(x => new Position3(int.Parse(x[0]), int.Parse(x[1]), int.Parse(x[2]))).ToList());

    [Sample("162,817,812\n57,618,57\n906,360,560\n592,479,940\n352,342,300\n466,668,158\n542,29,236\n431,825,988\n739,650,466\n52,470,668\n216,146,977\n819,987,18\n117,168,530\n805,96,715\n346,949,466\n970,615,88\n941,993,340\n862,61,35\n984,92,344\n425,690,689\n", 40)]
    protected override long Part1(Model input)
    {
        var sortedDistances = CalculateDistancesAscending(input);

        var circuits = input.JunctionBoxes.ToDictionary(x => x, x => new HashSet<Position3> { x });

        // make 10 connections for the sample, but 1000 for the real
        var numberOfConnections = input.JunctionBoxes.Count == 20 ? 10 : 1000;

        for (var i = 0; i < numberOfConnections; i++)
        {
            var (from, to, _) = sortedDistances[i];
            CombineCircuits(circuits, from, to);
        }

        return circuits.Values
            .Distinct()
            .OrderByDescending(x => x.Count)
            .Take(3)
            .Aggregate(1L, (acc, x) => acc * x.Count);
    }

    private static void CombineCircuits(Dictionary<Position3, HashSet<Position3>> circuits, Position3 from, Position3 to)
    {
        var fromCircuit = circuits[from];
        var toCircuit = circuits[to];

        if (fromCircuit == toCircuit)
        {
            return;
        }
        
        toCircuit.UnionWith(fromCircuit);
        foreach (var box in fromCircuit)
        {
            circuits[box] = toCircuit;
        }
    }

    [Sample("162,817,812\n57,618,57\n906,360,560\n592,479,940\n352,342,300\n466,668,158\n542,29,236\n431,825,988\n739,650,466\n52,470,668\n216,146,977\n819,987,18\n117,168,530\n805,96,715\n346,949,466\n970,615,88\n941,993,340\n862,61,35\n984,92,344\n425,690,689\n", 25272)]
    protected override long Part2(Model input)
    {
        var sortedDistances = CalculateDistancesAscending(input);

        var circuits = input.JunctionBoxes.ToDictionary(x => x, x => new HashSet<Position3> { x });

        var connection = 0;
        while (true)
        {
            var (from, to, _) = sortedDistances[connection++];
            CombineCircuits(circuits, from, to);
            
            if (circuits[to].Count == input.JunctionBoxes.Count)
            {
                return from.X * to.X;
            }
        }
    }
    
    private static IReadOnlyList<(Position3 From, Position3 To, decimal Distance)> CalculateDistancesAscending(Model input)
    {
        var distances = new List<(Position3 From, Position3 To, decimal Distance)>();
        for (var i = 0; i < input.JunctionBoxes.Count; i++)
        {
            for (var j = i + 1; j < input.JunctionBoxes.Count; j++)
            {
                var a = input.JunctionBoxes[i];
                var b = input.JunctionBoxes[j];

                var distance = (a - b).StraightLineDistance();

                distances.Add((a, b, distance));
            }
        }

        return distances.OrderBy(x => x.Distance).ToList();
    }
}