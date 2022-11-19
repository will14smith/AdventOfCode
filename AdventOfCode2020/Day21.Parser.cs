using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2020;

public partial class Day21
{
    protected override Tokenizer<TokenType> Tokenizer => new TokenizerBuilder<TokenType>()
        .Ignore(Span.WhiteSpace)
        .Match(Span.EqualTo("contains"), TokenType.Contains)
        .Match(Character.Letter.AtLeastOnce(), TokenType.Identifier)
        .Match(Character.EqualTo('('), TokenType.LeftBracket)
        .Match(Character.EqualTo(','), TokenType.Comma)
        .Match(Character.EqualTo(')'), TokenType.RightBracket)
        .Build();

    private static readonly TokenListParser<TokenType, string[]> Contains =
        Token.Sequence(TokenType.LeftBracket, TokenType.Contains)
            .IgnoreThen(Token.EqualTo(TokenType.Identifier).Select(x => x.ToStringValue()).AtLeastOnceDelimitedBy(Token.EqualTo(TokenType.Comma)))
            .ThenIgnore(Token.EqualTo(TokenType.RightBracket));
        
    private static readonly TokenListParser<TokenType, Line> LineParser = Token.EqualTo(TokenType.Identifier).Select(x => x.ToStringValue()).AtLeastOnce().Then(Contains, (ingredients, allergens) => new Line(ingredients, allergens)); 
    private static readonly TokenListParser<TokenType, Line[]> ModelParser = LineParser.AtLeastOnce();

    protected override TokenListParser<TokenType, Model> Parser => ModelParser.Select(x => new Model(x));

    public enum TokenType
    {
        Contains,
        Identifier,
        LeftBracket,
        Comma,
        RightBracket,
    }
}