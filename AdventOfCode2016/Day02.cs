using System.Text;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2016;

[Day]
public partial class Day02 : ParseDay<Day02.Model, string, string>
{
    private static readonly TextParser<Direction> DirectionParser = Character.In('U', 'D', 'L', 'R').Select(x => x switch
    {
        'U' => Direction.Up,
        'D' => Direction.Down,
        'L' => Direction.Left,
        'R' => Direction.Right,
    });
    private static readonly TextParser<Line> LineParser = DirectionParser.AtLeastOnce().Select(x => new Line(x));
    private static readonly TextParser<Model> ModelParser = LineParser.ManyDelimitedBy(SuperpowerExtensions.NewLine).Select(x => new Model(x)); 

    protected override TextParser<Model> Parser => ModelParser;

    [Sample("ULL\nRRDDD\nLURDL\nUUUUD", "1985")]
    protected override string Part1(Model input)
    {
        const string keypad = 
            "123" + 
            "456" + 
            "789";
        
        return FindCode(input, keypad, 3);
    }

    [Sample("ULL\nRRDDD\nLURDL\nUUUUD", "5DB3")]
    protected override string Part2(Model input)
    {
        const string keypad = 
            "  1  " + 
            " 234 " + 
            "56789" + 
            " ABC " + 
            "  D  ";
        
        return FindCode(input, keypad, 5);
    }

    private static string FindCode(Model input, string keypad, int width)
    {
        var digits = new StringBuilder();

        var grid = new Grid<char>(keypad.ToCharArray(), width);
        var current = grid.Keys().First(k => grid[k] == '5');
        
        foreach (var line in input.Lines)
        {
            foreach (var direction in line.Directions)
            {
                var next = current + direction switch
                {
                    Direction.Up => new Position(0, -1),
                    Direction.Down => new Position(0, 1),
                    Direction.Left => new Position(-1, 0),
                    Direction.Right => new Position(1, 0),
                };

                if (!grid.IsValid(next) || grid[next] == ' ')
                {
                    continue;
                }

                current = next;
            }

            digits.Append(grid[current]);
        }

        return digits.ToString();
    }

    public record Model(IReadOnlyList<Line> Lines);
    public record Line(IReadOnlyList<Direction> Directions);
    public enum Direction
    {
        Up,
        Down,
        Left, 
        Right,
    }
}
