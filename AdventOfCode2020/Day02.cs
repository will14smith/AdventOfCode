using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2020;

[Day]
public partial class Day02 : ParseDay<IEnumerable<Day02.Model>, Day02.TokenType, int, int>
{
    private static readonly TokenListParser<TokenType, int> Number = Token.EqualTo(TokenType.Number).Apply(Numerics.IntegerInt32);
    private static readonly TokenListParser<TokenType, (int Min, int Max)> Range = Number.ThenIgnore(Token.EqualTo(TokenType.Dash)).Then(Number, (a, b) => (a, b));

    private static readonly TokenListParser<TokenType, Model> LineParser = Range
        .Then(Token.EqualTo(TokenType.String), (a, b) => (Range: a, Target: b.Span.Source[b.Position.Absolute]))
        .ThenIgnore(Token.EqualTo(TokenType.Colon))
        .Then(Token.EqualTo(TokenType.String), (a, b) => new Model(a.Range.Min, a.Range.Max, a.Target, b.ToStringValue()));

    protected override Tokenizer<TokenType> Tokenizer => new TokenizerBuilder<TokenType>()
        .Ignore(Span.WhiteSpace)
        .Match(Numerics.Natural, TokenType.Number)
        .Match(Character.Letter.AtLeastOnce(), TokenType.String)
        .Match(Character.EqualTo('-'), TokenType.Dash)
        .Match(Character.EqualTo(':'), TokenType.Colon)
        .Build();
    
    protected override TokenListParser<TokenType, IEnumerable<Model>> Parser => LineParser.AtLeastOnce().Select(x => (IEnumerable<Model>)x);


    [Sample("1-3 a: abcde\n1-3 b: cdefg\n2-9 c: ccccccccc\n", 2)]
    protected override int Part1(IEnumerable<Model> input) => input.Count(x =>
    {
        var count = x.Password.Count(c => c == x.Target);
        return count >= x.Min && count <= x.Max;
    });

    [Sample("1-3 a: abcde\n1-3 b: cdefg\n2-9 c: ccccccccc\n", 1)]
    protected override int Part2(IEnumerable<Model> input) => input.Count(x =>
    {
        var a = x.Password[x.Min - 1];
        var b = x.Password[x.Max - 1];

        return (a == x.Target) ^ (b == x.Target);
    });

    public enum TokenType
    {
        Number,
        String,
            
        Dash,
        Colon,
    }

    public record Model(int Min, int Max, char Target, string Password);
}
