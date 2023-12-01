using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2017;

[Day]
public partial class Day18 : ParseLineDay<Day18.Model, long, int>
{
    private static readonly TextParser<char> RegisterParser = Character.Letter;
    private static readonly TextParser<Source> SourceRegisterParser = RegisterParser.Select(x => (Source)new Source.Register(x));
    private static readonly TextParser<Source> SourceIntegerParser = Numerics.IntegerInt64.Select(x => (Source)new Source.Integer(x));
    private static readonly TextParser<Source> SourceParser = SourceRegisterParser.Or(SourceIntegerParser);
    
    private static readonly TextParser<Model> ModelPlayParser = Span.EqualTo("snd ").Try().IgnoreThen(SourceParser).Select(x => (Model)new Model.Play(x));
    private static readonly TextParser<Model> ModelSetParser = Span.EqualTo("set ").Try().IgnoreThen(RegisterParser).ThenIgnore(Span.EqualTo(" ")).Then(SourceParser).Select(x => (Model)new Model.Set(x.Item1, x.Item2));
    private static readonly TextParser<Model> ModelAddParser = Span.EqualTo("add ").Try().IgnoreThen(RegisterParser).ThenIgnore(Span.EqualTo(" ")).Then(SourceParser).Select(x => (Model)new Model.Add(x.Item1, x.Item2));
    private static readonly TextParser<Model> ModelMulParser = Span.EqualTo("mul ").Try().IgnoreThen(RegisterParser).ThenIgnore(Span.EqualTo(" ")).Then(SourceParser).Select(x => (Model)new Model.Mul(x.Item1, x.Item2));
    private static readonly TextParser<Model> ModelModParser = Span.EqualTo("mod ").Try().IgnoreThen(RegisterParser).ThenIgnore(Span.EqualTo(" ")).Then(SourceParser).Select(x => (Model)new Model.Mod(x.Item1, x.Item2));
    private static readonly TextParser<Model> ModelRecoverParser = Span.EqualTo("rcv ").Try().IgnoreThen(SourceParser).Select(x => (Model)new Model.Recover(x));
    private static readonly TextParser<Model> ModelJumpParser = Span.EqualTo("jgz ").Try().IgnoreThen(SourceParser).ThenIgnore(Span.EqualTo(" ")).Then(SourceParser).Select(x => (Model)new Model.Jump(x.Item1, x.Item2));

    protected override TextParser<Model> LineParser { get; } = ModelPlayParser.Or(ModelSetParser).Or(ModelAddParser).Or(ModelMulParser).Or(ModelModParser).Or(ModelRecoverParser).Or(ModelJumpParser);

    [Sample("set a 1\nadd a 2\nmul a a\nmod a 5\nsnd a\nset a 0\nrcv a\njgz a -1\nset a 1\njgz a -2", 4L)]
    protected override long Part1(IEnumerable<Model> input)
    {
        long sound = 0;
        var registers = new Dictionary<char, long>();
        var instructions = input.ToArray();
        long ip = 0;

        while (true)
        {
            switch (instructions[ip++])
            {
                case Model.Add add: registers[add.Destination] = GetValue(new Source.Register(add.Destination)) + GetValue(add.Source); break;
                case Model.Jump jump: if (GetValue(jump.Check) > 0) ip = ip - 1 + GetValue(jump.Offset); break;
                case Model.Mod mod: registers[mod.Destination] = GetValue(new Source.Register(mod.Destination)) % GetValue(mod.Source); break;
                case Model.Mul mul: registers[mul.Destination] = GetValue(new Source.Register(mul.Destination)) * GetValue(mul.Source); break;
                case Model.Play play: sound = GetValue(play.Source); break;
                case Model.Recover recover: if (GetValue(recover.Source) != 0) { return sound; } break;
                case Model.Set set: registers[set.Destination] = GetValue(set.Source); break;

                default: throw new ArgumentOutOfRangeException();
            }
        }

        long GetValue(Source source)
        {
            return source switch
            {
                Source.Integer integer => integer.Value,
                Source.Register register => registers.TryGetValue(register.Id, out var value) ? value : 0,
                _ => throw new ArgumentOutOfRangeException(nameof(source))
            };
        }
    }

