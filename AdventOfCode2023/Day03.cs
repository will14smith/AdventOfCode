using System.Text;

namespace AdventOfCode2023;

[Day]
public partial class Day03 : Day<Day03.Model, int, int>
{
    protected override Model Parse(string input) => new(GridParser.ParseChar(input, x => x));

    [Sample(
        "467..114..\n...*......\n..35..633.\n......#...\n617*......\n.....+.58.\n..592.....\n......755.\n...$.*....\n.664.598..", 4361)]
    protected override int Part1(Model input)
    {
        var symbols = new List<Position>();
        foreach (var position in input.Grid.Keys())
        {
            var value = input.Grid[position];
            if (value != '.' && !char.IsDigit(value))
            {
                symbols.Add(position);
            }
        }

        var numbers = new Dictionary<Position, int>();
        foreach (var symbol in symbols)
        {
            foreach (var neighbour in symbol.StrictNeighbours())
            {
                if (!char.IsDigit(input.Grid[neighbour]))
                {
                    continue;
                }
                
                var (start, value) = FindNumber(input.Grid, neighbour);
                numbers[start] = value;
            }
        }

        return numbers.Sum(x => x.Value);
    }
    
    [Sample("467..114..\n...*......\n..35..633.\n......#...\n617*......\n.....+.58.\n..592.....\n......755.\n...$.*....\n.664.598..", 467835)]
    protected override int Part2(Model input)
    {
        var symbols = new HashSet<Position>();

        foreach (var position in input.Grid.Keys())
        {
            var value = input.Grid[position];
            if (value == '*')
            {
                symbols.Add(position);
            }
        }

        var numbers = new Dictionary<Position, Dictionary<Position, int>>();
        foreach (var symbol in symbols)
        {
            foreach (var neighbour in symbol.StrictNeighbours())
            {
                if (!char.IsDigit(input.Grid[neighbour]))
                {
                    continue;
                }
                
                var (start, value) = FindNumber(input.Grid, neighbour);

                if (!numbers.TryGetValue(symbol, out var symbolNumbers))
                {
                    symbolNumbers = new Dictionary<Position, int>();
                    numbers[symbol] = symbolNumbers;
                }
                    
                symbolNumbers[start] = value;
            }
        }

        return numbers.Where(x => x.Value.Count == 2).Sum(x => x.Value.First().Value * x.Value.Last().Value);
    }
    
    private static (Position Start, int Value) FindNumber(Grid<char> grid, Position position)
    {
        var start = FindNumberStart(grid, position);
        var end = FindNumberEnd(grid, position);

        var value = ParseNumber(grid, position, start, end);

        return (position with { X = start }, value);
    }
    
    private static int FindNumberStart(Grid<char> grid, Position position)
    {
        var start = position.X;
        for (var x = position.X - 1; x >= 0; x--)
        {
            if (char.IsDigit(grid[x, position.Y]))
            {
                start = x;
            }
            else
            {
                break;
            }
        }

        return start;
    }
    
    private static int FindNumberEnd(Grid<char> grid, Position position)
    {
        var end = position.X;
        for (var x = position.X + 1; x < grid.Width; x++)
        {
            if (char.IsDigit(grid[x, position.Y]))
            {
                end = x;
            }
            else
            {
                break;
            }
        }

        return end;
    }
    
    private static int ParseNumber(Grid<char> grid, Position position, int start, int end)
    {
        var str = new StringBuilder();
        for (var x = start; x <= end; x++)
        {
            str.Append(grid[x, position.Y]);
        }
        return int.Parse(str.ToString());
    }


    public record Model(Grid<char> Grid);
}