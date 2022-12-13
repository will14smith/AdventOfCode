using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2022;

public partial class Day13
{
    public enum TokenType
    {
        LeftSquare,
        RightSquare,
        Comma,
        Number,
    }

    protected override Tokenizer<TokenType> Tokenizer => new TokenizerBuilder<TokenType>()
        .Ignore(Span.WhiteSpace)
        .Match(Character.EqualTo('['), TokenType.LeftSquare)
        .Match(Character.EqualTo(']'), TokenType.RightSquare)
        .Match(Character.EqualTo(','), TokenType.Comma)
        .Match(Numerics.Integer, TokenType.Number)
        .Build();

    private static readonly TokenListParser<TokenType, int> NumberParser = Token.EqualTo(TokenType.Number).Select(x => int.Parse(x.ToStringValue()));
    private static readonly TokenListParser<TokenType, Data> DataNumberParser = NumberParser.Select(x => (Data)new Data.Number(x));

    private static readonly TokenListParser<TokenType, Data> DataArrayParser =
            from _1 in Token.EqualTo(TokenType.LeftSquare)
            from values in Superpower.Parse.Ref(() => DataParser).ManyDelimitedBy(Token.EqualTo(TokenType.Comma))
            from _2 in Token.EqualTo(TokenType.RightSquare)
            select (Data) new Data.Array(values);

    private static readonly TokenListParser<TokenType, Data> DataParser = DataNumberParser.Or(DataArrayParser);
    private static readonly TokenListParser<TokenType, Packet> PacketParser = DataParser.Select(x => new Packet(x));
    private static readonly TokenListParser<TokenType, (Packet, Packet)> PairParser = PacketParser.Then(PacketParser);

    protected override TokenListParser<TokenType, Model> Parser => PairParser.Many().Select(x => new Model(x));
}