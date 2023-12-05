namespace AdventOfCode2017;

[Day]
public partial class Day22 : Day<Day22.Model, int, int>
{
    protected override Model Parse(string input) => new(GridParser.ParseBool(input, '#'));

    [Sample("..#\n#..\n...", 5587)]
    protected override int Part1(Model input)
    {
        var cells = input.Grid.Keys().ToDictionary(x => x, x => input.Grid[x]);

        var position = new Position(input.Grid.Width / 2, input.Grid.Height / 2);
        var heading = new Position(0, -1);
        
        var infected = 0;
        for (var step = 0; step < 10_000; step++)
        {
            var currentNode = cells.TryGetValue(position, out var temp) && temp;
            if (currentNode)
            {
                heading = heading.RotateCCW(90);
            }
            else
            {
                heading = heading.RotateCW(90);
                infected++;
            }
            
            cells[position] = !currentNode;
            position += heading;
        }

        return infected;
    }
    
    [Sample("..#\n#..\n...", 2511944)]
    protected override int Part2(Model input)
    {
        var cells = input.Grid.Keys().ToDictionary(x => x, x => input.Grid[x] ? State.Infected : State.Clean);

        var position = new Position(input.Grid.Width / 2, input.Grid.Height / 2);
        var heading = new Position(0, -1);
        
        var infected = 0;
        for (var step = 0; step < 10_000_000; step++)
        {
            State nextState;
            
            var currentNode = cells.TryGetValue(position, out var temp) ? temp : State.Clean;
            switch (currentNode)
            {
                case State.Clean:
                    heading = heading.RotateCW(90);
                    nextState = State.Weakened;
                    break;
                case State.Weakened:
                    nextState = State.Infected;
                    infected++;
                    break;
                case State.Infected:
                    heading = heading.RotateCCW(90);
                    nextState = State.Flagged;
                    break;
                case State.Flagged:
                    heading = -heading;
                    nextState = State.Clean;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            cells[position] = nextState;
            position += heading;
        }

        return infected;
    }

    public record Model(Grid<bool> Grid);

    public enum State
    {
        Clean,
        Weakened,
        Infected,
        Flagged,
    }
}
