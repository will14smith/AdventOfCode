namespace AdventOfCode2017;

[Day]
public partial class Day19 : Day<Day19.Model, string, int>
{
    protected override Model Parse(string input)
    {
        var grid = new Dictionary<Position, Cell>();
        var letters = new Dictionary<Position, char>();

        var y = 0;
        foreach (var line in input.Split('\n'))
        {
            var x = 0;
            
            foreach (var c in line)
            {
                switch (c)
                {
                    case ' ': break;
                    case '|': grid[new Position(x, y)] = Cell.Vertical; break;
                    case '-': grid[new Position(x, y)] = Cell.Horizontal; break;
                    case '+': grid[new Position(x, y)] = Cell.Corner; break;
                    default:
                        if (char.IsLetter(c))
                        {
                            grid[new Position(x, y)] = Cell.Letter;
                            letters[new Position(x, y)] = c;
                        }
                        break;
                }

                x++;
            }

            y++;
        }
        
        return new Model(grid, letters);
    }

    [Sample("     |          \n     |  +--+    \n     A  |  C    \n F---|----E|--+ \n     |  |  |  D \n     +B-+  +--+", "ABCDEF")]
    protected override string Part1(Model input) => Solve(input).VisitedLetters;

    [Sample("     |          \n     |  +--+    \n     A  |  C    \n F---|----E|--+ \n     |  |  |  D \n     +B-+  +--+", 38)]
    protected override int Part2(Model input) => Solve(input).TotalSteps;

    private (string VisitedLetters, int TotalSteps) Solve(Model input)
    {
        var position = input.Grid.Keys.First(x => x.Y == 0);
        var heading = new Position(0, 1);
        
        var count = 1;
        var seen = new List<char>();
        
        while (true)
        {
            var next = position + heading;
            if (!input.Grid.TryGetValue(next, out var nextCell))
            {
                // assume letters aren't on corners...
                break;
            }

            switch (nextCell)
            {
                case Cell.Corner:
                    var newHeading = heading.X == 0 ? new Position(1, 0) : new Position(0, 1);
                    var cornerNext = next + newHeading;
                    heading = input.Grid.ContainsKey(cornerNext) ? newHeading : -newHeading;
                    break;
                
                case Cell.Letter:
                    seen.Add(input.Letters[next]);
                    break;
            }

            position = next;
            count++;
        }

        return (new string(seen.ToArray()), count);
    }

    public record Model(IReadOnlyDictionary<Position, Cell> Grid, IReadOnlyDictionary<Position, char> Letters);

    public enum Cell
    {
        Vertical,
        Horizontal,
        Corner,
        Letter,
    }
}
