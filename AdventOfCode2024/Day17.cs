using Microsoft.Z3;

namespace AdventOfCode2024;

[Day]
public partial class Day17 : Day<Day17.Model, string, long>
{
    public record Model(long A, long B, long C, IReadOnlyList<long> Instructions);
    
    protected override Model Parse(string input)
    {
        var parts = input.Split("\n\n");
        
        var initialValues = parts[0].Split("\n").Select(x => long.Parse(x["Register A: ".Length..])).ToArray();
        var instructions = parts[1]["Program: ".Length..].Split(",").Select(long.Parse).ToArray();
        
        return new Model(initialValues[0], initialValues[1], initialValues[2], instructions);
    }

    [Sample("Register A: 729\nRegister B: 0\nRegister C: 0\n\nProgram: 0,1,5,4,3,0", "4,6,3,5,6,3,5,2,1,0")]
    protected override string Part1(Model input)
    {
        var a = input.A;
        var b = input.B;
        var c = input.C;
        var instructions = input.Instructions;

        var ip = 0;

        var output = new List<long>();
        
        while (ip >= 0 && ip < instructions.Count)
        {
            var instruction = instructions[ip++];
            var operand = instruction is 0 or 2 or 5 or 6 or 7 ? instructions[ip++] switch
            {
                0 => 0,
                1 => 1,
                2 => 2,
                3 => 3,
                4 => a,
                5 => b,
                6 => c,
                _ => throw new InvalidOperationException(),
            } : instructions[ip++];

            switch (instruction)
            {
                case 0: a >>= (int)operand; break;
                case 1: b ^= operand; break;
                case 2: b = operand % 8; break;
                case 3: if (a != 0) { ip = (int) operand; } break;
                case 4: b ^= c; break;
                case 5: output.Add(operand % 8); break;
                case 6: c = b >> (int)operand; break;
                case 7: c = a >> (int)operand; break;

                default: throw new InvalidOperationException();
            }
        }
        
        return string.Join(",", output);
    }

    protected override long Part2(Model input)
    {
        // this is manually decompiled from the problem input

        var ctx = new Context();
        var solver = ctx.MkSolver();
        
        var size = (uint)input.Instructions.Count * 3;
        var a= ctx.MkBVConst("a", size);
        var a0 = a;

        foreach (var instruction in input.Instructions)
        {
            // b = a % 8
            var b = ctx.MkBVAND(a, ctx.MkBV(7, size));

            // b = b ^ 2
            b = ctx.MkBVXOR(b, ctx.MkBV(2, size));
            
            // c = a >> b
            var c = ctx.MkBVLSHR(a, b);
            
            // b = b ^ 3
            b = ctx.MkBVXOR(b, ctx.MkBV(3, size));
            
            // b = b ^ c
            b = ctx.MkBVXOR(b, c);
            
            // output(b % 8)
            b = ctx.MkBVAND(b, ctx.MkBV(7, size));
            solver.Assert(ctx.MkEq(b, ctx.MkBV(instruction, size)));
            
            // a = a >> 3
            a = ctx.MkBVLSHR(a, ctx.MkBV(3, size));
            
            // jnx 0
        }

        // final `a` value must be 0 to have exited 
        solver.Assert(ctx.MkEq(a, ctx.MkBV(0, size)));

        // there are multiple answers, this loop works on the hope that finding the correct one linearly is quick...
        // if that didn't work then a binary search would work too
        var result = long.MaxValue;
        while (true)
        {
            if (solver.Check() == Status.UNSATISFIABLE)
            {
                return result;
            }

            result = ((BitVecNum)solver.Model.Eval(a0)).Int64;
            solver.Assert(ctx.MkBVULT(a0, ctx.MkBV(result, size)));
        }
    }
}