    [Sample("snd 1\nsnd 2\nsnd p\nrcv a\nrcv b\nrcv c\nrcv d", 3)]
    protected override int Part2(IEnumerable<Model> input)
    {
        var instructions = input.ToArray();

        var program0Buffer = new Queue<long>();
        var program1Buffer = new Queue<long>();

        var program1SendCount = 0;
        
        var registers0 = new Dictionary<char, long> { { 'p', 0 } };
        var registers1 = new Dictionary<char, long> { { 'p', 1 } };
        
        var ip0 = 0L;
        var ip1 = 0L;

        var program0Stalled = false;
        var program1Stalled = false;
        
        while (!program0Stalled || !program1Stalled)
        {
            while (!program0Stalled) ProcessInstruction(true, ref ip0, registers0);
            while (!program1Stalled) ProcessInstruction(false, ref ip1, registers1);
        }

        return program1SendCount;
        
        void ProcessInstruction(bool program0Active, ref long ip, Dictionary<char, long> registers)
        {
            if (ip >= instructions.Length)
            {
                if (program0Active)
                {
                    program0Stalled = true;
                }
                else
                {
                    program1Stalled = true;
                }

                return;
            }
            
            switch (instructions[ip++])
            {
                case Model.Add add: registers[add.Destination] = GetRegister(add.Destination) + GetValue(add.Source); break;
                case Model.Jump jump: if (GetValue(jump.Check) > 0) ip += GetValue(jump.Offset) - 1; break;
                case Model.Mod mod: registers[mod.Destination] = GetRegister(mod.Destination) % GetValue(mod.Source); break;
                case Model.Mul mul: registers[mul.Destination] = GetRegister(mul.Destination) * GetValue(mul.Source); break;
                case Model.Set set: registers[set.Destination] = GetValue(set.Source); break;

                // play == send
                case Model.Play play:
                {
                    (program0Active ? program1Buffer : program0Buffer).Enqueue(GetValue(play.Source)); 
                    if (program0Active)
                    {
                        program1Stalled = false;
                    }
                    else
                    {
                        program0Stalled = false;
                        program1SendCount++;
                    }
                    break;
                }
                // recover == receive
                case Model.Recover recover:
                {
                    var buffer = (program0Active ? program0Buffer : program1Buffer);
                    if (buffer.Count == 0)
                    {
                        if (program0Active)
                        {
                            program0Stalled = true;
                        }
                        else
                        {
                            program1Stalled = true;
                        }
                        ip--;
                        break;
                    }

                    registers[((Source.Register)recover.Source).Id] = buffer.Dequeue();
                    
                    break;
                }
                
                default: throw new ArgumentOutOfRangeException();
            }
            
            long GetValue(Source source)
            {
                return source switch
                {
                    Source.Integer integer => integer.Value,
                    Source.Register register => GetRegister(register.Id),
                    _ => throw new ArgumentOutOfRangeException(nameof(source))
                };
            }
            long GetRegister(char source) => registers.TryGetValue(source, out var value) ? value : 0;
        }
    }

    public abstract record Model
    {
        public record Play(Source Source) : Model;
        public record Set(char Destination, Source Source) : Model;
        public record Add(char Destination, Source Source) : Model;
        public record Mul(char Destination, Source Source) : Model;
        public record Mod(char Destination, Source Source) : Model;
        public record Recover(Source Source) : Model;
        public record Jump(Source Check, Source Offset) : Model;
    }

    public abstract record Source
    {
        public record Register(char Id) : Source;
        public record Integer(long Value) : Source;
    }
}
