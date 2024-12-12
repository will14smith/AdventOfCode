namespace AdventOfCode2024;

[Day]
public partial class Day12 : Day<Day12.Model, int, int>
{
    public record Model(Grid<char> Map);
    
    protected override Model Parse(string input) => new(GridParser.ParseChar(input, x => x));
    
    [Sample("AAAA\nBBCD\nBBCC\nEEEC", 140)]
    [Sample("OOOOO\nOXOXO\nOOOOO\nOXOXO\nOOOOO", 772)]
    [Sample("RRRRIICCFF\nRRRRIICCCF\nVVRRRCCFFF\nVVRCCCJFFF\nVVVVCJJCFE\nVVIVCCJJEE\nVVIIICJJEE\nMIIIIIJJEE\nMIIISIJEEE\nMMMISSJEEE", 1930)]
    protected override int Part1(Model input)
    {
        var map = RenameRegions(input.Map);
        var regions = map.Keys().GroupBy(x => map[x]);
        
        return regions.Sum(x => x.Count() * FindSides(x.ToHashSet()).Count);
    }
    
    [Sample("AAAA\nBBCD\nBBCC\nEEEC", 80)]
    [Sample("OOOOO\nOXOXO\nOOOOO\nOXOXO\nOOOOO", 436)]
    [Sample("EEEEE\nEXXXX\nEEEEE\nEXXXX\nEEEEE", 236)]
    [Sample("AAAAAA\nAAABBA\nAAABBA\nABBAAA\nABBAAA\nAAAAAA", 368)]
    [Sample("RRRRIICCFF\nRRRRIICCCF\nVVRRRCCFFF\nVVRCCCJFFF\nVVVVCJJCFE\nVVIVCCJJEE\nVVIIICJJEE\nMIIIIIJJEE\nMIIISIJEEE\nMMMISSJEEE", 1206)]
    protected override int Part2(Model input)
    {
        var map = RenameRegions(input.Map);
        var regions = map.Keys().GroupBy(x => map[x]);

        return regions.Sum(region => region.Count() * MergeSides(FindSides(region.ToHashSet())).Count);
    }
    
    private Grid<int> RenameRegions(Grid<char> input)
    {
        // rename non-contiguous regions to be unique
        var output = Grid<int>.Empty(input.Width, input.Height);

        var counter = 1;
        var visited = new HashSet<Position>();
        foreach (var position in input.Keys())
        {
            if (!visited.Add(position))
            {
                continue;
            }
            
            var originalRegion = input[position];
            var newRegion = counter++;
            
            var queue = new Queue<Position>();
            queue.Enqueue(position);
            while (queue.Count > 0)
            {
                var currentPosition = queue.Dequeue();
                output[currentPosition] = newRegion;
                
                foreach (var neighbour in currentPosition.OrthogonalNeighbours())
                {
                    if (!input.IsValid(neighbour)) continue;
                    if (input[neighbour] != originalRegion) continue;
                    if (!visited.Add(neighbour)) continue;
                    
                    queue.Enqueue(neighbour);
                }
            }
        }
        
        return output;
    }

    private static HashSet<(Position, Position)> FindSides(IReadOnlySet<Position> positions)
    {
        var sides = new HashSet<(Position, Position)>();

        foreach (var position in positions)
        {
            // the side positions are on a 3x grid to avoid the bottom edges lining up with the top edges of a region below
            if (!positions.Contains(position - new Position(1, 0))) sides.Add((position * 3 - new Position(1, 0), position * 3 + new Position(-1, 3)));
            if (!positions.Contains(position + new Position(1, 0))) sides.Add((position * 3 + new Position(4, 0), position * 3 + new Position(4, 3)));
            if (!positions.Contains(position - new Position(0, 1))) sides.Add((position * 3 - new Position(0, 1), position * 3 + new Position(3, -1)));
            if (!positions.Contains(position + new Position(0, 1))) sides.Add((position * 3 + new Position(0, 4), position * 3 + new Position(3, 4)));
        }

        return sides;
    }
    
    private static HashSet<(Position, Position)> MergeSides(HashSet<(Position, Position)> sides)
    {
        var toMerge = new Queue<(Position, Position)>(sides);
        var visited = new HashSet<(Position, Position)>();
        var merged = new HashSet<(Position, Position)>();
        
        while (toMerge.Count > 0)
        {
            var initialSide = toMerge.Dequeue();
            if (!visited.Add(initialSide))
            {
                continue;
            }
            
            var start = initialSide.Item1;
            var end = initialSide.Item2;

            var delta = initialSide.Item2 - start;
            while (true)
            {
                if (!sides.Contains((end, end + delta)))
                {
                    break;
                }

                visited.Add((end, end + delta));
                end += delta;
            }

            merged.Add((start, end));
        }
        
        return merged;
    }
}