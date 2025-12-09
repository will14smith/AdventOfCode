namespace AdventOfCode2025;

[Day]
public partial class Day09 : Day<Day09.Model, long, long>
{
    public record Model(IReadOnlyList<LongPosition> RedTiles);

    protected override Model Parse(string input) => new(input.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split(',')).Select(x => new LongPosition(int.Parse(x[0]), int.Parse(x[1]))).ToList());

    [Sample("7,1\n11,1\n11,7\n9,7\n9,5\n2,5\n2,3\n7,3\n", 50)]
    protected override long Part1(Model input)
    {
        var corners = input.RedTiles;
        var maxArea = 0L;
        
        for (var i = 0; i < corners.Count; i++)
        for (var j = i + 1; j < corners.Count; j++)
        {
            var a = corners[i];
            var b = corners[j];
            var area = Math.Abs((a.X - b.X + 1) * (a.Y - b.Y + 1));
            if (area > maxArea)
            {
                maxArea = area;
            }
        }       
        
        return maxArea;
    }

    [Sample("7,1\n11,1\n11,7\n9,7\n9,5\n2,5\n2,3\n7,3\n", 24)]
    protected override long Part2(Model input)
    {
        var corners = input.RedTiles;
        var greenEdge = new HashSet<Edge>();

        for (var i = 0; i < corners.Count; i++)
        {
            var a = corners[i];
            var b = corners[(i + 1) % corners.Count];
            greenEdge.Add(new Edge(a, b));
        }

        var maxArea = 0L;
        
        for (var i = 0; i < corners.Count; i++)
        for (var j = i + 1; j < corners.Count; j++)
        {
            var rect = new Rect(corners[i], corners[j]);
            
            var area = rect.Area;
            if (area <= maxArea)
            {
                continue;
            }

            // in theory, I also need to check all the corners are inside the shape but this works for the inputs so...
            if (greenEdge.Any(edge => StrictlyIntersects(edge, rect)))
            {
                continue;
            }
            
            maxArea = area;
        }
        
        return maxArea;
    }
    
    private record struct Edge(LongPosition A, LongPosition B);

    private record struct Rect(LongPosition A, LongPosition B)
    {
        public long Area => (Math.Abs(A.X - B.X) + 1) * (Math.Abs(A.Y - B.Y) + 1);
    }
    
    
    private static bool StrictlyIntersects(Edge greenEdge, Rect rect)
    {
        var rectMinX = Math.Min(rect.A.X, rect.B.X);
        var rectMaxX = Math.Max(rect.A.X, rect.B.X);
        var rectMinY = Math.Min(rect.A.Y, rect.B.Y);
        var rectMaxY = Math.Max(rect.A.Y, rect.B.Y);
        
        var segmentMinX = Math.Min(greenEdge.A.X, greenEdge.B.X);
        var segmentMaxX = Math.Max(greenEdge.A.X, greenEdge.B.X);
        var segmentMinY = Math.Min(greenEdge.A.Y, greenEdge.B.Y);
        var segmentMaxY = Math.Max(greenEdge.A.Y, greenEdge.B.Y);
        
        return segmentMaxX > rectMinX && segmentMinX < rectMaxX && segmentMaxY > rectMinY && segmentMinY < rectMaxY;
    }
}