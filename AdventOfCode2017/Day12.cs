using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2017;

[Day]
public partial class Day12 : ParseLineDay<Day12.Model, int, int>
{
    public record Model(int Id, IReadOnlyList<int> Connections);

    [Sample("0 <-> 2\n1 <-> 1\n2 <-> 0, 3, 4\n3 <-> 2, 4\n4 <-> 2, 3, 6\n5 <-> 6\n6 <-> 4, 5", 6)]
    protected override int Part1(IEnumerable<Model> input)
    {
        var pipes = input.ToDictionary(x => x.Id);
        var seen = ExploreConnections(pipes, 0);

        return seen.Count;
    }
    
    [Sample("0 <-> 2\n1 <-> 1\n2 <-> 0, 3, 4\n3 <-> 2, 4\n4 <-> 2, 3, 6\n5 <-> 6\n6 <-> 4, 5", 2)]
    protected override int Part2(IEnumerable<Model> input)
    {
        var pipes = input.ToDictionary(x => x.Id);

        var groups = 0;
        while (pipes.Count > 0)
        {
            groups++;

            var seen = ExploreConnections(pipes, pipes.First().Key);
            
            foreach (var id in seen)
            {
                pipes.Remove(id);
            }
        }

        return groups;
    }

    private static IReadOnlySet<int> ExploreConnections(IReadOnlyDictionary<int, Model> pipes, int seed)
    {
        var seen = new HashSet<int>();
        var queue = new Queue<int>();

        queue.Enqueue(seed);

        while (queue.Count > 0)
        {
            var id = queue.Dequeue();
            var pipe = pipes[id];

            foreach (var connection in pipe.Connections)
            {
                if (seen.Add(connection))
                {
                    queue.Enqueue(connection);
                }
            }
        }

        return seen;
    }
    
    protected override TextParser<Model> LineParser { get; } =
        from id in Numerics.IntegerInt32
        from _ in Span.EqualTo(" <-> ")
        from connections in Numerics.IntegerInt32.ManyDelimitedBy(Span.EqualTo(", "))
        select new Model(id, connections);
}
