namespace AdventOfCode2023;

[Day]
public partial class Day03 : Day<Day03.Model, int, int>
{
    protected override Model Parse(string input)
    {
        var numbers = new Dictionary<Position, (Position Start, int Value)>();
        var symbols = new Dictionary<Position, char>();
        
        var lines = input.Split('\n');
        for (var y = 0; y < lines.Length; y++)
        {
            ParseLine(y, lines[y]);
        }

        return new Model(numbers, symbols);

        void ParseLine(int y, string line)
        {
            for (var x = 0; x < line.Length; x++)
            {
                if (char.IsDigit(line[x]))
                {
                    ParseNumber(ref x, y, line);
                }
                else if (line[x] != '.')
                {
                    symbols[new Position(x, y)] = line[x];
                }
            }
        }

        void ParseNumber(ref int x, int y, string line)
        {
            var startX = x;
            var value = line[x] - '0';
                    
            while (x + 1 < line.Length && char.IsDigit(line[x + 1]))
            {
                value = value * 10 + (line[++x] - '0');
            }

            var start = new Position(startX, y);

            for (var nx = startX; nx <= x; nx++)
            {
                numbers.Add(new Position(nx, y), (start, value));
            }
        }
    }

    [Sample("467..114..\n...*......\n..35..633.\n......#...\n617*......\n.....+.58.\n..592.....\n......755.\n...$.*....\n.664.598..", 4361)]
    protected override int Part1(Model input) =>
        input.Symbols.Keys
            .SelectMany(symbol => GetNeighbouringNumbers(input.Numbers, symbol))
            .Distinct()
            .Sum(numberAtPosition => numberAtPosition.Value);

    [Sample("467..114..\n...*......\n..35..633.\n......#...\n617*......\n.....+.58.\n..592.....\n......755.\n...$.*....\n.664.598..", 467835)]
    protected override int Part2(Model input) =>
        input.Symbols
            .Where(x => x.Value == '*')
            .Select(gear => GetNeighbouringNumbers(input.Numbers, gear.Key).ToList())
            .Where(neighbours => neighbours.Count == 2)
            .Sum(neighbours => neighbours[0].Value * neighbours[1].Value);

    private static IEnumerable<(Position Start, int Value)> GetNeighbouringNumbers(IReadOnlyDictionary<Position, (Position Start, int Value)> numbers, Position position) =>
        position.StrictNeighbours()
            .Where(numbers.ContainsKey)
            .Select(x => numbers[x])
            .Distinct();

    public record Model(IReadOnlyDictionary<Position, (Position Start, int Value)> Numbers, IReadOnlyDictionary<Position, char> Symbols);
}