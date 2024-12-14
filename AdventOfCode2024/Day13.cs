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
    protected override long Part1(Model input) => input.Machines.Sum(SolveCheapest);

    private long SolveCheapest(Machine machine)
    {
        var ctx = new Context();
        var optimize = ctx.MkOptimize();

        var a = ctx.MkIntConst("a");
        var b = ctx.MkIntConst("b");

        optimize.Assert(a >= 0);
        optimize.Assert(b >= 0);
        optimize.Assert(ctx.MkEq(machine.A.X * a + machine.B.X * b, ctx.MkInt(machine.Prize.X)));
        optimize.Assert(ctx.MkEq(machine.A.Y * a + machine.B.Y * b, ctx.MkInt(machine.Prize.Y)));
        var min = optimize.MkMinimize(3 * a + b);
        
        return optimize.Check() == Status.SATISFIABLE ? ((IntNum)min.Value).Int64 : 0;
    }

    protected override long Part2(Model input) => input.Machines.Sum(SolveCheapest2);
    
    private long SolveCheapest2(Machine machine)
    {
        var ctx = new Context();
        var optimize = ctx.MkOptimize();

        var a = ctx.MkIntConst("a");
        var b = ctx.MkIntConst("b");

        optimize.Assert(a >= 0);
        optimize.Assert(b >= 0);
        optimize.Assert(ctx.MkEq(machine.A.X * a + machine.B.X * b, ctx.MkInt(machine.Prize.X + 10000000000000)));
        optimize.Assert(ctx.MkEq(machine.A.Y * a + machine.B.Y * b, ctx.MkInt(machine.Prize.Y + 10000000000000)));
        var min = optimize.MkMinimize(3 * a + b);
        
        return optimize.Check() == Status.SATISFIABLE ? ((IntNum)min.Value).Int64 : 0;
    }
}