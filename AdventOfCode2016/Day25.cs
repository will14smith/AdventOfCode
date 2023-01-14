using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2016;

[Day]
public partial class Day25 : ParseLineDay<Day25.Instruction, int, int>
{
    private static readonly TextParser<Destination> RegisterParser = Span.Regex("[a-z]").Select(x => new Destination(x.ToStringValue()[0] - 'a'));
    private static readonly TextParser<Source> SourceParser = RegisterParser.Select(x => (Source)new Source.Register(x.Id)).Or(Numerics.IntegerInt32.Select(x => (Source)new Source.Literal(x)));
    
    private static readonly TextParser<Instruction> CopyParser = Span.EqualTo("cpy ").IgnoreThen(SourceParser).ThenIgnore(Character.EqualTo(' ')).Then(SourceParser).Select(x => (Instruction)new Instruction.Copy(x.Item1, x.Item2));
    private static readonly TextParser<Instruction> IncParser = Span.EqualTo("inc ").IgnoreThen(RegisterParser).Select(x => (Instruction)new Instruction.Inc(x));
    private static readonly TextParser<Instruction> DecParser = Span.EqualTo("dec ").IgnoreThen(RegisterParser).Select(x => (Instruction)new Instruction.Dec(x));
    private static readonly TextParser<Instruction> JumpParser = Span.EqualTo("jnz ").IgnoreThen(SourceParser).ThenIgnore(Character.EqualTo(' ')).Then(SourceParser).Select(x => (Instruction)new Instruction.Jump(x.Item1, x.Item2));
    private static readonly TextParser<Instruction> OutParser = Span.EqualTo("out ").IgnoreThen(RegisterParser).Select(x => (Instruction)new Instruction.Out(x));
    private static readonly TextParser<Instruction> InstructionParser = CopyParser.Or(IncParser).Or(DecParser).Or(JumpParser).Or(OutParser);

    protected override TextParser<Instruction> LineParser => InstructionParser;

    protected override int Part1(IEnumerable<Instruction> input)
    {
        // found via decompilation
        var magic = 180;
        
        var mem = input.ToList();
        Optimise(mem);

        var output = Run(mem, new[] { magic, 0, 0, 0 });
        Output.WriteLine(string.Join(", ", output.Take(30)));

        return magic;
    }

