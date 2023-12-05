using System.Collections.Immutable;

namespace AdventOfCode2017;

[Day]
public partial class Day24 : Day<Day24.Model, int, int>
{
    protected override Model Parse(string input) => new(input.Split('\n').Select(line =>
    {
        var parts = line.Split('/');
        return (int.Parse(parts[0]), int.Parse(parts[1]));
    }).ToList());

    [Sample("0/2\n2/2\n2/3\n3/4\n3/5\n0/1\n10/1\n9/10", 31)]
    protected override int Part1(Model input) => EnumerateBridges(input.GetNextPortLookup(), 0, ImmutableList<(int, int)>.Empty).Max(Score);

    [Sample("0/2\n2/2\n2/3\n3/4\n3/5\n0/1\n10/1\n9/10", 19)]
    protected override int Part2(Model input)
    {
        var ports = input.GetNextPortLookup();
        var length = EnumerateBridges(ports, 0, ImmutableList<(int, int)>.Empty).Max(x => x.Count);
        return EnumerateBridges(ports, 0, ImmutableList<(int, int)>.Empty).Where(x => x.Count == length).Max(x => x.Sum(y => y.Item1 + y.Item2));
        
    }

    private static IEnumerable<ImmutableList<(int, int)>> EnumerateBridges(ILookup<int, int> ports, int current, ImmutableList<(int, int)> previous)
    {
        yield return previous;
            
        foreach (var next in ports[current])
        {
            if (previous.Contains((current, next))) continue;
            if (previous.Contains((next, current))) continue;

            foreach (var results in EnumerateBridges(ports, next, previous.Add((current, next))))
            {
                yield return results;
            }
        }
    }
    
    private static int Score(ImmutableList<(int, int)> bridge) => bridge.Sum(y => y.Item1 + y.Item2);

    public record Model(IReadOnlyList<(int A, int B)> Ports)
    {
        public ILookup<int, int> GetNextPortLookup() => Ports.Concat(Ports.Select(x => (A: x.B, B: x.A))).Distinct().ToLookup(x => x.A, x => x.B);
    }
}
