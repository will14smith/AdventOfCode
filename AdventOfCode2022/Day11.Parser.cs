using System.Collections.Immutable;
using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2022;

public partial class Day11
{
    protected override Tokenizer<TokenType> Tokenizer => new TokenizerBuilder<TokenType>()
        .Ignore(Span.WhiteSpace)
        .Ignore(Character.EqualTo(':'))
        .Match(Span.EqualTo("Monkey"), TokenType.Monkey)
        .Match(Span.EqualTo("Starting items"), TokenType.StartingItems)
        .Match(Span.EqualTo("Operation: new ="), TokenType.Operation)
        .Match(Span.EqualTo("old"), TokenType.Old)
        .Match(Span.EqualTo("Test: divisible by"), TokenType.Test)
        .Match(Span.EqualTo("If true: throw to monkey"), TokenType.True)
        .Match(Span.EqualTo("If false: throw to monkey"), TokenType.False)
        .Match(Character.EqualTo('+'), TokenType.Plus)
        .Match(Character.EqualTo('*'), TokenType.Star)
        .Match(Character.EqualTo(','), TokenType.Comma)
        .Match(Numerics.Integer, TokenType.Number)
        .Build();

    private static readonly TokenListParser<TokenType, int> NumberParser = Token.EqualTo(TokenType.Number).Select(x => int.Parse(x.ToStringValue()));
    private static readonly TokenListParser<TokenType, int[]> NumberListParser = NumberParser.ManyDelimitedBy(Token.EqualTo(TokenType.Comma));

    private static readonly TokenListParser<TokenType, MonkeyExpression> ExpressionNumberParser = NumberParser.Select(x => (MonkeyExpression) new MonkeyExpression.Constant(x));
    private static readonly TokenListParser<TokenType, MonkeyExpression> ExpressionOldParser = Token.EqualTo(TokenType.Old).Select(_ => (MonkeyExpression) new MonkeyExpression.Old());
    private static readonly TokenListParser<TokenType, MonkeyExpression> ExpressionAtomParser = ExpressionNumberParser.Or(ExpressionOldParser);
    private static readonly TokenListParser<TokenType, MonkeyExpression> ExpressionParser = ExpressionAtomParser.Then(Token.EqualTo(TokenType.Plus).Or(Token.EqualTo(TokenType.Star))).Then(ExpressionAtomParser)
        .Select(x => x.Item1.Item2.Kind == TokenType.Plus ? (MonkeyExpression)new MonkeyExpression.Add(x.Item1.Item1, x.Item2) : new MonkeyExpression.Mult(x.Item1.Item1, x.Item2));
    
    private static readonly TokenListParser<TokenType, MonkeyAction> ActionParser = Token.Sequence(TokenType.Test, TokenType.Number, TokenType.True, TokenType.Number, TokenType.False, TokenType.Number)
        .Select(x => new MonkeyAction(int.Parse(x[1].ToStringValue()), int.Parse(x[3].ToStringValue()), int.Parse(x[5].ToStringValue())));
    
    private static readonly TokenListParser<TokenType, Monkey> MonkeyParser =
        from id in Token.EqualTo(TokenType.Monkey).IgnoreThen(NumberParser)
        from items in Token.EqualTo(TokenType.StartingItems).IgnoreThen(NumberListParser)
        from expr in Token.EqualTo(TokenType.Operation).IgnoreThen(ExpressionParser)
        from action in ActionParser
        select new Monkey(id, ToQueue(items.Select(x => (long)x)), expr, action);
    
    private static readonly TokenListParser<TokenType, Model> ModelParser = MonkeyParser.Many().AtEnd().Select(x => new Model(x.ToImmutableList()));
    
    private static ImmutableQueue<T> ToQueue<T>(IEnumerable<T> items) => 
        items.Aggregate(ImmutableQueue<T>.Empty, (current, item) => current.Enqueue(item));

    protected override TokenListParser<TokenType, Model> Parser => ModelParser;

    public enum TokenType
    {
        Number,
        Plus,
        Star,
        Comma,
        
        Monkey,
        StartingItems,
        Operation,
        Old,
        Test,
        True,
        False,
    }
}