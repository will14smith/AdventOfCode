using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2015;

[Day]
public partial class Day23 : ParseLineDay<Day23.Instruction, int, int>
{
    private static readonly TextParser<char> Register = Character.In('a', 'b');

    private static readonly TextParser<Instruction> HalfParser = Span.EqualTo("hlf ").IgnoreThen(Register).Select(x => (Instruction)new Instruction.Half(x));
    private static readonly TextParser<Instruction> TripleParser = Span.EqualTo("tpl ").IgnoreThen(Register).Select(x => (Instruction)new Instruction.Triple(x));
    private static readonly TextParser<Instruction> IncrementParser = Span.EqualTo("inc ").IgnoreThen(Register).Select(x => (Instruction)new Instruction.Increment(x));
    private static readonly TextParser<Instruction> JumpParser = Span.EqualTo("jmp ").IgnoreThen(Numerics.IntegerInt32).Select(x => (Instruction)new Instruction.Jump(x));
    private static readonly TextParser<Instruction> JumpEvenParser = Span.EqualTo("jie ").IgnoreThen(Register).ThenIgnore(Span.EqualTo(", ")).Then(Numerics.IntegerInt32).Select(x => (Instruction)new Instruction.JumpEven(x.Item1, x.Item2));
    private static readonly TextParser<Instruction> JumpOneParser = Span.EqualTo("jio ").IgnoreThen(Register).ThenIgnore(Span.EqualTo(", ")).Then(Numerics.IntegerInt32).Select(x => (Instruction)new Instruction.JumpOne(x.Item1, x.Item2));

    protected override TextParser<Instruction> LineParser =>
        HalfParser.Try()
            .Or(TripleParser.Try())
            .Or(IncrementParser.Try())
            .Or(JumpParser.Try())
            .Or(JumpEvenParser.Try())
            .Or(JumpOneParser.Try());

    [Sample("inc b\njio b, +2\ntpl b\ninc b", 2)]
    protected override int Part1(IEnumerable<Instruction> input) => Evaluate(input, 0);

    protected override int Part2(IEnumerable<Instruction> input) => Evaluate(input, 1);

    private static int Evaluate(IEnumerable<Instruction> input, int initialA)
    {
        var instructions = input.ToList();

        var a = initialA;
        var b = 0;

        var ip = 0;

        while (ip < instructions.Count)
        {
            var instruction = instructions[ip++];

            switch (instruction)
            {
                case Instruction.Half half:
                    if (half.Register == 'a')
                    {
                        a >>= 1;
                    }
                    else
                    {
                        b >>= 1;
                    }

                    break;
                case Instruction.Triple triple:
                    if (triple.Register == 'a')
                    {
                        a *= 3;
                    }
                    else
                    {
                        b *= 3;
                    }

                    break;

                case Instruction.Increment increment:
                    if (increment.Register == 'a')
                    {
                        a++;
                    }
                    else
                    {
                        b++;
                    }

                    break;

                case Instruction.Jump jump:
                    ip = ip - 1 + jump.Offset;
                    break;

                case Instruction.JumpEven jumpEven:
                    if ((jumpEven.Register == 'a' ? a : b) % 2 == 0)
                    {
                        ip = ip - 1 + jumpEven.Offset;
                    }

                    break;

                case Instruction.JumpOne jumpOne:
                    if ((jumpOne.Register == 'a' ? a : b) == 1)
                    {
                        ip = ip - 1 + jumpOne.Offset;
                    }

                    break;

                default: throw new ArgumentOutOfRangeException(nameof(instruction));
            }
        }

        return b;
    }

    public abstract record Instruction
    {
        public record Half(char Register) : Instruction;
        public record Triple(char Register) : Instruction;
        public record Increment(char Register) : Instruction;
        public record Jump(int Offset) : Instruction;
        public record JumpEven(char Register, int Offset) : Instruction;
        public record JumpOne(char Register, int Offset) : Instruction;
    }
}