using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2019;

[Day]
public partial class Day03 : ParseDay<Day03.Model, int, int>
{
    private static readonly TextParser<Direction> DirectionParser = 
            Character.EqualTo('U').Select(_ => Direction.Up)
        .Or(Character.EqualTo('L').Select(_ => Direction.Left))
        .Or(Character.EqualTo('D').Select(_ => Direction.Down))
        .Or(Character.EqualTo('R').Select(_ => Direction.Right));
    private static readonly TextParser<Instruction> InstructionParser = DirectionParser.Then(Numerics.IntegerInt32).Select(x => new Instruction(x.Item1, x.Item2));
    private static readonly TextParser<Instruction[]> InstructionsParser = InstructionParser.ManyDelimitedBy(Character.EqualTo(','));
    
    protected override TextParser<Model> Parser { get; } = InstructionsParser.ThenIgnore(Character.EqualTo('\n')).Then(InstructionsParser).Select(x => new Model(x.Item1, x.Item2));

    [Sample("R8,U5,L5,D3\nU7,R6,D4,L4", 6)]
    [Sample("R75,D30,R83,U83,L12,D49,R71,U7,L72\nU62,R66,U55,R34,D71,R55,D58,R83", 159)]
    [Sample("R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51\nU98,R91,D20,R16,D67,R40,U7,R15,U6,R7", 135)]
    protected override int Part1(Model input)
    {
        var seen = new HashSet<Position>();

        var position = Position.Identity;
        foreach (var instruction in input.First)
        {
            var heading = GetHeading(instruction.Direction);

            for (var i = 0; i < instruction.Distance; i++)
            {
                position += heading;
                seen.Add(position);
            }
        }

        var closest = int.MaxValue;
        
        position = Position.Identity;
        foreach (var instruction in input.Second)
        {
            var heading = GetHeading(instruction.Direction);

            for (var i = 0; i < instruction.Distance; i++)
            {
                position += heading;
                if (seen.Contains(position))
                {
                    closest = Math.Min(closest, position.BlockDistance());
                }
            }
        }

        return closest;
    }

    private static Position GetHeading(Direction direction) =>
        direction switch
        {
            Direction.Up => new Position(0, 1),
            Direction.Left => new Position(-1, 0),
            Direction.Down => new Position(0, -1),
            Direction.Right => new Position(1, 0),
            _ => throw new ArgumentOutOfRangeException()
        };

    [Sample("R8,U5,L5,D3\nU7,R6,D4,L4", 30)]
    [Sample("R75,D30,R83,U83,L12,D49,R71,U7,L72\nU62,R66,U55,R34,D71,R55,D58,R83", 610)]
    [Sample("R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51\nU98,R91,D20,R16,D67,R40,U7,R15,U6,R7", 410)]
    protected override int Part2(Model input)
    {
        var seen = new Dictionary<Position, int>();

        var position = Position.Identity;
        var steps = 0;
        foreach (var instruction in input.First)
        {
            var heading = GetHeading(instruction.Direction);

            for (var i = 0; i < instruction.Distance; i++)
            {
                position += heading;
                steps++;

                seen.TryAdd(position, steps);
            }
        }

        var closest = int.MaxValue;
        
        position = Position.Identity;
        steps = 0;
        foreach (var instruction in input.Second)
        {
            var heading = GetHeading(instruction.Direction);

            for (var i = 0; i < instruction.Distance; i++)
            {
                position += heading;
                steps++;
                
                if (seen.TryGetValue(position, out var firstSteps))
                {
                    closest = Math.Min(closest, firstSteps + steps);
                }
            }
        }

        return closest;
    }

    public record Model(IReadOnlyList<Instruction> First, IReadOnlyList<Instruction> Second);
    public record Instruction(Direction Direction, int Distance);
    public enum Direction
    {
        Up,
        Left,
        Down,
        Right,
    }
}
