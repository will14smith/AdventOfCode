namespace AdventOfCode2024;

[Day]
public partial class Day08 : Day<Day08.Model, int, int>
{
    private const string Sample1 = "..........\n..........\n..........\n....a.....\n..........\n.....a....\n..........\n..........\n..........\n..........";
    private const string Sample2 = "..........\n..........\n..........\n....a.....\n........a.\n.....a....\n..........\n..........\n..........\n..........";
    private const string Sample3 = "............\n........0...\n.....0......\n.......0....\n....0.......\n......A.....\n............\n............\n........A...\n.........A..\n............\n............";
    
    public record Model(Grid<char> Grid);
    
    protected override Model Parse(string input) => new(GridParser.ParseChar(input, x => x));
    
    [Sample(Sample1, 2)]
    [Sample(Sample2, 4)]
    [Sample(Sample3, 14)]
    protected override int Part1(Model input)
    {
        var antinodes = new HashSet<Position>();

        foreach (var antennaGroup in GroupAntennas(input))
        {
            var combinations = Combinations.Get(antennaGroup.ToList(), 2);

            foreach (var combination in combinations)
            {
                var a = combination.Span[0];
                var b = combination.Span[1];

                var diff = a - b;

                antinodes.Add(a + diff);
                antinodes.Add(b - diff);
            }
        }

        return antinodes.Count(x => input.Grid.IsValid(x));
    }
    
    [Sample(Sample3, 34)]
    protected override int Part2(Model input)
    {
        var antinodes = new HashSet<Position>();
        foreach (var antennaGroup in GroupAntennas(input))
        {
            var combinations = Combinations.Get(antennaGroup.ToList(), 2);

            foreach (var combination in combinations)
            {
                var a = combination.Span[0];
                var b = combination.Span[1];

                // TODO diff might need reduced by GCD e.g. if the diff is (2,2) then there should be an antinode at (1,1) away
                var diff = a - b;
                
                while (input.Grid.IsValid(a))
                {
                    antinodes.Add(a);
                    a += diff;
                }
                while (input.Grid.IsValid(b))
                {
                    antinodes.Add(b);
                    b -= diff;
                }
            }
        }
        
        return antinodes.Count;
    }
    
    private static ILookup<char, Position> GroupAntennas(Model input) => input.Grid.Keys().Where(x => input.Grid[x] != '.').ToLookup(x => input.Grid[x], x => x);
}