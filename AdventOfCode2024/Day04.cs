namespace AdventOfCode2024;

[Day]
public partial class Day04 : Day<Day04.Model, int, int>
{
    public record Model(Grid<char> Grid);
    
    protected override Model Parse(string input) => new(GridParser.ParseChar(input, x => x));

    [Sample("MMMSXXMASM\nMSAMXMSMSA\nAMXSXMAAMM\nMSAMASMSMX\nXMASAMXAMM\nXXAMMXXAMA\nSMSMSASXSS\nSAXAMASAAA\nMAMMMXMMMM\nMXMXAXMASX", 18)]
    protected override int Part1(Model input)
    {
        var target = "XMAS";
        var directions = Position.Identity.StrictNeighbours().Select(x => Enumerable.Repeat(x, target.Length).Index().Select(x => x.Item * x.Index).ToArray()).ToArray();

        var count = 0;
        foreach (var position in input.Grid.Keys())
        {
            foreach (var direction in directions)
            {
                var found = true;
                for (var i = 0; i < direction.Length; i++)
                {
                    var position1 = position + direction[i];
                    if (!input.Grid.IsValid(position1))
                    {
                        found = false;
                        break;
                    }

                    if (input.Grid[position1] != target[i])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    count++;
                }
            }
        }
        return count;
    }

    [Sample(
        "MMMSXXMASM\nMSAMXMSMSA\nAMXSXMAAMM\nMSAMASMSMX\nXMASAMXAMM\nXXAMMXXAMA\nSMSMSASXSS\nSAXAMASAAA\nMAMMMXMMMM\nMXMXAXMASX",
        9)]
    protected override int Part2(Model input)
    {
        var count = 0;
        foreach (var position in input.Grid.Keys())
        {
            if (input.Grid[position] != 'A')
            {
                continue;
            }

            if (!input.Grid.IsValid(position - new Position(1, 1)))
            {
                continue;
            }
            if (!input.Grid.IsValid(position + new Position(1, 1)))
            {
                continue;
            }

            var positives = input.Grid[position - new Position(1 , 1)].ToString() + input.Grid[position + new Position(1 , 1)];
            var negatives = input.Grid[position - new Position(1 , -1)].ToString() + input.Grid[position + new Position(1 , -1)];

            if (positives is not ("MS" or "SM")) continue;
            if (negatives is not ("MS" or "SM")) continue;

            count++;
        }

        return count;
    }
}