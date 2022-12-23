namespace AdventOfCode2022;

[Day]
public partial class Day23 : Day<Day23.Model, int, int>
{
    private const string Sample1 = ".....\n..##.\n..#..\n.....\n..##.\n.....";
    private const string Sample2 = "..............\n..............\n.......#......\n.....###.#....\n...#...#.#....\n....#...##....\n...#.###......\n...##.#.##....\n....#..#......\n..............\n..............\n..............";
    
    protected override Model Parse(string input) => new(GridParser.ParseBool(input, '#'));

    // [Sample(Sample1, 25)]
    [Sample(Sample2, 110)]
    protected override int Part1(Model input)
    {
        var elves = input.ToSparse();

        for (var i = 0; i < 10; i++)
        {
            (elves, _) = DoRound(elves, i);
        }

        var minX = elves.Min(x => x.X);
        var minY = elves.Min(x => x.Y);
        var maxX = elves.Max(x => x.X);
        var maxY = elves.Max(x => x.Y);
        
        return (maxX - minX + 1) * (maxY - minY + 1) - elves.Count;
    }
    
    [Sample(Sample2, 20)]
    protected override int Part2(Model input)
    {
        var elves = input.ToSparse();

        var rounds = 0;
        while (true)
        {
            (elves, var changed) = DoRound(elves, rounds++);
            if (!changed)
            {
                break;
            }
        }

        return rounds;
    }

    private (IReadOnlySet<Position>, bool) DoRound(IReadOnlySet<Position> elves, int round)
    {
        // NW|N|NE|E|SE|S|SW|W
        const int NMask = 0b11100000;
        const int EMask = 0b00111000;
        const int SMask = 0b00001110;
        const int WMask = 0b10000011;

        var deltas = new[]
        {
            new Position(-1, 0),
            new Position(-1, 1),
            new Position(0, 1),
            new Position(1, 1),
            new Position(1, 0),
            new Position(1, -1),
            new Position(0, -1),
            new Position(-1, -1),
        };

        var moves = new[]
        {
            (NMask, new Position(0, -1)),
            (SMask, new Position(0, 1)),
            (WMask, new Position(-1, 0)),
            (EMask, new Position(1, 0)),
        };

        var candidates = new List<(Position Old, Position New)>(elves.Count);

        foreach (var elf in elves)
        {
            var invNeighbours = 0;

            for (var i = 0; i < deltas.Length; i++)
            {
                invNeighbours |= (elves.Contains(elf + deltas[i]) ? 0 : 1) << i;
            }

            if (invNeighbours == 255)
            {
                candidates.Add((elf, elf));
            }
            else
            {
                Position? candidate = null;
                for (var i = 0; i < moves.Length; i++)
                {
                    var move = moves[(i + round) % moves.Length];
                    if ((invNeighbours & move.Item1) == move.Item1)
                    {
                        candidate = elf + move.Item2;
                        break;
                    }
                }
                
                candidates.Add((elf, candidate ?? elf));
            }
        }
        
        var newElves = candidates.GroupBy(x => x.New).SelectMany(x => x.Count() == 1 ? new[] { x.Key } : x.Select(y => y.Old)).ToHashSet();
        var changed = !newElves.SetEquals(elves);

        return (newElves, changed);
    }

    public record Model(Grid<bool> Map)
    {
        public IReadOnlySet<Position> ToSparse() => Map.Keys().Where(x => Map[x]).ToHashSet();
    }
}
