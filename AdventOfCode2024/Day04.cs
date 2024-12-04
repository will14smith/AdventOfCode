namespace AdventOfCode2024;

[Day]
public partial class Day04 : Day<Day04.Model, int, int>
{
    public record Model(Grid<char> Grid);
    
    protected override Model Parse(string input) => new(GridParser.ParseChar(input, x => x));

    [Sample("MMMSXXMASM\nMSAMXMSMSA\nAMXSXMAAMM\nMSAMASMSMX\nXMASAMXAMM\nXXAMMXXAMA\nSMSMSASXSS\nSAXAMASAAA\nMAMMMXMMMM\nMXMXAXMASX", 18)]
    protected override int Part1(Model input)
    {
        const string target = "XMAS";
        var directions = Position.Identity.StrictNeighbours().Select(x => Enumerable.Repeat(x, target.Length).Index().Select(x => x.Item * x.Index).ToArray()).ToArray();

        return input.Grid.Keys().Sum(position => directions.Count(direction => CheckWordInDirection(input, direction, position, target)));
    }

    private static bool CheckWordInDirection(Model input, Position[] directions, Position start, string target)
    {
        for (var i = 0; i < directions.Length; i++)
        {
            var position = start + directions[i];
            if (!input.Grid.IsValid(position) || input.Grid[position] != target[i])
            {
                return false;
            }
        }

        return true;
    }

    [Sample("MMMSXXMASM\nMSAMXMSMSA\nAMXSXMAAMM\nMSAMASMSMX\nXMASAMXAMM\nXXAMMXXAMA\nSMSMSASXSS\nSAXAMASAAA\nMAMMMXMMMM\nMXMXAXMASX", 9)]
    protected override int Part2(Model input) => input.Grid.Keys().Count(position => IsCrossOfMas(input, position));

    private static bool IsCrossOfMas(Model input, Position position)
    {
        if (input.Grid[position] != 'A')
        {
            return false;
        }

        if (!input.Grid.IsValid(position - new Position(1, 1)))
        {
            return false;
        }
        if (!input.Grid.IsValid(position + new Position(1, 1)))
        {
            return false;
        }

        var positives = (input.Grid[position - new Position(1 , 1)], input.Grid[position + new Position(1 , 1)]);
        var negatives = (input.Grid[position - new Position(1 , -1)], input.Grid[position + new Position(1 , -1)]);

        return positives is ('M', 'S') or ('S', 'M') && negatives is ('M', 'S') or ('S', 'M');
    }
}