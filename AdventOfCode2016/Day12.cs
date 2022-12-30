using System.Security.Cryptography;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2016;

[Day]
public partial class Day12 : ParseLineDay<Day12.Instruction, int, int>
{
    private const string Sample = "cpy 41 a\ninc a\ninc a\ndec a\njnz a 2\ndec a";

    private static readonly TextParser<Destination> RegisterParser = Span.Regex("[a-z]").Select(x => new Destination(x.ToStringValue()[0] - 'a'));
    private static readonly TextParser<Source> SourceParser = RegisterParser.Select(x => (Source)new Source.Register(x.Id)).Or(Numerics.IntegerInt32.Select(x => (Source)new Source.Literal(x)));
    
    private static readonly TextParser<Instruction> CopyParser = Span.EqualTo("cpy ").IgnoreThen(SourceParser).ThenIgnore(Character.EqualTo(' ')).Then(RegisterParser).Select(x => (Instruction)new Instruction.Copy(x.Item1, x.Item2));
    private static readonly TextParser<Instruction> IncParser = Span.EqualTo("inc ").IgnoreThen(RegisterParser).Select(x => (Instruction)new Instruction.Inc(x));
    private static readonly TextParser<Instruction> DecParser = Span.EqualTo("dec ").IgnoreThen(RegisterParser).Select(x => (Instruction)new Instruction.Dec(x));
    private static readonly TextParser<Instruction> JumpParser = Span.EqualTo("jnz ").IgnoreThen(SourceParser).ThenIgnore(Character.EqualTo(' ')).Then(SourceParser).Select(x => (Instruction)new Instruction.Jump(x.Item1, x.Item2));
    private static readonly TextParser<Instruction> InstructionParser = CopyParser.Or(IncParser).Or(DecParser).Or(JumpParser);

    protected override TextParser<Instruction> LineParser => InstructionParser;

    [Sample(Sample, 42)]
    protected override int Part1(IEnumerable<Instruction> input) => Run(input, new int[4]);

    protected override int Part2(IEnumerable<Instruction> input) => Run(input, new[] { 0, 0, 1, 0 });

    private static int Run(IEnumerable<Instruction> input, int[] registers)
    {
        var ip = 0;
        var mem = input.ToList();

        while (ip < mem.Count)
        {
            switch (mem[ip])
            {
                case Instruction.Copy copy:
                    ip++;
                    registers[copy.Destination.Id] = copy.Source switch
                    {
                        Source.Literal literal => literal.Value,
                        Source.Register register => registers[register.Id],

                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;

                case Instruction.Inc inc:
                    ip++;
                    registers[inc.Destination.Id]++;
                    break;

                case Instruction.Dec dec:
                    ip++;
                    registers[dec.Destination.Id]--;
                    break;

                case Instruction.Jump jump:
                    var sourceValue = jump.Value switch
                    {
                        Source.Literal literal => literal.Value,
                        Source.Register register => registers[register.Id],

                        _ => throw new ArgumentOutOfRangeException()
                    };

                    if (sourceValue != 0)
                    {
                        ip += jump.Offset switch
                        {
                            Source.Literal literal => literal.Value,
                            Source.Register register => registers[register.Id],

                            _ => throw new ArgumentOutOfRangeException()
                        };
                    }
                    else
                    {
                        ip++;
                    }

                    break;

                default: throw new ArgumentOutOfRangeException();
            }
        }

        return registers[0];
    }

    public abstract record Instruction
    {
        public record Copy(Source Source, Destination Destination) : Instruction;
        public record Inc(Destination Destination) : Instruction;
        public record Dec(Destination Destination) : Instruction;
        public record Jump(Source Value, Source Offset) : Instruction;
    }

    public abstract record Source
    {
        public record Register(int Id) : Source;
        public record Literal(int Value) : Source;
    }

    public record Destination(int Id);
}
