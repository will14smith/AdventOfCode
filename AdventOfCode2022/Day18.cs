namespace AdventOfCode2022;

[Day]
public partial class Day18 : LineDay<Position3, int, int>
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
        
        return cubes.Sum(cube => cube.OrthogonalNeighbours().Count(neighbour => !cubes.Contains(neighbour)));
    }

    [Sample(Sample, 58)]
    protected override int Part2(IEnumerable<Position3> input)
    {
        var cubes = input.ToHashSet();
        
        // create a region with a 1-wide border to allow flood fill to wrap around fully
        var bounds = CalculateBounds(cubes);
        var region = (bounds.TL + new Position3(-1, -1, -1), bounds.BR + new Position3(1, 1, 1));
        
        var surroundingWater = FloodFillRegion(region, cubes);

        return cubes.Sum(cube => cube.OrthogonalNeighbours().Count(neighbour => surroundingWater.Contains(neighbour)));
    }

    private static HashSet<Position3> FloodFillRegion((Position3 TL, Position3 BR) region, IReadOnlySet<Position3> cubes)
    {
        var filled = new HashSet<Position3>();

        var frontier = new Queue<Position3>();
        frontier.Enqueue(region.TL);
        
        while (frontier.Count > 0)
        {
            var pos = frontier.Dequeue();
            if (!filled.Add(pos))
            {
                continue;
            }

            foreach (var next in pos.OrthogonalNeighbours())
            {
                if (!InRegion(region, next) || cubes.Contains(next))
                {
                    continue;
                }

                frontier.Enqueue(next);
            }
        }

        return filled;
    }

    private static (Position3 TL, Position3 BR) CalculateBounds(HashSet<Position3> cubes)
    {
        var min = Position3.MaxValue;
        var max = Position3.MinValue;
        
        foreach (var cube in cubes)
        {
            min = Position3.Min(min, cube);
            max = Position3.Max(max, cube);
        }

        return (min, max);
    }
    
    private static bool InRegion((Position3 TL, Position3 BR) region, Position3 pos) => !(pos < region.TL) && !(pos > region.BR);
}
