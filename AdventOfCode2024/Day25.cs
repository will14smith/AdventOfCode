namespace AdventOfCode2024;

[Day]
public partial class Day25 : Day<Day25.Model, int, int>
{
    public record Model(IReadOnlyList<Grid<bool>> Specifications);
    
    protected override Model Parse(string input) => new(input.Split("\n\n").Select(x => GridParser.ParseBool(x, '#')).ToList());

    [Sample("#####\n.####\n.####\n.####\n.#.#.\n.#...\n.....\n\n#####\n##.##\n.#.##\n...##\n...#.\n...#.\n.....\n\n.....\n#....\n#....\n#...#\n#.#.#\n#.###\n#####\n\n.....\n.....\n#.#..\n###..\n###.#\n###.#\n#####\n\n.....\n.....\n.....\n#....\n#.#..\n#.#.#\n#####", 3)]
    protected override int Part1(Model input)
    {
        var locks = input.Specifications.Where(IsLock).Select(DecodeLock).ToList();
        var keys = input.Specifications.Where(IsKey).Select(DecodeKey).ToList();

        return locks.Sum(l => keys.Count(k => !Overlaps(l, k)));
    }

    private static bool Overlaps(IReadOnlyList<int> lockPins, IReadOnlyList<int> keyPins)
    {
        return lockPins.Zip(keyPins).Any(x => x.First + x.Second > 5);
    }

    private static bool IsLock(Grid<bool> specification)
    {
        for (var i = 0; i < specification.Width; i++)
        {
            if (!specification[i, 0]) return false;
            if (specification[i, specification.Height - 1]) return false;
        }
        
        return true;
    }

    private static bool IsKey(Grid<bool> specification)
    {
        for (var i = 0; i < specification.Width; i++)
        {
            if (specification[i, 0]) return false;
            if (!specification[i, specification.Height - 1]) return false;
        }
        
        return true;
    }

    private static IReadOnlyList<int> DecodeLock(Grid<bool> specification)
    {
        var pins = new int[specification.Width];
        
        for (var i = 0; i < specification.Width; i++)
        {
            for (var j = 0; j < specification.Height; j++)
            {
                if (!specification[i, j])
                {
                    pins[i] = j - 1;
                    break;
                }
            }
        }

        return pins;
    }

    private static IReadOnlyList<int> DecodeKey(Grid<bool> specification)
    {
        var pins = new int[specification.Width];
        
        for (var i = 0; i < specification.Width; i++)
        {
            for (var j = 0; j < specification.Height; j++)
            {
                if (specification[i, j])
                {
                    pins[i] = specification.Height - j - 1;
                    break;
                }
            }
        }

        return pins;
    }

    protected override int Part2(Model input) => 2024;
}