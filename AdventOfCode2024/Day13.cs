using Microsoft.Z3;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2024;

[Day]
public partial class Day13 : ParseDay<Day13.Model, long, long>
{
    public record Model(IReadOnlyList<Machine> Machines);
    public record Machine(Position A, Position B, Position Prize);

    private static readonly TextParser<Position> Button = Span.Regex("Button .: X+").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(", Y+")).Then(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo("\n")).Select(x => new Position(x.Item1, x.Item2));
    private static readonly TextParser<Position> Prize = Span.Regex("Prize: X=").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(", Y=")).Then(Numerics.IntegerInt32).Select(x => new Position(x.Item1, x.Item2));
    private static readonly TextParser<Machine> MachineParser = Button.Then(Button).Then(Prize).Select(x => new Machine(x.Item1.Item1, x.Item1.Item2, x.Item2));
    protected override TextParser<Model> Parser { get; } = MachineParser.ManyDelimitedBy(Span.EqualTo("\n\n")).Select(m => new Model(m));

    [Sample("Button A: X+94, Y+34\nButton B: X+22, Y+67\nPrize: X=8400, Y=5400\n\nButton A: X+26, Y+66\nButton B: X+67, Y+21\nPrize: X=12748, Y=12176\n\nButton A: X+17, Y+86\nButton B: X+84, Y+37\nPrize: X=7870, Y=6450\n\nButton A: X+69, Y+23\nButton B: X+27, Y+71\nPrize: X=18641, Y=10279", 480L)]
    protected override long Part1(Model input) => input.Machines.Sum(x => SolveCheapest(x, 0));
    protected override long Part2(Model input) => input.Machines.Sum(x => SolveCheapest(x, 10000000000000));

    private long SolveCheapest(Machine machine, long offset)
    {
        // a*Ax + b*Bx = Px
        // a*Ay + b*By = Py
        
        // By(a*Ax + b*Bx) = By*Px
        // Bx(a*Ay + b*By) = Bx*Py
        
        // a*Ax*By + b*Bx*By = By*Px
        // a*Ay*Bx + b*Bx*By = Bx*Py

        // a*Ax*By + b*Bx*By - (a*Ay*Bx + b*Bx*By) = By*Px - Bx*Py
        // a*Ax*By - a*Ay*Bx = By*Px - Bx*Py
        // a * (Ax*By - Ay*Bx) = By*Px - Bx*Py
        // a = (By*Px - Bx*Py) / (Ax*By - Ay*Bx)

        var prizeX = machine.Prize.X + offset;
        var prizeY = machine.Prize.Y + offset;
        
        var aNumerator = (long)machine.B.Y * prizeX - machine.B.X * prizeY;
        var aDenominator = (long)machine.A.X * machine.B.Y - machine.A.Y * machine.B.X;

        if (aNumerator % aDenominator != 0)
        {
            return 0;
        }
        
        var a = aNumerator / aDenominator;
        
        // a*Ax + b*Bx = Px
        // b*Bx = Px - a*Ax
        // b = (Px - a*Ax) / Bx
        var b = (prizeX - a * machine.A.X) / machine.B.X;

        return a * 3 + b;
    }
}