namespace AdventOfCode2019;

[Day]
public partial class Day06 : LineDay<Day06.Model, int, int>
{
    protected override Model ParseLine(string input)
    {
        var parts = input.Split(')');
        return new Model(parts[0], parts[1]);
    }

    [Sample("COM)B\nB)C\nC)D\nD)E\nE)F\nB)G\nG)H\nD)I\nE)J\nJ)K\nK)L", 42)]
    protected override int Part1(IEnumerable<Model> input)
    {
        var children = input.ToLookup(x => x.Parent, x => x.Child);
        return Count("COM", 0);
        
        int Count(string node, int depth)
        {
            var nodeChildren = children[node];

            return depth + nodeChildren.Sum(child => Count(child, depth + 1));
        }
    }

    [Sample("COM)B\nB)C\nC)D\nD)E\nE)F\nB)G\nG)H\nD)I\nE)J\nJ)K\nK)L\nK)YOU\nI)SAN", 4)]
    protected override int Part2(IEnumerable<Model> input)
    {
        var parents = input.ToDictionary(x => x.Child, x => x.Parent);

        var sanParents = GetParents("SAN").ToArray();
        var youParents = GetParents("YOU").ToArray();

        var commonParent = sanParents.First(x => youParents.Contains(x));

        return Array.IndexOf(sanParents, commonParent) + Array.IndexOf(youParents, commonParent);
        
        IEnumerable<string> GetParents(string node)
        {
            return parents.TryGetValue(node, out var parent) ? GetParents(parent).Prepend(parent).ToArray() : Array.Empty<string>();
        }
    }
    
    public record Model(string Parent, string Child);
}
