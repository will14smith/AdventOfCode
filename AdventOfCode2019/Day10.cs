namespace AdventOfCode2019;

[Day]
public partial class Day10 : Day<Day10.Model, int, int>
{
    protected override Model Parse(string input) => new (GridParser.ParseBool(input, '#'));

    [Sample(".#..#\n.....\n#####\n....#\n...##", 8)]
    [Sample("......#.#.\n#..#.#....\n..#######.\n.#.#.###..\n.#..#.....\n..#....#.#\n#..#....#.\n.##.#..###\n##...#..#.\n.#....####", 33)]
    protected override int Part1(Model input)
    {
        return SolvePart1(input).Item2.Count;
    }
    
    [Sample(".#..##.###...#######\n##.############..##.\n.#.######.########.#\n.###.#######.####.#.\n#####.##.#.##.###.##\n..#####..#.#########\n####################\n#.####....###.#.#.##\n##.#################\n#####.##.###..####..\n..######..##.#######\n####.##.####...##..#\n.#####..#.######.###\n##...#.##########...\n#.##########.#######\n.####.#.###.###.#.##\n....##.##.###..#####\n.#.#.###########.###\n#.#.#.#####.####.###\n###.##.####.##.#..##", 802)]
    protected override int Part2(Model input)
    {
        var (position, deltas) = SolvePart1(input);

        var orderedDeltas = deltas
            .Select(delta => (delta, Math.Atan2(delta.Y, delta.X) * 180 / Math.PI + 90))
            .Select(x => x.Item2 < 0 ? (x.delta, x.Item2 + 360) : x)
            .OrderBy(x => x.Item2)
            .Select(x => x.delta).ToList();

        var destroyed = 0;
        
        var offset = 0;
        while (input.Map.Count(x => x) > 1)
        {
            var delta = orderedDeltas[offset++ % orderedDeltas.Count];
            var multiple = 1;
            while (true)
            {
                var target = position + delta * multiple++;
                if (!input.Map.IsValid(target)) break;

                if (!input.Map[target]) continue;

                if (++destroyed == 200)
                {
                    return target.X * 100 + target.Y;
                }
                
                input.Map[target] = false;
                break;
            }
        }
        
        throw new Exception("no.");
    }
    
    private static (Position, HashSet<Position>) SolvePart1(Model input)
    {
        var max = (Position.Identity, new HashSet<Position>());

        foreach (var position in input.Map.Keys())
        {
            if (!input.Map[position]) continue;

            var visible = new HashSet<Position>();

            foreach (var position2 in input.Map.Keys())
            {
                if (position == position2) continue;
                if (!input.Map[position2]) continue;

                var delta = position2 - position;
                var gcd = NumberExtensions.GreatestCommonDenominator(delta.X, delta.Y);
                var reducedDelta = delta / Math.Abs(gcd);
                
                visible.Add(reducedDelta);
            }

            if (max.Item2.Count < visible.Count)
            {
                max = (position, visible);
            }
        }

        return max;
    }

    public record Model(Grid<bool> Map);
}
