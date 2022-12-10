using System.Text;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2022;

[Day]
public partial class Day10 : ParseLineDay<Day10.Instruction, int, string>
{
    private const string Sample = "addx 15\naddx -11\naddx 6\naddx -3\naddx 5\naddx -1\naddx -8\naddx 13\naddx 4\nnoop\naddx -1\naddx 5\naddx -1\naddx 5\naddx -1\naddx 5\naddx -1\naddx 5\naddx -1\naddx -35\naddx 1\naddx 24\naddx -19\naddx 1\naddx 16\naddx -11\nnoop\nnoop\naddx 21\naddx -15\nnoop\nnoop\naddx -3\naddx 9\naddx 1\naddx -3\naddx 8\naddx 1\naddx 5\nnoop\nnoop\nnoop\nnoop\nnoop\naddx -36\nnoop\naddx 1\naddx 7\nnoop\nnoop\nnoop\naddx 2\naddx 6\nnoop\nnoop\nnoop\nnoop\nnoop\naddx 1\nnoop\nnoop\naddx 7\naddx 1\nnoop\naddx -13\naddx 13\naddx 7\nnoop\naddx 1\naddx -33\nnoop\nnoop\nnoop\naddx 2\nnoop\nnoop\nnoop\naddx 8\nnoop\naddx -1\naddx 2\naddx 1\nnoop\naddx 17\naddx -9\naddx 1\naddx 1\naddx -3\naddx 11\nnoop\nnoop\naddx 1\nnoop\naddx 1\nnoop\nnoop\naddx -13\naddx -19\naddx 1\naddx 3\naddx 26\naddx -30\naddx 12\naddx -1\naddx 3\naddx 1\nnoop\nnoop\nnoop\naddx -9\naddx 18\naddx 1\naddx 2\nnoop\nnoop\naddx 9\nnoop\nnoop\nnoop\naddx -1\naddx 2\naddx -37\naddx 1\naddx 3\nnoop\naddx 15\naddx -21\naddx 22\naddx -6\naddx 1\nnoop\naddx 2\naddx 1\nnoop\naddx -10\nnoop\nnoop\naddx 20\naddx 1\naddx 2\naddx 2\naddx -6\naddx -11\nnoop\nnoop\nnoop";

    protected override TextParser<Instruction> LineParser => Span.EqualTo("addx ").IgnoreThen(Numerics.IntegerInt32).Select(x => (Instruction)new Instruction.AddX(x))
        .Or(Span.EqualTo("noop").Select(_ => (Instruction) new Instruction.NoOp()));

    [Sample(Sample, 13140)]
    protected override int Part1(IEnumerable<Instruction> input)
    {
        var cycles = 0;
        var x = 1;

        var sum = 0;
        
        foreach (var instruction in input)
        {
            switch (instruction)
            {
                case Instruction.AddX addX:
                    cycles++;
                    if(CheckCycle()) return sum;
                    
                    cycles++;
                    if(CheckCycle()) return sum;

                    x += addX.Value;
                    break;
                case Instruction.NoOp noOp: 
                    cycles++; 
                    if(CheckCycle()) return sum;
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(instruction));
            }
        }

        throw new Exception("no solution");

        bool CheckCycle()
        {
            if (cycles > 0 && (cycles - 20) % 40 == 0)
            {
                sum += cycles*x;
            }

            return cycles > 220;
        }
    }

    [Sample(Sample, "\n##..##..##..##..##..##..##..##..##..##..\n###...###...###...###...###...###...###.\n####....####....####....####....####....\n#####.....#####.....#####.....#####.....\n######......######......######......####\n#######.......#######.......#######.....\n")]
    protected override string Part2(IEnumerable<Instruction> input)
    {
        var cycles = 0;
        var x = 1;

        var output = new StringBuilder();
        output.Append('\n');
        
        foreach (var instruction in input)
        {
            switch (instruction)
            {
                case Instruction.AddX addX:
                    cycles++;
                    if(CheckCycle()) return output.ToString();
                    
                    cycles++;
                    if(CheckCycle()) return output.ToString();

                    x += addX.Value;
                    break;
                case Instruction.NoOp noOp: 
                    cycles++; 
                    if(CheckCycle()) return output.ToString();
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(instruction));
            }
        }

        throw new Exception("no solution");

        bool CheckCycle()
        {
            output.Append(Math.Abs((cycles-1) % 40 - x) <= 1 ? '#' : '.');
            if ((cycles % 40) == 0)
            {
                output.Append('\n');
            }

            return cycles >= 240;
        }
    }
    
    public abstract record Instruction
    {
        public record AddX(int Value) : Instruction;
        public record NoOp : Instruction;
    }
}
