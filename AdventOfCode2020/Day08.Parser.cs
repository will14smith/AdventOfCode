using System.Collections.Immutable;
using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2020;

public partial class Day08
{
    protected override Tokenizer<Token> Tokenizer => new TokenizerBuilder<Token>()
        .Ignore(Span.WhiteSpace)
        .Match(Character.Lower.AtLeastOnce(), Token.OpCode)
        .Match(Numerics.Integer, Token.Number)
        .Build();

    private static readonly TokenListParser<Token, int> Num = Superpower.Parsers.Token.EqualTo(Token.Number).Apply(Numerics.IntegerInt32);

    private static readonly TokenListParser<Token, Op> Nop = Superpower.Parsers.Token.EqualToValue(Token.OpCode, "nop").IgnoreThen(Num).Select(n => (Op) new Op.Nop(n));
    private static readonly TokenListParser<Token, Op> Acc = Superpower.Parsers.Token.EqualToValue(Token.OpCode, "acc").IgnoreThen(Num).Select(n => (Op) new Op.Acc(n));
    private static readonly TokenListParser<Token, Op> Jmp = Superpower.Parsers.Token.EqualToValue(Token.OpCode, "jmp").IgnoreThen(Num).Select(n => (Op) new Op.Jmp(n));

    private static readonly TokenListParser<Token, Op> Instruction = Nop.Or(Acc).Or(Jmp);
    private static readonly TokenListParser<Token, ImmutableList<Op>> Instructions = Instruction.AtLeastOnce().Select(x => x.ToImmutableList());

    protected override TokenListParser<Token, ImmutableList<Op>> Parser => Instructions;

    public abstract class Op
    {
        public class Nop : Op
        {
            public int Value { get; }
            public Nop(in int value) => Value = value;
        }

        public class Acc : Op
        {
            public int Value { get; }
            public Acc(in int value) => Value = value;
        }

        public class Jmp : Op
        {
            public int Value { get; }
            public Jmp(in int value) => Value = value;
        }
    }


    public enum Token
    {
        OpCode,
        Number,
    }
}