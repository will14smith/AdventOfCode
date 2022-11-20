using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2016;

[Day]
public partial class Day01 : ParseDay<Day01.Model, int, int>
{
    private static readonly TextParser<bool> DirectionParser = Character.In('L', 'R').Select(x => x == 'L');
    private static readonly TextParser<Movement> MovementParser = DirectionParser.Then(Numerics.IntegerInt32).Select(x => new Movement(x.Item1, x.Item2));
    protected override TextParser<Model> Parser => MovementParser.ManyDelimitedBy(Span.EqualTo(", ")).Select(x => new Model(x));

    [Sample("R2, L3", 5)]
    [Sample("R2, R2, R2", 2)]
    [Sample("R5, L5, R5, R3", 12)]
    protected override int Part1(Model input)
    {
        var position = Position.Identity;
        var heading = new Position(0, -1);
        
        foreach (var movement in input.Movements)
        {
            heading = movement.TurnLeft ? heading.RotateCCW(90) : heading.RotateCW(90);
            position += heading * movement.Distance;
        }

        return position.BlockDistance();
    }
    
    [Sample("R8, R4, R4, R8", 4)]
    protected override int Part2(Model input)
    {
        var position = Position.Identity;
        var heading = new Position(0, -1);

        var visited = new HashSet<Position>();
        
        foreach (var movement in input.Movements)
        {
            heading = movement.TurnLeft ? heading.RotateCCW(90) : heading.RotateCW(90);

            for (int i = 0; i < movement.Distance; i++)
            {
                position += heading;
                if (!visited.Add(position))
                {
                    return position.BlockDistance();
                }
            }
        }

        throw new Exception("no solution");
    }
    

    public record Model(IReadOnlyList<Movement> Movements);
    public record Movement(bool TurnLeft, int Distance);
}
