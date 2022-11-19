using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2020;

public partial class Day20
{
    private static readonly Tokenizer<TokenType> StaticTokenizer = new TokenizerBuilder<TokenType>()
        .Match(Character.EqualTo('\n'), TokenType.NewLine)
        .Ignore(Span.WhiteSpace)
        .Match(Numerics.Integer, TokenType.Number)
        .Match(Span.EqualTo("Tile"), TokenType.Tile)
        .Match(Character.EqualTo(':'), TokenType.Colon)
        .Match(Character.EqualTo('.'), TokenType.Pixel)
        .Match(Character.EqualTo('#'), TokenType.Pixel)
        .Build();

    protected override Tokenizer<TokenType> Tokenizer => StaticTokenizer;

    private static readonly TokenListParser<TokenType, long> Number = Token.EqualTo(TokenType.Number).Apply(Numerics.IntegerInt64); 
    private static readonly TokenListParser<TokenType, long> TileHeader = Token.EqualTo(TokenType.Tile).IgnoreThen(Number).ThenIgnore(Token.Sequence(TokenType.Colon, TokenType.NewLine)); 
    private static readonly TokenListParser<TokenType, bool> Pixel = Token.EqualTo(TokenType.Pixel).Select(x => x.Span.Source[x.Position.Absolute] == '#'); 
    private static readonly TokenListParser<TokenType, bool[]> ImageLine = Pixel.AtLeastOnce().ThenIgnore(Token.EqualTo(TokenType.NewLine));
    private static readonly TokenListParser<TokenType, bool[,]> ImageLines = ImageLine.AtLeastOnce().Select(lines => lines.Combine());
    private static readonly TokenListParser<TokenType, Tile> TileParser = TileHeader.Then(id => ImageLines.Select(image => new Tile(id, image))).ThenIgnore(Token.EqualTo(TokenType.NewLine));
    private static readonly TokenListParser<TokenType, Model> ModelParser = TileParser.AtLeastOnce().Select(tile => new Model(tile.ToDictionary(x => x.Id)));

    protected override TokenListParser<TokenType, Model> Parser => ModelParser;

    public enum TokenType
    {
        Number,
        Tile,
            
        Colon,
        Pixel,
        NewLine,
    }
}