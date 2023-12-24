using Microsoft.Z3;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2023;

[Day]
public partial class Day24 : ParseLineDay<Day24.Model, long, long>
{
    private static readonly TextParser<LongPosition3> CoordinateParser =
        Numerics.IntegerInt64.ThenIgnore(Span.EqualTo(", "))
            .Then(Numerics.IntegerInt64).ThenIgnore(Span.EqualTo(", "))
            .Then(Numerics.IntegerInt64).Select(x => new LongPosition3(x.Item1.Item1, x.Item1.Item2, x.Item2));
    protected override TextParser<Model> LineParser { get; } = CoordinateParser.ThenIgnore(Span.EqualTo(" @ ")).Then(CoordinateParser)
        .Select(x => new Model(x.Item1, x.Item2));
    
    // [Sample("19, 13, 30 @ -2, 1, -2\n18, 19, 22 @ -1, -1, -2\n20, 25, 34 @ -2, -2, -4\n12, 31, 28 @ -1, -2, -1\n20, 19, 15 @ 1, -5, -3", 2L)]
    protected override long Part1(IEnumerable<Model> input)
    {
        // var sampleArea = (new LongPosition(7, 7), new LongPosition(27, 27));
        var sampleArea = (new LongPosition(200000000000000, 200000000000000), new LongPosition(400000000000000, 400000000000000));

        var count = 0;
        
        var hails = input.ToArray();
        for (var i = 0; i < hails.Length; i++)
        {
            var hailI = hails[i];
            var gradI = (decimal)hailI.Velocity.Y / hailI.Velocity.X;
            // when X = 0, Y = ?
            var interceptI = hailI.Position.Y - hailI.Position.X * gradI;
            
            // x(t) = x(0) + vx * t
            // x(t) - x(0) = vx * t
            // t = (x(t) - x(0)) / vx
            
            for (var j = i + 1; j < hails.Length; j++)
            {
                var hailJ = hails[j];
                var gradJ = (decimal)hailJ.Velocity.Y / hailJ.Velocity.X;
                var interceptJ = hailJ.Position.Y - hailJ.Position.X * gradJ;

                // parallel
                if (gradI == gradJ) continue;
                
                // x = (iJ - iI) / (gI - gJ)
                var x = (interceptJ - interceptI) / (gradI - gradJ);
                var y = gradI * x + interceptI;

                var tI = (x - hailI.Position.X) / hailI.Velocity.X;
                var tJ = (x - hailJ.Position.X) / hailJ.Velocity.X;
                
                if (x >= sampleArea.Item1.X && x <= sampleArea.Item2.X && y >= sampleArea.Item1.Y && y <= sampleArea.Item2.Y && tI >= 0 && tJ >= 0)
                {
                    // Output.WriteLine($"{i} and {j} at ({x}, {y})");
                    
                    count++;
                }
            }
        }

        return count;
    }

    [Sample("19, 13, 30 @ -2, 1, -2\n18, 19, 22 @ -1, -1, -2\n20, 25, 34 @ -2, -2, -4\n12, 31, 28 @ -1, -2, -1\n20, 19, 15 @ 1, -5, -3", 47L)]
    protected override long Part2(IEnumerable<Model> input)
    {
        // I tried to solve it as a system of equations but it was complicated...
        
        using var ctx = new Context();

        var varX = ctx.MkRealConst("x");
        var varY = ctx.MkRealConst("y");
        var varZ = ctx.MkRealConst("z");
        var varU = ctx.MkRealConst("u");
        var varV = ctx.MkRealConst("v");
        var varW = ctx.MkRealConst("w");

        var solver = ctx.MkSolver();
        
        // only need 3 points to find a straight line
        foreach (var (model, index) in input.Select((x, i) => (x, i)).Take(3))
        {
            var t = ctx.MkRealConst($"t{index}");
            solver.Add(t > 0);
            
            solver.Add(ctx.MkEq(model.Position.X + model.Velocity.X * t, varX + varU * t));
            solver.Add(ctx.MkEq(model.Position.Y + model.Velocity.Y * t, varY + varV * t));
            solver.Add(ctx.MkEq(model.Position.Z + model.Velocity.Z * t, varZ + varW * t));
        }

        var status = solver.Check();
        if (status != Status.SATISFIABLE) throw new Exception("no");

        var result = solver.Model;
        var value = (RatNum) result.Eval(varX + varY + varZ);
        if (value.Denominator.Int64 != 1) throw new Exception("no");
        
        return value.Numerator.Int64;
    }

    public record Model(LongPosition3 Position, LongPosition3 Velocity);
}