namespace AdventOfCode2020;

public partial class Day20 : ParseDay<Day20.Model, Day20.TokenType, long, long>
{
    private const string Sample = "Tile 2311:\n..##.#..#.\n##..#.....\n#...##..#.\n####.#...#\n##.##.###.\n##...#.###\n.#.#.#..##\n..#....#..\n###...#.#.\n..###..###\n\nTile 1951:\n#.##...##.\n#.####...#\n.....#..##\n#...######\n.##.#....#\n.###.#####\n###.##.##.\n.###....#.\n..#.#..#.#\n#...##.#..\n\nTile 1171:\n####...##.\n#..##.#..#\n##.#..#.#.\n.###.####.\n..###.####\n.##....##.\n.#...####.\n#.##.####.\n####..#...\n.....##...\n\nTile 1427:\n###.##.#..\n.#..#.##..\n.#.##.#..#\n#.#.#.##.#\n....#...##\n...##..##.\n...#.#####\n.#.####.#.\n..#..###.#\n..##.#..#.\n\nTile 1489:\n##.#.#....\n..##...#..\n.##..##...\n..#...#...\n#####...#.\n#..#.#.#.#\n...#.#.#..\n##.#...##.\n..##.##.##\n###.##.#..\n\nTile 2473:\n#....####.\n#..#.##...\n#.##..#...\n######.#.#\n.#...#.#.#\n.#########\n.###.#..#.\n########.#\n##...##.#.\n..###.#.#.\n\nTile 2971:\n..#.#....#\n#...###...\n#.#.###...\n##.##..#..\n.#####..##\n.#..####.#\n#..#.#..#.\n..####.###\n..#.#.###.\n...#.#.#.#\n\nTile 2729:\n...#.#.#.#\n####.#....\n..#.#.....\n....#..#.#\n.##..##.#.\n.#.####...\n####.#.#..\n##.####...\n##..#.##..\n#.##...##.\n\nTile 3079:\n#.#.#####.\n.#..######\n..#.......\n######....\n####.#..#.\n.#...#.##.\n#.#####.##\n..#.###...\n..#.......\n..#.###...\n\n";
    
    private readonly Tile _monster = new (0, ImageLines.MustParse(StaticTokenizer, "..................#.\n#....##....##....###\n.#..#..#..#..#..#...\n"));
    private readonly IReadOnlyCollection<TransformedTile> _transformedMonsters;

    /*
     * ---0---
     * |      |
     * 3      1
     * |      |
     * ---2---|
     */
    
    public Day20(ITestOutputHelper output) : base(20, output)
    {
        _transformedMonsters = EnumerateTransformations(_monster).ToList();
    }
    
    [Sample(Sample, 20899048083289L)]
    protected override long Part1(Model input)
    {
        var image = new Aligner(input.Tiles).Align();
        var width = image.GetLength(0);
        var height = image.GetLength(1);
            
        var corners = new[]
        {
            image[0, 0],
            image[0, height - 1],
            image[width - 1, 0],
            image[width - 1, height - 1],
        };
            
        return corners.Select(x => x.Tile.Id).Aggregate((a, x) => a * x);
    }
    
    public record Tile(long Id, bool[,] Image);
    public record Model(IReadOnlyDictionary<long, Tile> Tiles);
    
    private static IEnumerable<TransformedTile> EnumerateTransformations(Tile tile) => 
        Enum.GetValues<Flip>().SelectMany(_ => Enum.GetValues<Rotate>(), (flip, rotate) => new TransformedTile(tile, flip, rotate));
    
    private class TransformedTile
    {
        private readonly bool[,] _transformed;

        public int YSize => _transformed.GetLength(0);
        public int XSize => _transformed.GetLength(1);
        
        public Tile Tile { get; }
        public Flip Flip { get; }
        public Rotate Rotate { get; }

        public TransformedTile(Tile tile, Flip flip, Rotate rotate)
        {
            Tile = tile;
            Flip = flip;
            Rotate = rotate;

            _transformed = BuildTransform(tile.Image, flip, rotate);
        }
        
        private static bool[,] BuildTransform(bool[,] image, Flip flip, Rotate rotate)
        {
            var (oySize, oxSize) = (image.GetLength(0), image.GetLength(1));
            var (ySize, xSize) = rotate is Rotate.CW90 or Rotate.CW270 ? (oxSize, oySize) : (oySize, oxSize);
            var transformed = new bool[ySize, xSize];

            for (var y = 0; y < ySize; y++)
            {
                for (var x = 0; x < xSize; x++)
                {
                    var (ry, rx) = rotate switch
                    {
                        Rotate.CW0 => (y, x),
                        Rotate.CW90 => (oySize-x-1, y),
                        Rotate.CW180 => (oySize-y-1, oxSize-x-1),
                        Rotate.CW270 => (x, oxSize-y-1),

                        _ => throw new ArgumentOutOfRangeException(nameof(rotate), rotate, null)
                    };
                    
                    var (fy, fx) = flip switch
                    {
                        Flip.None => (ry, rx),
                        Flip.FlipX => (oySize-ry-1, rx),
                        Flip.FlipY => (ry, oxSize-rx-1),
                        Flip.FlipXY => (oySize-ry-1, oxSize-rx-1),
                        
                        _ => throw new ArgumentOutOfRangeException(nameof(flip), flip, null)
                    };
                    
                    transformed[y, x] = image[fy, fx];
                }
            }

            return transformed;
        }

        public bool Get(in int y, in int x) => _transformed[y, x];

        public IReadOnlyList<bool> GetEdge(int index)
        {
            var isH = index is 0 or 2;
            var isM = index is 0 or 3;
            
            var edge = new bool[isH ? XSize : YSize];
            
            for (var i = 0; i < edge.Length; i++)
            {
                var (x, y) = isH ? (i, isM ? 0 : YSize - 1) : (isM ? 0 : XSize - 1, i);
                edge[i] = _transformed[y, x];
            }

            return edge;
        }
        
        public IEnumerable<IReadOnlyList<bool>> GetEdges()
        {
            var edges = new IReadOnlyList<bool>[4];

            for (var i = 0; i < edges.Length; i++)
            {
                edges[i] = GetEdge(i);
            }
        
            return edges;
        }
    }

    private enum Flip
    {
        None,
        FlipX,
        FlipY,
        FlipXY,
    }  
    private enum Rotate
    {
        CW0,
        CW90,
        CW180,
        CW270
    }
}
