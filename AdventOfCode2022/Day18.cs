namespace AdventOfCode2022;

[Day]
public partial class Day18 : LineDay<Day18.Position3, int, int>
{
    private const string Sample = "2,2,2\n1,2,2\n3,2,2\n2,1,2\n2,3,2\n2,2,1\n2,2,3\n2,2,4\n2,2,6\n1,2,5\n3,2,5\n2,1,5\n2,3,5";
    
    protected override Position3 ParseLine(string input)
    {
        var elems = input.Split(',').Select(int.Parse).ToArray();
        return new Position3(elems[0], elems[1], elems[2]);
    }
    
    [Sample(Sample, 64)]
    protected override int Part1(IEnumerable<Position3> input)
    {
        var cubes = input.ToHashSet();

        var deltas = new[]
        {
            new Position3(1, 0, 0),
            new Position3(-1, 0, 0),
            new Position3(0, 1, 0),
            new Position3(0, -1, 0),
            new Position3(0, 0, 1),
            new Position3(0, 0, -1),
        };
        
        var faces = 0;
        foreach (var cube in cubes)
        {
            foreach (var delta in deltas)
            {
                if (!cubes.Contains(cube + delta))
                {
                    faces++;
                }
            }
        }

        return faces;
    }

    [Sample(Sample, 58)]
    protected override int Part2(IEnumerable<Position3> input)
    {
        var cubes = input.ToHashSet();
        var bounds = CalculateBounds(cubes);
        var tl = bounds.TL + new Position3(-1, -1, -1);
        var br = bounds.BR + new Position3(1, 1, 1);

        var deltas = new[]
        {
            new Position3(1, 0, 0),
            new Position3(-1, 0, 0),
            new Position3(0, 1, 0),
            new Position3(0, -1, 0),
            new Position3(0, 0, 1),
            new Position3(0, 0, -1),
        };

        var frontier = new Queue<Position3>();
        frontier.Enqueue(tl);
        
        var filled = new HashSet<Position3>();

        while (frontier.Count > 0)
        {
            var pos = frontier.Dequeue();
            if (!filled.Add(pos))
            {
                continue;
            }
            
            foreach (var delta in deltas)
            {
                var next = pos + delta;
                if (!InRegion((tl, br), next))
                {
                    continue;
                }

                if (!cubes.Contains(next))
                {
                    frontier.Enqueue(next);
                } 
            }
        }

        var faces = 0;
        foreach (var cube in cubes)
        {
            foreach (var delta in deltas)
            {
                if (filled.Contains(cube + delta))
                {
                    faces++;
                }
            }
        }

        return faces;
    }
    
    private (Position3 TL, Position3 BR) CalculateBounds(HashSet<Position3> cubes)
    {
        int minX = int.MaxValue, minY = int.MaxValue, minZ = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue, maxZ = int.MinValue;

        foreach (var cube in cubes)
        {
            minX = Math.Min(minX, cube.X);
            minY = Math.Min(minY, cube.Y);
            minZ = Math.Min(minZ, cube.Z);
            
            maxX = Math.Max(maxX, cube.X);
            maxY = Math.Max(maxY, cube.Y);
            maxZ = Math.Max(maxZ, cube.Z);
        }

        return (new Position3(minX, minY, minZ), new Position3(maxX, maxY, maxZ));
    }
    
    private bool InRegion((Position3 TL, Position3 BR) region, Position3 pos)
    {
        if (pos.X < region.TL.X) return false;
        if (pos.Y < region.TL.Y) return false;
        if (pos.Z < region.TL.Z) return false;
        
        if (pos.X > region.BR.X) return false;
        if (pos.Y > region.BR.Y) return false;
        if (pos.Z > region.BR.Z) return false;

        return true;
    }


    public record Position3(int X, int Y, int Z)
    {
        public static Position3 operator +(Position3 a, Position3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }
}
