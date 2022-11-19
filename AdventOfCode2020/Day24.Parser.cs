using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2020;

public partial class Day24
{
    protected override Tokenizer<TokenType> Tokenizer => new TokenizerBuilder<TokenType>()
        .Match(Character.EqualTo('\n'), TokenType.NewLine)
        .Ignore(Span.WhiteSpace)
        .Match(Span.EqualTo("se"), TokenType.SouthEast)
        .Match(Span.EqualTo("sw"), TokenType.SouthWest)
        .Match(Span.EqualTo("ne"), TokenType.NorthEast)
        .Match(Span.EqualTo("nw"), TokenType.NorthWest)
        .Match(Span.EqualTo("e"), TokenType.East)
        .Match(Span.EqualTo("w"), TokenType.West)
        .Build();

    private static readonly TokenListParser<TokenType, Direction> DirectionParser =
        Token.EqualTo(TokenType.East).Select(_ => Direction.East)
            .Or(Token.EqualTo(TokenType.SouthEast).Select(_ => Direction.SouthEast))
            .Or(Token.EqualTo(TokenType.SouthWest).Select(_ => Direction.SouthWest))
            .Or(Token.EqualTo(TokenType.West).Select(_ => Direction.West))
            .Or(Token.EqualTo(TokenType.NorthWest).Select(_ => Direction.NorthWest))
            .Or(Token.EqualTo(TokenType.NorthEast).Select(_ => Direction.NorthEast));
    private static readonly TokenListParser<TokenType, Direction[]> LineParser = DirectionParser.AtLeastOnce().ThenIgnore(Token.EqualTo(TokenType.NewLine));
    private static readonly TokenListParser<TokenType, Direction[][]> ModelParser = LineParser.AtLeastOnce();

    protected override TokenListParser<TokenType, Direction[][]> Parser => ModelParser;
    
    public enum TokenType
    {
        East,
        SouthEast,
        SouthWest,
        West,
        NorthWest,
        NorthEast,
            
        NewLine,
    }
}