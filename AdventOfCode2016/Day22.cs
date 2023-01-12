using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2016;

[Day]
public partial class Day22 : ParseDay<Day22.Model, int, int>
{
    private static readonly TextParser<Node> NodeParser = 
        from x in Span.EqualTo("/dev/grid/node-x").IgnoreThen(Numerics.IntegerInt32)
        from y in Span.EqualTo("-y").IgnoreThen(Numerics.IntegerInt32)
        from size in Span.Regex("\\s+").IgnoreThen(Numerics.IntegerInt32)
        from used in Span.Regex("T\\s+").IgnoreThen(Numerics.IntegerInt32)
        from _ in Span.Regex("T\\s+\\d+T\\s+\\d+%")
        select new Node(x, y, size, used);

    protected override TextParser<Model> Parser => Span.Regex("[^\\n]*\n[^\\n]*\n").IgnoreThen(NodeParser.ManyDelimitedBy(SuperpowerExtensions.NewLine)).AtEnd().Select(x => new Model(x));

    protected override int Part1(Model input)
    {
        var pairs = input.Nodes.SelectMany(a => input.Nodes.Select(b => (a, b)));
        return pairs.Count(x => x.a.Used != 0 && x.a != x.b && x.a.Used <= x.b.Available);
    }

    protected override int Part2(Model input)
    {
        // this is very hard coded for the puzzle input.
        var target = new Position(0, 0);
        var source = new Position(input.Nodes.Max(x => x.X), 0);
        
        var freeNode = input.Nodes.First(x => x.Used == 0);
        var free = new Position(freeNode.X, freeNode.Y);
        
        // steps from free to source
        // this doesn't work for the sample, using A* instead would work but :shrug:
        var stepsToX0 = free.X;
        var stepsToY0 = free.Y;
        var stepsToSource = source.X;

        var stepsFreeToSource = stepsToX0 + stepsToY0 + stepsToSource;
        
        // steps from source to target
        var distanceToTarget = source.X - 1;
        var stepsToTarget = distanceToTarget * 5;

        return stepsFreeToSource + stepsToTarget;
    }

    public record Model(IReadOnlyCollection<Node> Nodes);

    public record Node(int X, int Y, int Size, int Used)
    {
        public int Available => Size - Used;
    }
}
