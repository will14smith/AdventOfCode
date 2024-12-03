using System.Text.RegularExpressions;

namespace AdventOfCode2024;

[Day]
public partial class Day03 : Day<Day03.Model, int, int>
{
    public record Model(IReadOnlyList<Instruction> Instructions);

    public abstract record Instruction
    {
        public record Mul(int X, int Y) : Instruction;
        public record Do : Instruction;
        public record Dont : Instruction;
    }
    
    [GeneratedRegex(@"mul\((\d{1,3}),(\d{1,3})\)|do\(\)|don't\(\)")]
    private static partial Regex InstructionRegex();

    protected override Model Parse(string input) => new(InstructionRegex().Matches(input).Select(ParseInstruction).ToArray());
    private static Instruction ParseInstruction(Match x)
    {
        var match = x.Groups[0].Value;

        if (match.StartsWith("mul"))
        {
            return new Instruction.Mul(int.Parse(x.Groups[1].Value), int.Parse(x.Groups[2].Value));
        }
        
        return match.StartsWith("don't") ? new Instruction.Dont() : new Instruction.Do();
    }

    [Sample("xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))", 161)]
    [Sample("mul(4*", 0)]
    [Sample("mul(6,9!", 0)]
    [Sample("?(12,34)", 0)]
    [Sample("mul ( 2 , 4 )", 0)]
    protected override int Part1(Model input) => input.Instructions.OfType<Instruction.Mul>().Sum(x => x.X * x.Y);
    [Sample("xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))", 48)]
    protected override int Part2(Model input)
    {
        var sum = 0;
        var enabled = true;
        
        foreach (var instruction in input.Instructions)
        {
            switch (instruction)
            {
                case Instruction.Do: enabled = true; break;
                case Instruction.Dont: enabled = false; break;
                case Instruction.Mul mul when enabled: sum += mul.X * mul.Y; break;
            }
        }
        
        return sum;
    }
}