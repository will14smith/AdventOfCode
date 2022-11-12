using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace AdventOfCode2015;

[Day]
public partial class Day07 : ParseLineDay<Day07.Definition, int, int>
{
    private static readonly TextParser<string> NameParser = Span.MatchedBy(Character.Lower.AtLeastOnce()).Select(x => x.ToStringValue());
    
    private static readonly TextParser<Expression> AtomParser = 
        NameParser.Select(x => (Expression)new Expression.Ref(x))
        .Or(Numerics.IntegerInt32.Select(x => (Expression)new Expression.Val(x)));
    
    private static readonly TextParser<Func<Expression, Expression, Expression>> BinaryParser = 
        Span.EqualTo(" ").IgnoreThen(
            Span.EqualTo("AND").Select<TextSpan, Func<Expression, Expression, Expression>>(_ => (a, b) => new Expression.And(a, b))
                .Or(Span.EqualTo("OR").Select<TextSpan, Func<Expression, Expression, Expression>>(_ => (a, b) => new Expression.Or(a, b)))
                .Or(Span.EqualTo("LSHIFT").Select<TextSpan, Func<Expression, Expression, Expression>>(_ => (a, b) => new Expression.LeftShift(a, b)))
                .Or(Span.EqualTo("RSHIFT").Select<TextSpan, Func<Expression, Expression, Expression>>(_ => (a, b) => new Expression.RightShift(a, b)))
        ).ThenIgnore(Span.EqualTo(" "));

    private static readonly TextParser<Expression> ExpressionParser = 
            Span.EqualTo("NOT ").IgnoreThen(AtomParser).Select(x => (Expression)new Expression.Not(x))
            .Or(AtomParser.Then(BinaryParser.Then(AtomParser).Try().OptionalOrDefault()).Select(x => x.Item2 == default ? x.Item1 : x.Item2.Item1(x.Item1, x.Item2.Item2)));

    private static readonly TextParser<Definition> DefinitionParser =
        from e in ExpressionParser
        from _ in Span.EqualTo(" -> ")
        from n in NameParser
        select new Definition(n, e);

    protected override TextParser<Definition> LineParser => DefinitionParser;

    [Sample("123 -> a", 123)]
    [Sample("123 -> b\nb -> a", 123)]
    [Sample("b -> a\n123 -> b", 123)]
    [Sample("123 AND 123 -> b\nb -> a", 123)]
    [Sample("122 -> b\nb OR 1 -> a", 123)]
    [Sample("123 -> b\nb LSHIFT 1 -> a", 246)]
    [Sample("123 -> b\nb RSHIFT 1 -> a", 61)]
    [Sample("123 -> b\nNOT b -> a", 65412)]
    protected override int Part1(IEnumerable<Definition> input) => Calculate(input, "a", new Dictionary<string, ushort>());
    
    protected override int Part2(IEnumerable<Definition> input)
    {
        var definitions = input as Definition[] ?? input.ToArray();
        
        var a = Calculate(definitions, "a", new Dictionary<string, ushort>());
        return Calculate(definitions, "a", new Dictionary<string, ushort> { { "b", (ushort)a } });
    }
    
    private static int Calculate(IEnumerable<Definition> input, string target, Dictionary<string, ushort> values)
    {
        Definition?[] remaining = input.ToArray();

        ushort targetValue;
        while (!values.TryGetValue(target, out targetValue))
        {
            for (var i = 0; i < remaining.Length; i++)
            {
                var definition = remaining[i];
                if (definition == null)
                {
                    continue;
                }

                if (values.ContainsKey(definition.Name))
                {
                    remaining[i] = null;
                    continue;
                }

                var value = TryCalculate(definition.Value, values);
                if (value is not null)
                {
                    values[definition.Name] = value.Value;
                    remaining[i] = null;
                    break;
                }
            }
        }

        return targetValue;
    }

    private static ushort? TryCalculate(Expression expression, IReadOnlyDictionary<string, ushort> values)
    {
        return expression switch
        {
            Expression.And and => TryCalculate(and.Left, values) is { } left && TryCalculate(and.Right, values) is { } right ? (ushort)(left & right) : null,
            Expression.LeftShift leftShift => TryCalculate(leftShift.Left, values) is { } left && TryCalculate(leftShift.Right, values) is { } right ? (ushort)(left << right) : null,
            Expression.Not not => TryCalculate(not.Expression, values) is { } value ? (ushort)~value : null,
            Expression.Or or => TryCalculate(or.Left, values) is { } left && TryCalculate(or.Right, values) is { } right ? (ushort)(left | right) : null,
            Expression.Ref @ref => values.TryGetValue(@ref.Name, out var value) ? value : null,
            Expression.RightShift rightShift => TryCalculate(rightShift.Left, values) is { } left && TryCalculate(rightShift.Right, values) is { } right ? (ushort)(left >> right) : null,
            Expression.Val val => (ushort)val.Value,
            
            _ => throw new ArgumentOutOfRangeException(nameof(expression))
        };
    }

    public record Definition(string Name, Expression Value);
    public abstract record Expression
    {
        public record Ref(string Name) : Expression;
        public record Val(int Value) : Expression;
        
        public record Not(Expression Expression) : Expression;
        public record And(Expression Left, Expression Right) : Expression;
        public record Or(Expression Left, Expression Right) : Expression;
        public record LeftShift(Expression Left, Expression Right) : Expression;
        public record RightShift(Expression Left, Expression Right) : Expression;
    }
}