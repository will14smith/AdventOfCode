using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2023;

public partial class Day04
{
    public enum TokenType
    {
        Card,
        Number,
        Colon,
        Pipe,
        NewLine,
    }
    
    protected override Tokenizer<TokenType> Tokenizer { get; } = new TokenizerBuilder<TokenType>()
        .Match(Character.EqualTo('\n'), TokenType.NewLine)
        .Ignore(Span.WhiteSpace)
        .Match(Span.EqualTo("Card"), TokenType.Card)
        .Match(Numerics.Integer, TokenType.Number)
        .Match(Character.EqualTo(':'), TokenType.Colon)
        .Match(Character.EqualTo('|'), TokenType.Pipe)
        .Build();

    private static readonly TokenListParser<TokenType, int> NumberParser = 
        Token.EqualTo(TokenType.Number)
            .Select(x => int.Parse(x.Span.ToStringValue()));

    private static readonly TokenListParser<TokenType, int> HeaderParser = 
        Token.EqualTo(TokenType.Card)
            .IgnoreThen(NumberParser)
            .ThenIgnore(Token.EqualTo(TokenType.Colon));

    private static readonly TokenListParser<TokenType, Card> CardParser = 
        HeaderParser
            .Then(NumberParser.Many())
            .ThenIgnore(Token.EqualTo(TokenType.Pipe))
            .Then(NumberParser.Many())
            .Select(x => new Card(x.Item1.Item1, x.Item1.Item2, x.Item2));
    
    protected override TokenListParser<TokenType, Model> Parser { get; } =
        CardParser
            .ManyDelimitedBy(Token.EqualTo(TokenType.NewLine))
            .Select(x => new Model(x));
}