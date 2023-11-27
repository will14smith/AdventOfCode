using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2017;

[Day]
public partial class Day16 : ParseDay<Day16.Model, string, string>
{
    private static TextParser<Move> SpinParser { get; } = Character.EqualTo('s').IgnoreThen(Numerics.IntegerInt32).Select(x => (Move)new Move.Spin(x)); 
    private static TextParser<Move> ExchangeParser { get; } = Character.EqualTo('x').IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Character.EqualTo('/')).Then(Numerics.IntegerInt32).Select(x => (Move)new Move.Exchange(x.Item1, x.Item2)); 
    private static TextParser<Move> PartnerParser { get; } = Character.EqualTo('p').IgnoreThen(Character.Letter).ThenIgnore(Character.EqualTo('/')).Then(Character.Letter).Select(x => (Move)new Move.Partner(x.Item1, x.Item2)); 
    private static TextParser<Move> MoveParser { get; } = SpinParser.Or(ExchangeParser).Or(PartnerParser);
    protected override TextParser<Model> Parser { get; } = MoveParser.ManyDelimitedBy(Character.EqualTo(',')).Select(x => new Model(x));
    
    [Sample("s1", "pabcdefghijklmno")]
    [Sample("x3/4", "abcedfghijklmnop")]
    [Sample("pe/b", "aecdbfghijklmnop")]
    protected override string Part1(Model input)
    {
        Span<char> buffer1 = stackalloc char[16]; 
        Span<char> buffer2 = stackalloc char[16]; 
        
        "abcdefghijklmnop".CopyTo(buffer1);
        
        foreach (var move in input.Moves)
        {
            move.Apply(buffer1, buffer2);

            var temp = buffer1;
            buffer1 = buffer2;
            buffer2 = temp;
        }

        return new string(buffer1);
    }

    protected override string Part2(Model input)
    {
        // we have to do a billion so there better be a cycle.
        var (offset, length) = CycleDetection.Detect(() => (ReadOnlyMemory<char>)"abcdefghijklmnop".ToCharArray(), x =>
        {
            var a = new char[x.Length];
            var b = new char[x.Length];
            
            x.CopyTo(a);
            
            foreach (var move in input.Moves)
            {
                move.Apply(a, b);
                (a, b) = (b, a);
            }

            return a;
        }, (a, b) => new string(a.Span) == new string(b.Span));

        var totalIterationsRequired = 1_000_000_000;
        var stepsRemaining = (totalIterationsRequired - offset) % length;
        
        Span<char> buffer1 = stackalloc char[16]; 
        Span<char> buffer2 = stackalloc char[16]; 
        
        "abcdefghijklmnop".CopyTo(buffer1);

        for (var i = 0; i < stepsRemaining; i++)
        {
            foreach (var move in input.Moves)
            {
                move.Apply(buffer1, buffer2);

                var temp = buffer1;
                buffer1 = buffer2;
                buffer2 = temp;
            }
        }

        return new string(buffer1);
    }
    
    public record Model(IReadOnlyList<Move> Moves);

    public abstract record Move
    {
        public abstract void Apply(ReadOnlySpan<char> input, Span<char> output);
        
        public record Spin(int Amount) : Move
        {
            public override void Apply(ReadOnlySpan<char> input, Span<char> output)
            {
                input.SplitAt(input.Length - Amount, out var start, out var end);
                
                end.CopyTo(output);
                start.CopyTo(output[Amount..]);
            }
        }

        public record Exchange(int IndexA, int IndexB) : Move
        {
            public override void Apply(ReadOnlySpan<char> input, Span<char> output)
            {
                input.CopyTo(output);
                (output[IndexA], output[IndexB]) = (output[IndexB], output[IndexA]);
            }
        }

        public record Partner(char ProgramA, char ProgramB) : Move
        {
            public override void Apply(ReadOnlySpan<char> input, Span<char> output)
            {
                input.CopyTo(output);

                var indexA = output.IndexOf(ProgramA);
                var indexB = output.IndexOf(ProgramB);
                
                (output[indexA], output[indexB]) = (output[indexB], output[indexA]);
            }
        }
    }
}
