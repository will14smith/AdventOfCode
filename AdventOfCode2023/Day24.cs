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
        // p(t) = pX + vX * t
        
        // rock is p0 @ v0
        // hail is pi @ vi (i > 0)
        
        // rock and hail i collide at ti
        // p0 + v0 * ti = pi + vi * ti
        
        // re-arrange to have 0 on one side
        // p0 + v0 * t1 - p1 - v1 * t1 = 0
        // p0 - p1 + v0 * t1 - v1 * t1 = 0
        // (p0 - p1) + (v0 - v1) * t1 = 0
        
        // re-arrange to have ti on one side
        // (p0 - pi) + (v0 - vi) * ti = 0
        // (v0 - vi) * ti = pi - p0
        // ti = (pi - p0) / (v0 - v1)
        
        // now combine the x component 0 equation and the y component ti equation
        // (px0 - pxi) + (vx0 - vxi) * ti = 0
        // (px0 - pxi) + (vx0 - vxi) * (pyi - py0) / (vy0 - vyi) = 0
        // (vx0 - vxi) * (pyi - py0) / (vy0 - vyi) = (pxi - px0)
        // (vx0 - vxi) * (pyi - py0) = (pxi - px0) * (vy0 - vyi)
        
        // expand all the multiplications
        // (vx0 - vxi) * (pyi - py0) = (pxi - px0) * (vy0 - vyi)
        // vx0 * (pyi - py0) - vxi * (pyi - py0) = pxi * (vy0 - vyi) - px0 * (vy0 - vyi)
        // (vx0 * pyi - vx0 * py0) - (vxi * pyi - vxi * py0) = (pxi * vy0 - pxi * vyi) - (px0 * vy0 - px0 * vyi)
        // vx0 * pyi - vx0 * py0 - vxi * pyi + vxi * py0 = pxi * vy0 - pxi * vyi - px0 * vy0 + px0 * vyi

        // since it isn't linear, use hail j as well
        // vx0 * pyj - vx0 * py0 - vxj * pyj + vxj * py0 = pxj * vy0 - pxj * vyj - px0 * vy0 + px0 * vyj
        
        // subtract both sides, this cancels the non-linear part
        // vx0 * pyi - vx0 * py0 - vxi * pyi + vxi * py0 - (vx0 * pyj - vx0 * py0 - vxj * pyj + vxj * py0) = pxi * vy0 - pxi * vyi - px0 * vy0 + px0 * vyi - (pxj * vy0 - pxj * vyj - px0 * vy0 + px0 * vyj)
        // vx0 * pyi - vx0 * pyj - vxi * pyi + vxi * py0 - vxj * py0 + vxj * pyj = pxi * vy0 - pxj * vy0 - pxi * vyi + px0 * vyi - px0 * vyj + pxj * vyj
        // vx0 * (pyi - pyj) - vxi * pyi + (vxi - vxj) * py0 + vxj * pyj = (pxi - pxj) * vy0 - pxi * vyi + px0 * (vyi - vyj) + pxj * vyj
        // (pyi - pyj) * vx0 + (vxi - vxj) * py0 - (pxi - pxj) * vy0 - (vyi - vyj) * px0 = pxj * vyj + vxi * pyi - vxj * pyj - pxi * vyi
        
        // now it is in a form to use 6 simultaneous equations (since we have 6 unknowns) to solve, using the x/y/z components from hail i/j and j/k will work

        var solver = new RowReduction(6);
        
        (int, int)[] hailIndexes = [(0, 1), (1, 2)];
        var hail = input.Take(3).ToArray();

        var y = 0;
        foreach (var (ia, ib) in hailIndexes)
        {
            var a = hail[ia];
            var b = hail[ib];
            
            // xy
            solver[y, 0] = b.Velocity.Y - a.Velocity.Y;
            solver[y, 1] = a.Velocity.X - b.Velocity.X;
            solver[y, 3] = a.Position.Y - b.Position.Y;
            solver[y, 4] = b.Position.X - a.Position.X;
            solver[y, 6] = b.Position.X * b.Velocity.Y + a.Position.Y * a.Velocity.X - b.Position.Y * b.Velocity.X - a.Position.X * a.Velocity.Y;

            y++;
            
            // yz 
            solver[y, 1] = b.Velocity.Z - a.Velocity.Z;
            solver[y, 2] = a.Velocity.Y - b.Velocity.Y;
            solver[y, 4] = a.Position.Z - b.Position.Z;
            solver[y, 5] = b.Position.Y - a.Position.Y;
            solver[y, 6] = b.Position.Y * b.Velocity.Z + a.Position.Z * a.Velocity.Y - b.Position.Z * b.Velocity.Y - a.Position.Y * a.Velocity.Z;

            y++;
            
            // zx
            solver[y, 2] = b.Velocity.X - a.Velocity.X;
            solver[y, 0] = a.Velocity.Z - b.Velocity.Z;
            solver[y, 5] = a.Position.X - b.Position.X;
            solver[y, 3] = b.Position.Z - a.Position.Z;
            solver[y, 6] = b.Position.Z * b.Velocity.X + a.Position.X * a.Velocity.Z - b.Position.X * b.Velocity.Z - a.Position.Z * a.Velocity.X;

            y++;
        }

        solver.Reduce();
        
        return (long)(solver[0, 6] + solver[1, 6] + solver[2, 6]).Numerator;
    }

    public record Model(LongPosition3 Position, LongPosition3 Velocity);
}

public class RowReduction
{
    private readonly int _unknowns;
    private readonly BigRational[,] _matrix;
    public RowReduction(int unknowns)
    {
        _unknowns = unknowns;
        _matrix = new BigRational[unknowns, unknowns + 1];
    }

    public BigRational this[int row, int col]
    {
        get => _matrix[row, col];
        set => _matrix[row, col] = value;
    }

    public void Reduce()
    {
        for (var i = 0; i < _unknowns; i++)
        {
            // step 1: is i,i == 0: swap with next non-zero row
            var rowNormalisation = _matrix[i, i];
            if (rowNormalisation.IsZero)
            {
                for (var j = i + 1; j < _unknowns; j++)
                {
                    if (_matrix[j, i].IsZero)
                    {
                        continue;
                    }
                    
                    // swap rows i & j
                    for (var col = 0; col < _unknowns + 1; col++)
                    {
                        (_matrix[i, col], _matrix[j, col]) = (_matrix[j, col], _matrix[i, col]);
                    }

                    rowNormalisation = _matrix[i, i];
                    break;
                }

                if (rowNormalisation.IsZero)
                {
                    throw new Exception("no");
                }
            }
            
            // step 2: divide row i by i,i to normalise
            for (var col = 0; col < _unknowns + 1; col++)
            {
                _matrix[i, col] /= rowNormalisation;
            }
            
            // step 3: for each other row j - multiply row i by j,i and subtract
            for (var row = 0; row < _unknowns; row++)
            {
                if(row == i) continue;
                
                var factor = _matrix[row, i];
                for (var col = 0; col < _unknowns + 1; col++)
                {
                    _matrix[row, col] -= factor * _matrix[i, col];
                }
            }
        }
    }
}