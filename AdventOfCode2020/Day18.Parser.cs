using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2020;

public partial class Day18
{
    public static readonly Tokenizer<TokenType> Tokenizer = new TokenizerBuilder<TokenType>()
        .Ignore(Span.WhiteSpace)
        .Match(Numerics.Integer, TokenType.Number)
        .Match(Character.EqualTo('+'), TokenType.Plus)
        .Match(Character.EqualTo('*'), TokenType.Star)
        .Match(Character.EqualTo('('), TokenType.LeftBracket)
        .Match(Character.EqualTo(')'), TokenType.RightBracket)
        .Build();
    
    private static readonly TokenListParser<TokenType, Expr> NumberPart1 = Token.EqualTo(TokenType.Number).Apply(Numerics.IntegerInt64.Select(n => (Expr) new Expr.Number(n)));
    private static readonly TokenListParser<TokenType, Expr> BracedExpressionPart1 = Token.EqualTo(TokenType.LeftBracket).IgnoreThen(Superpower.Parse.Ref(() => ExpressionPart1)).ThenIgnore(Token.EqualTo(TokenType.RightBracket));
    private static readonly TokenListParser<TokenType, Expr> AtomPart1 = NumberPart1.Or(BracedExpressionPart1);
    private static readonly TokenListParser<TokenType, Expr> ExpressionPart1 = Superpower.Parse.Chain(Token.EqualTo(TokenType.Plus).Or(Token.EqualTo(TokenType.Star)), AtomPart1, (op, l, r) => op.Kind == TokenType.Plus ? new Expr.Plus(l, r) : new Expr.Star(l, r));
    private static readonly TokenListParser<TokenType, Expr[]> ExpressionsPart1 = ExpressionPart1.AtLeastOnce();

    private static readonly TokenListParser<TokenType, Expr> NumberPart2 = Token.EqualTo(TokenType.Number).Apply(Numerics.IntegerInt64.Select(n => (Expr) new Expr.Number(n)));
    private static readonly TokenListParser<TokenType, Expr> BracedExpressionPart2 = Token.EqualTo(TokenType.LeftBracket).IgnoreThen(Superpower.Parse.Ref(() => ExpressionPart2)).ThenIgnore(Token.EqualTo(TokenType.RightBracket));
    private static readonly TokenListParser<TokenType, Expr> AtomPart2 = NumberPart2.Or(BracedExpressionPart2);
    private static readonly TokenListParser<TokenType, Expr> AdditionPart2 = Superpower.Parse.Chain(Token.EqualTo(TokenType.Plus), AtomPart2, (op, l, r) => new Expr.Plus(l, r));
    private static readonly TokenListParser<TokenType, Expr> MultiplicationPart2 = Superpower.Parse.Chain(Token.EqualTo(TokenType.Star), AdditionPart2, (op, l, r) => new Expr.Star(l, r));
    private static readonly TokenListParser<TokenType, Expr> ExpressionPart2 = MultiplicationPart2;
    private static readonly TokenListParser<TokenType, Expr[]> ExpressionsPart2 = ExpressionPart2.AtLeastOnce();

    public abstract record Expr
    {
        public record Number(long Value) : Expr;
        public record Plus(Expr Left, Expr Right) : Expr;
        public record Star(Expr Left, Expr Right) : Expr;
    }
    
    public enum TokenType
    {
        Number,
        Plus,
        Star,
        LeftBracket,
        RightBracket,
    }
}