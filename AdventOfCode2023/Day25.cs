namespace AdventOfCode2023;

[Day]
public partial class Day25 : Day<Day25.Model, int, int>
{
    protected override Model Parse(string input)
    {
        var nodes = new HashSet<string>();
        var edges = new HashSet<(string A, string B)>();
        
        foreach (var line in input.Split('\n'))
        {
            var parts1 = line.Split(": ");
            var parts2 = parts1[1].Split(' ');

            nodes.Add(parts1[0]);
            foreach (var part in parts2)
            {
                nodes.Add(part);
                edges.Add(string.Compare(parts1[0], part, StringComparison.Ordinal) < 0 ? (parts1[0], part) : (part, parts1[0]));
            }
        }

        return new Model(nodes, edges);
    }

    [Sample("jqt: rhn xhk nvd\nrsh: frs pzl lsr\nxhk: hfx\ncmg: qnr nvd lhk bvb\nrhn: xhk bvb hfx\nbvb: xhk hfx\npzl: lsr hfx nvd\nqnr: nvd\nntq: jqt hfx bvb xhk\nnvd: lhk\nlsr: lhk\nrzs: qnr cmg lsr rsh\nfrs: qnr lhk lsr", 54)]
    protected override int Part1(Model input)
    {
        var nodes = input.Nodes.Select((x, i) => (x, i)).ToDictionary(x => x.x, x => x.i);
        var weights = new int[input.Nodes.Count, input.Nodes.Count];

        var nv = nodes.Count;
        foreach (var edge in input.Edges)
        {
            var a = nodes[edge.A];
            var b = nodes[edge.B];
            
            weights[a, b] = 1;
            weights[b, a] = 1;
        }

        var removedNodes = new bool[nv];
        var equivalentNodes = new List<int>[nv];
        for (var i = 0; i < equivalentNodes.Length; i++)
        {
            equivalentNodes[i] = [i];
        }
        
        var result = MinimumCut(0);
        return result.Item1.Count * (nodes.Count - result.Item1.Count);

        (IReadOnlyList<int>, int) MinimumCut(int startNode)
        {
            var min = int.MaxValue;
            IReadOnlyList<int>? cut = null;

            while(nv > 1)
            {
                var (index, value) = MinimumCutPhase(startNode);
                if (value >= min) continue;
                
                min = value;
                cut = equivalentNodes[index];
            }

            return (cut!, min);
        }
        
        (int,int) MinimumCutPhase(int startNode)
        {
            var used = new bool[nodes.Count];
            used[startNode] = true;

            var nodeWeights = new int[nodes.Count];

            for(var u = 0; u < nodes.Count; u++)
            {
                if (!removedNodes[u])
                {
                    nodeWeights[u] = weights[startNode, u];
                }
            }

            var prev = startNode;
            for (var i = 1; i < nv; i++)
            {
                var z = -1;
                for(var j = 0; j < nodes.Count; j++)
                {
                    if (!removedNodes[j] && !used[j] && (z < 0 || nodeWeights[j] > nodeWeights[z]))
                    {
                        z = j;
                    }
                }
		
                used[z] = true;

                if(i == nv-1)
                {
                    for (var j = 0; j < nodes.Count; j++)
                    {
                        weights[j, prev] = weights[prev, j] += weights[z, j];
                    }

                    removedNodes[z] = true;
                    foreach (var u in equivalentNodes[z])
                    {
                        equivalentNodes[prev].Add(u);
                    }

                    nv--;
                    return (z, nodeWeights[z]);
                }

                prev = z;
                for(var j = 0; j < nodes.Count; j++)
                    if(!used[j] && !removedNodes[j])
                        nodeWeights[j] += weights[z, j];
            }

            throw new Exception("no");
        }
    }

    protected override int Part2(Model input) => 2023;

    public record Model(IReadOnlySet<string> Nodes, IReadOnlySet<(string A, string B)> Edges);
}