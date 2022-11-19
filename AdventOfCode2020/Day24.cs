namespace AdventOfCode2020;

[Day]
public partial class Day24 : ParseDay<Day24.Direction[][], Day24.TokenType, int, int>
{
    private const string Sample = "sesenwnenenewseeswwswswwnenewsewsw\nneeenesenwnwwswnenewnwwsewnenwseswesw\nseswneswswsenwwnwse\nnwnwneseeswswnenewneswwnewseswneseene\nswweswneswnenwsewnwneneseenw\neesenwseswswnenwswnwnwsewwnwsene\nsewnenenenesenwsewnenwwwse\nwenwwweseeeweswwwnwwe\nwsweesenenewnwwnwsenewsenwwsesesenwne\nneeswseenwwswnwswswnw\nnenwswwsewswnenenewsenwsenwnesesenew\nenewnwewneswsewnwswenweswnenwsenwsw\nsweneswneswneneenwnewenewwneswswnese\nswwesenesewenwneswnwwneseswwne\nenesenwswwswneneswsenwnewswseenwsese\nwnwnesenesenenwwnenwsewesewsesesew\nnenewswnwewswnenesenwnesewesw\neneswnwswnwsenenwnwnwwseeswneewsenese\nneswnwewnwnwseenwseesewsenwsweewe\nwseweeenwnesenwwwswnew\n";
    
    private static readonly ReadOnlyMemory<(int, int)> Offsets = new[] { (-1, 0), (-1, 1), (0, 1), (1, 0), (1, -1), (0, -1) };
    
    [Sample(Sample, 10)]
    protected override int Part1(Direction[][] input) => CalculateTiles(input).Count;

    [Sample(Sample, 2208)]
    protected override int Part2(Direction[][] input)
    {
        var tiles = CalculateTiles(input);
            
        for (var day = 0; day < 100; day++)
        {
            var neighbours = new Dictionary<(int, int), int>();
            foreach (var tile in tiles)
            {
                for (var index = 0; index < Offsets.Span.Length; index++)
                {
                    var offset = Offsets.Span[index];
                    var neighbour = Add(tile, offset);

                    if (!neighbours.TryAdd(neighbour, 1))
                    {
                        neighbours[neighbour] += 1;
                    }
                }
            }

            var next = new HashSet<(int, int)>();
            foreach (var (neighbour, count) in neighbours)
            {
                if (count == 2 || count == 1 && tiles.Contains(neighbour))
                {
                    next.Add(neighbour);
                }
            }                

            tiles = next;
        }

        return tiles.Count;
    }
    
    private static HashSet<(int, int)> CalculateTiles(Direction[][] input)
    {
        var tiles = new HashSet<(int, int)>();

        foreach (var line in input)
        {
            var loc = Calculate(line);
            if (tiles.Contains(loc))
            {
                tiles.Remove(loc);
            }
            else
            {
                tiles.Add(loc);
            }
        }

        return tiles;
    }

    private static (int, int) Calculate(IEnumerable<Direction> line) => line.Aggregate((0, 0), (current, direction) => Add(current, Offsets.Span[(int) direction]));
    private static (int, int) Add((int, int) a, (int, int) b) => (a.Item1 + b.Item1, a.Item2 + b.Item2);

    public enum Direction
    {
        East,
        SouthEast,
        SouthWest,
        West,
        NorthWest,
        NorthEast,
    }
}
