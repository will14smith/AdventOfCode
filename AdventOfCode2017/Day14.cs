using System.Numerics;
using System.Text;

namespace AdventOfCode2017;

[Day]
public partial class Day14 : Day<Day14.Model, int, int>
{
    protected override Model Parse(string input) => new (input);

    [Sample("flqrgnkx", 8108)]
    protected override int Part1(Model input)
    {
        var cnt = 0;

        for (int i = 0; i < 128; i++)
        {
            var hash = KnotHash.Standard(Encoding.ASCII.GetBytes(input.Value + "-" + i));

            cnt += hash.Sum(x => BitOperations.PopCount(x));
        }

        return cnt;
    }

    [Sample("flqrgnkx", 1242)]
    protected override int Part2(Model input)
    {
        // build grid
        var grid = Grid.Empty<bool>(128, 128);
        for (var i = 0; i < 128; i++)
        {
            var hash = KnotHash.Standard(Encoding.ASCII.GetBytes(input.Value + "-" + i));

            for (var j = 0; j < 16; j++)
            {
                for (var k = 0; k < 8; k++)
                {
                    var cell = ((hash[j] >> (7 - k)) & 1) == 1;
                    
                    var x = j * 8 + k;
                    grid[x, i] = cell;
                }
            }
        }

        // count groups
        var seen = new HashSet<Position>();
        var groups = 0;

        foreach (var position in grid.Keys())
        {
            if (!grid[position])
            {
                continue;
            }
            
            if (seen.Contains(position))
            {
                continue;
            }

            groups++;
            
            var queue = new Queue<Position>();
            queue.Enqueue(position);

            while (queue.Count > 0)
            {
                var p = queue.Dequeue();
                if (!seen.Add(p))
                {
                    continue;
                }
                
                foreach (var n in p.OrthogonalNeighbours())
                {
                    if (grid.IsValid(n) && !seen.Contains(n) && grid[n])
                    {
                        queue.Enqueue(n);
                    }
                }
            }
        }

        return groups;
    }
    
    public record Model(string Value);
}
