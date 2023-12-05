using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2017;

[Day]
public partial class Day23 : ParseLineDay<Day23.Model, int, long>
{
    private static readonly TextParser<char> RegisterParser = Character.Letter;
    private static readonly TextParser<Source> SourceRegisterParser = RegisterParser.Select(x => (Source)new Source.Register(x));
    private static readonly TextParser<Source> SourceIntegerParser = Numerics.IntegerInt64.Select(x => (Source)new Source.Integer(x));
    private static readonly TextParser<Source> SourceParser = SourceRegisterParser.Or(SourceIntegerParser);
    
    private static readonly TextParser<Model> ModelSetParser = Span.EqualTo("set ").Try().IgnoreThen(RegisterParser).ThenIgnore(Span.EqualTo(" ")).Then(SourceParser).Select(x => (Model)new Model.Set(x.Item1, x.Item2));
    private static readonly TextParser<Model> ModelSubParser = Span.EqualTo("sub ").Try().IgnoreThen(RegisterParser).ThenIgnore(Span.EqualTo(" ")).Then(SourceParser).Select(x => (Model)new Model.Sub(x.Item1, x.Item2));
    private static readonly TextParser<Model> ModelMulParser = Span.EqualTo("mul ").Try().IgnoreThen(RegisterParser).ThenIgnore(Span.EqualTo(" ")).Then(SourceParser).Select(x => (Model)new Model.Mul(x.Item1, x.Item2));
    private static readonly TextParser<Model> ModelJumpParser = Span.EqualTo("jnz ").Try().IgnoreThen(SourceParser).ThenIgnore(Span.EqualTo(" ")).Then(SourceParser).Select(x => (Model)new Model.Jump(x.Item1, x.Item2));
    
    protected override TextParser<Model> LineParser { get; } = ModelSetParser.Or(ModelSubParser).Or(ModelMulParser).Or(ModelJumpParser);
    
    protected override int Part1(IEnumerable<Model> input)
    {
        var registers = new Dictionary<char, long>();
        var instructions = input.ToArray();
        long ip = 0;

        var mulCount = 0;
        
        while (true)
        {
            if (ip >= instructions.Length)
            {
                break;
            }
            
            switch (instructions[ip++])
            {
                case Model.Set set: registers[set.Destination] = GetValue(set.Source); break;
                case Model.Sub sub: registers[sub.Destination] = GetValue(new Source.Register(sub.Destination)) - GetValue(sub.Source); break;
                case Model.Mul mul: registers[mul.Destination] = GetValue(new Source.Register(mul.Destination)) * GetValue(mul.Source); mulCount++; break;
                case Model.Jump jump: if (GetValue(jump.Check) != 0) ip = ip - 1 + GetValue(jump.Offset); break;

                default: throw new ArgumentOutOfRangeException();
            }
        }

        return mulCount;
        
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

    protected override long Part2(IEnumerable<Model> input)
    {
        long count = 0;

        for (var i = 107900; i <= 124900; i += 17)
        {
            var isPrime = true;

            for (var factor = 2; factor < i; factor++)
            {
                if (i % factor != 0)
                {
                    continue;
                }
                
                isPrime = false;
                goto incH;
            }
            
            incH:
            if (!isPrime)
            {
                count++;
            }
        }

        return count;
    }

    private static long Part2Original()
    {
        long a = 0, b = 0, c = 0, d = 0, e = 0, f = 0, g = 0, h = 0;

        // set a 1
        a = 1;
        // set b 79
        b = 79;
        // set c b
        c = b;
        // jnz a 2 (jump1) 
        if (a != 0) goto jump1;
        // jnz 1 5 (jump2)
        if (1 != 0) goto jump2;
        // mul b 100
        jump1:
        b = b * 100;
        // sub b -100000
        b = b + 100000;
        // set c b
        c = b;
        // sub c -17000
        c = c + 17000;
        // set f 1
        jump9:
        jump2:
        f = 1;
        // set d 2
        d = 2;
        // set e 2
        jump5:
        e = 2;
        // set g d
        jump4:
        g = d;
        // mul g e
        g = g * e;
        // sub g b
        g = g - b;
        // jnz g 2 (jump3)
        if (g != 0) goto jump3;
        // set f 0
        f = 0;
        // sub e -1
        jump3:
        e = e + 1;
        // set g e
        g = e;
        // sub g b
        g = g - b;
        // jnz g -8 (jump4)
        if (g != 0) goto jump4;
        // sub d -1
        d = d + 1;
        // set g d
        g = d;
        // sub g b
        g = g - b;
        // jnz g -13 (jump5)
        if (g != 0) goto jump5;
        // jnz f 2 (jump6)
        if (f != 0) goto jump6;
        // sub h -1
        h = h + 1;
        // set g b
        jump6:
        g = b;
        // sub g c
        g = g - c;
        // jnz g 2 (jump7)
        if (g != 0) goto jump7;
        // jnz 1 3 (jump8)
        if (1 != 0) goto jump8;
        // sub b -17
        jump7:
        b = b + 17;
        // jnz 1 -23 (jump9)
        if (1 != 0) goto jump9;

        jump8:
        return h;
    }

    
    public abstract record Model
    {
        public record Set(char Destination, Source Source) : Model;
        public record Sub(char Destination, Source Source) : Model;
        public record Mul(char Destination, Source Source) : Model;
        public record Jump(Source Check, Source Offset) : Model;
    }

    public abstract record Source
    {
        public record Register(char Id) : Source;
        public record Integer(long Value) : Source;
    }

}
