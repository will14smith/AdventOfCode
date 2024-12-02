namespace AdventOfCode2024;

[Day]
public partial class Day02 : LineDay<Day02.Model, int, int>
{
    public record Model(LinkedList<int> Levels);

    protected override Model ParseLine(string input) => new(new LinkedList<int>(input.Split(' ').Select(int.Parse)));

    [Sample("7 6 4 2 1\n1 2 7 8 9\n9 7 6 2 1\n1 3 2 4 5\n8 6 4 4 1\n1 3 6 7 9", 2)]
    protected override int Part1(IEnumerable<Model> input) => input.Count(IsSafe);
    
    [Sample("7 6 4 2 1\n1 2 7 8 9\n9 7 6 2 1\n1 3 2 4 5\n8 6 4 4 1\n1 3 6 7 9", 4)]
    protected override int Part2(IEnumerable<Model> input) => input.Count(IsMostlySafe);
    
    private static bool IsSafe(Model report)
    {
        var deltas = new List<int>();

        var node = report.Levels.First;
        while (node != null)
        {
            if (node.Next != null)
            {
                deltas.Add(node.Next.Value - node.Value);
            }
            
            node = node.Next;
        }

        if (deltas.Any(x => Math.Sign(x) != Math.Sign(deltas[0])))
        {
            return false;
        }

        if (deltas.Any(x => x is < -3 or > 3 or 0))
        {
            return false;
        }
        
        return true;
    }
    
    private static bool IsMostlySafe(Model report)
    {
        if(IsSafe(report)) return true;

        var node = report.Levels.First;
        while (node != null)
        {
            var next = node.Next;
            report.Levels.Remove(node);
            
            if(IsSafe(report)) return true;

            if (next != null)
            {
                report.Levels.AddBefore(next, node);
            }
            node = next;            
        }
        
        return false;
    }
}