    private static IEnumerable<int> Run(IEnumerable<Instruction> input, int[] registers)
    {
        var ip = 0;
        var mem = input.ToList();
        Optimise(mem);
        
        while (ip < mem.Count)
        {
            switch (mem[ip])
            {
                case Instruction.Copy copy:
                    ip++;

                    if (copy.Destination is not Source.Register copyDestinationRegister)
                    {
                        continue;
                    }
                    
                    registers[copyDestinationRegister.Id] = copy.Source switch
                    {
                        Source.Literal literal => literal.Value,
                        Source.Register register => registers[register.Id],

                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                
                case Instruction.Add add:
                    ip++;
                    
                    if (add.Destination is not Source.Register addDestinationRegister)
                    {
                        continue;
                    }
                    
                    registers[addDestinationRegister.Id] += add.Source switch
                    {
                        Source.Literal literal => literal.Value,
                        Source.Register register => registers[register.Id],

                        _ => throw new ArgumentOutOfRangeException()
                    };

                    break;
                
                case Instruction.Mult mult:
                    ip++;
                    
                    if (mult.Destination is not Source.Register multDestinationRegister)
                    {
                        continue;
                    }
                    
                    registers[multDestinationRegister.Id] += mult.Source1 switch
                    {
                        Source.Literal literal => literal.Value,
                        Source.Register register => registers[register.Id],

                        _ => throw new ArgumentOutOfRangeException()
                    } * mult.Source2 switch
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
                
                case Instruction.Out output:
                    ip++;
                    yield return registers[output.Destination.Id];
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }

    
    protected override int Part2(IEnumerable<Instruction> input) => 0;

    private static void Optimise(List<Instruction> mem)
    {
        for (var i = 0; i < mem.Count; i++)
        {
            var fusedAdd = IsFusedAdd(mem, i);
            if (fusedAdd.HasValue)
            {
                mem[i] = new Instruction.Add(new Source.Register(fusedAdd.Value.Source), new Source.Register(fusedAdd.Value.Destination));
                mem[i+1] = new Instruction.Copy(new Source.Literal(0), new Source.Register(fusedAdd.Value.Source));
                mem[i+2] = new Instruction.Jump(new Source.Literal(0), new Source.Literal(0));
                
                i += 2;
                continue;
            }
            
            var fusedMult = IsFusedMult(mem, i);
            if (fusedMult.HasValue)
            {
                mem[i] = new Instruction.Mult(fusedMult.Value.Source1, new Source.Register(fusedMult.Value.Source2), new Source.Register(fusedMult.Value.Destination));
                mem[i+1] = new Instruction.Copy(new Source.Literal(0), new Source.Register(fusedMult.Value.Source2));
                mem[i+2] = new Instruction.Copy(new Source.Literal(0), new Source.Register(fusedMult.Value.Temporary));
                mem[i+3] = new Instruction.Jump(new Source.Literal(0), new Source.Literal(0));
                mem[i+4] = new Instruction.Jump(new Source.Literal(0), new Source.Literal(0));
                mem[i+5] = new Instruction.Jump(new Source.Literal(0), new Source.Literal(0));
                
                i += 5;
                continue;
            }

        }
    }

    private static (int Destination, int Source)? IsFusedAdd(IReadOnlyList<Instruction> input, int ip)
    {
        if (ip + 2 >= input.Count)
        {
            return null;
        }

        if (input[ip] is not Instruction.Inc { Destination: var destination }) return null;
        if (input[ip + 1] is not Instruction.Dec { Destination: var source }) return null;
        if (input[ip + 2] is not Instruction.Jump { Value: Source.Register jumpValueRegister, Offset: Source.Literal { Value: -2 } }) return null;
        if (destination.Id == source.Id || jumpValueRegister.Id != source.Id) return null;

        return (destination.Id, source.Id);
    }
    
    private static (int Destination, Source Source1, int Source2, int Temporary)? IsFusedMult(IReadOnlyList<Instruction> input, int ip)
    {
        if (ip + 5 >= input.Count)
        {
            return null;
        }

        if (input[ip] is not Instruction.Copy { Source: var source1, Destination: Source.Register temporary }) return null;
        if (input[ip + 1] is not Instruction.Inc { Destination: var destination }) return null;
        if (input[ip + 2] is not Instruction.Dec { Destination: var decTemporary } || decTemporary.Id != temporary.Id) return null;
        if (input[ip + 3] is not Instruction.Jump { Value: Source.Register jumpTemporary, Offset: Source.Literal { Value: -2 } } || jumpTemporary.Id != temporary.Id) return null;
        if (input[ip + 4] is not Instruction.Dec { Destination: var source2 }) return null;
        if (input[ip + 5] is not Instruction.Jump { Value: Source.Register jumpSource2, Offset: Source.Literal { Value: -5 } } || jumpSource2.Id != source2.Id) return null;

        return (destination.Id, source1, source2.Id, temporary.Id);
    }
    
    public abstract record Instruction
    {
        public record Copy(Source Source, Source Destination) : Instruction;
        public record Inc(Destination Destination) : Instruction;
        public record Dec(Destination Destination) : Instruction;
        public record Jump(Source Value, Source Offset) : Instruction;
        public record Out(Destination Destination) : Instruction;
        
        public record Add(Source Source, Source Destination) : Instruction;
        public record Mult(Source Source1, Source Source2, Source Destination) : Instruction;
    }

    public abstract record Source
    {
        public record Register(int Id) : Source;
        public record Literal(int Value) : Source;
    }

    public record Destination(int Id);
}
