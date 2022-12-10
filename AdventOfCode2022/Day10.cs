using System.Text;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2022;

[Day]
public partial class Day10 : ParseLineDay<Day10.Instruction, int, string>
{
    private const string Sample = "addx 15\naddx -11\naddx 6\naddx -3\naddx 5\naddx -1\naddx -8\naddx 13\naddx 4\nnoop\naddx -1\naddx 5\naddx -1\naddx 5\naddx -1\naddx 5\naddx -1\naddx 5\naddx -1\naddx -35\naddx 1\naddx 24\naddx -19\naddx 1\naddx 16\naddx -11\nnoop\nnoop\naddx 21\naddx -15\nnoop\nnoop\naddx -3\naddx 9\naddx 1\naddx -3\naddx 8\naddx 1\naddx 5\nnoop\nnoop\nnoop\nnoop\nnoop\naddx -36\nnoop\naddx 1\naddx 7\nnoop\nnoop\nnoop\naddx 2\naddx 6\nnoop\nnoop\nnoop\nnoop\nnoop\naddx 1\nnoop\nnoop\naddx 7\naddx 1\nnoop\naddx -13\naddx 13\naddx 7\nnoop\naddx 1\naddx -33\nnoop\nnoop\nnoop\naddx 2\nnoop\nnoop\nnoop\naddx 8\nnoop\naddx -1\naddx 2\naddx 1\nnoop\naddx 17\naddx -9\naddx 1\naddx 1\naddx -3\naddx 11\nnoop\nnoop\naddx 1\nnoop\naddx 1\nnoop\nnoop\naddx -13\naddx -19\naddx 1\naddx 3\naddx 26\naddx -30\naddx 12\naddx -1\naddx 3\naddx 1\nnoop\nnoop\nnoop\naddx -9\naddx 18\naddx 1\naddx 2\nnoop\nnoop\naddx 9\nnoop\nnoop\nnoop\naddx -1\naddx 2\naddx -37\naddx 1\naddx 3\nnoop\naddx 15\naddx -21\naddx 22\naddx -6\naddx 1\nnoop\naddx 2\naddx 1\nnoop\naddx -10\nnoop\nnoop\naddx 20\naddx 1\naddx 2\naddx 2\naddx -6\naddx -11\nnoop\nnoop\nnoop";
    private const string Expected2 = "\n##..##..##..##..##..##..##..##..##..##..\n###...###...###...###...###...###...###.\n####....####....####....####....####....\n#####.....#####.....#####.....#####.....\n######......######......######......####\n#######.......#######.......#######.....\n";
    
    protected override TextParser<Instruction> LineParser => Span.EqualTo("addx ").IgnoreThen(Numerics.IntegerInt32).Select(x => (Instruction)new Instruction.AddX(x))
        .Or(Span.EqualTo("noop").Select(_ => (Instruction) new Instruction.NoOp()));

    [Sample(Sample, 13140)]
    protected override int Part1(IEnumerable<Instruction> input) => 
        Run(input).Where(s => (s.Cycle - 20) % 40 == 0).TakeWhile(s => s.Cycle <= 220).Sum(s => s.Cycle * s.X);

    [Sample(Sample, Expected2)]
    protected override string Part2(IEnumerable<Instruction> input)
    {
        // crt is 40*6, extra new lines are for easier reading of the output
        return "\n" + Run(input).TakeWhile(x => x.Cycle <= 240).Select(ToPixel).BatchesOfN(40).Select(x => x.Join()).Join("\n") + "\n";

        // sprite is at: x-1, x, x+1
        char ToPixel(State state) => Math.Abs((state.Cycle - 1) % 40 - state.X) <= 1 ? '#' : '.';
    }

    private static IEnumerable<State> Run(IEnumerable<Instruction> instructions)
    {
        var cycle = 0;
        var x = 1;
        
        foreach (var instruction in instructions)
        {
            switch (instruction)
            {
                case Instruction.AddX addX:
                    yield return new State(++cycle, x);
                    yield return new State(++cycle, x);

                    x += addX.Value;
                    break;
                case Instruction.NoOp noOp: 
                    yield return new State(++cycle, x);
                    break;
                
                default: throw new ArgumentOutOfRangeException(nameof(instruction));
            }
        }
    }
    private record State(int Cycle, int X);
    
    public abstract record Instruction
    {
        public record AddX(int Value) : Instruction;
        public record NoOp : Instruction;
    }
}
