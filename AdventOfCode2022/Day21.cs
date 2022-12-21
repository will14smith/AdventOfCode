using System.Numerics;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2022;

[Day]
public partial class Day21 : ParseLineDay<Day21.Model, BigInteger, BigInteger>
{
    private const string Sample = "root: pppw + sjmn\ndbpl: 5\ncczh: sllz + lgvd\nzczc: 2\nptdq: humn - dvpt\ndvpt: 3\nlfqf: 4\nhumn: 5\nljgn: 2\nsjmn: drzm * dbpl\nsllz: 4\npppw: cczh / lfqf\nlgvd: ljgn * ptdq\ndrzm: hmdt - zczc\nhmdt: 32";
    
    private static readonly TextParser<string> VariableParser = Span.Regex("[a-z]+").Select(x => x.ToStringValue());
    private static readonly TextParser<Expression> NumberParser = Numerics.IntegerInt32.Select(x => (Expression)new Expression.Number(x));
    private static readonly TextParser<Expression> BinaryParser = 
        from l in VariableParser
        from op in Span.Regex(" [+\\-*/] ")
        from r in VariableParser
        select Expression.CreateBinary(op.ToStringValue()[1], new Expression.Variable(l), new Expression.Variable(r));
    private static readonly TextParser<Expression> ExpressionParser = NumberParser.Or(BinaryParser);

    protected override TextParser<Model> LineParser => 
        VariableParser.ThenIgnore(Span.EqualTo(": ")).Then(ExpressionParser).Select(x => new Model(x.Item1, x.Item2));

    // [Sample(Sample, new BigInteger(128))]
    protected override BigInteger Part1(IEnumerable<Model> input)
    {
        var indexed = input.ToDictionary(x => x.Target, x => x.Value);

        while (indexed["root"] is not Expression.Number)
        {
            foreach (var (target, expr) in indexed)
            {
                if (expr is not Expression.Number number)
                {
                    continue;
                }
                
                if(target == "root")
                {
                    return number.Value;
                }
                
                indexed.Remove(target);
                foreach (var otherTarget in indexed.Keys)
                {
                    indexed[otherTarget] = Simplify(Substitute(indexed[otherTarget], new Expression.Variable(target), number));
                }
            }
        }

        return ((Expression.Number)indexed["root"]).Value;
    }
    
    private Expression Substitute(Expression expression, Expression target, Expression replacement)
    {
        return expression switch
        {
            Expression.Variable variable when variable == target => replacement,
            Expression.Variable variable => variable,
            Expression.Number number => number,

            Expression.Add add => new Expression.Add(Substitute(add.Left, target, replacement), Substitute(add.Right, target, replacement)),
            Expression.Sub sub => new Expression.Sub(Substitute(sub.Left, target, replacement), Substitute(sub.Right, target, replacement)),
            Expression.Mul mul => new Expression.Mul(Substitute(mul.Left, target, replacement), Substitute(mul.Right, target, replacement)),
            Expression.Div div => new Expression.Div(Substitute(div.Left, target, replacement), Substitute(div.Right, target, replacement)),
        };
    }
    
    private Expression Simplify(Expression expression)
    {
        return expression switch
        {
            Expression.Add { Left: Expression.Number left, Right: Expression.Number right } => new Expression.Number(left.Value + right.Value),
            Expression.Sub { Left: Expression.Number left, Right: Expression.Number right } => new Expression.Number(left.Value - right.Value),
            Expression.Mul { Left: Expression.Number left, Right: Expression.Number right } => new Expression.Number(left.Value * right.Value),
            Expression.Div { Left: Expression.Number left, Right: Expression.Number right } => new Expression.Number(left.Value / right.Value),
            
            _ => expression,
        };
    }

    // [Sample(Sample, 301L)]
    protected override BigInteger Part2(IEnumerable<Model> input)
    {
        var indexed = input.ToDictionary(x => x.Target, x => x.Value);

        var rootAdd = (Expression.Add)indexed["root"];
        indexed["root"] = new Expression.Equal(rootAdd.Left, rootAdd.Right);
        indexed["humn"] = new Expression.Human();
        
        // build tree
        var root = (Expression.Equal) BuildTree(indexed, indexed["root"]);
        
        // solve for humn
        while (true)
        {
            if (root.Left is Expression.Human)
            {
                return ((Expression.Number)root.Right).Value;
            }
            
            root = Rearrange(root);
        }

        throw new NotImplementedException();
    }

    private Expression.Equal Rearrange(Expression.Equal root)
    {
        if (root.Left is Expression.Number)
        {
            root = new Expression.Equal(root.Right, root.Left);
        }

        return root.Left switch
        {
            Expression.Human human => throw new NotImplementedException("complete."),
            Expression.Number number => throw new InvalidOperationException("invalid expression"),
            Expression.Variable variable => throw new InvalidOperationException("should have been removed."),

            Expression.Add { Left: Expression.Number left, Right: { } right } => new Expression.Equal(right, Simplify(new Expression.Sub(root.Right, left))),
            Expression.Add { Left: { } left, Right: Expression.Number right } => new Expression.Equal(left, Simplify(new Expression.Sub(root.Right, right))),
            // n - X = Y
            // n = X + Y
            // X = n - Y
            Expression.Sub { Left: Expression.Number left, Right: { } right } => new Expression.Equal(right, Simplify(new Expression.Sub(left, root.Right))),
            Expression.Sub { Left: { } left, Right: Expression.Number right } => new Expression.Equal(left, Simplify(new Expression.Add(root.Right, right))),
            Expression.Mul { Left: Expression.Number left, Right: { } right } => new Expression.Equal(right, Simplify(new Expression.Div(root.Right, left))),
            Expression.Mul { Left: { } left, Right: Expression.Number right } => new Expression.Equal(left, Simplify(new Expression.Div(root.Right, right))),
            // n / X = Y
            // n / Y = X
            Expression.Div { Left: Expression.Number left, Right: { } right } => new Expression.Equal(right, Simplify(new Expression.Div(left, root.Right))),
            Expression.Div { Left: { } left, Right: Expression.Number right } => new Expression.Equal(left, Simplify(new Expression.Mul(root.Right, right))),

            Expression.Equal equal => throw new InvalidOperationException("invalid expression"),

            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private Expression BuildTree(Dictionary<string, Expression> indexed, Expression expression)
    {
        return expression switch
        {
            Expression.Human human => human,
            Expression.Number number => number,
            Expression.Variable variable => BuildTree(indexed, indexed[variable.Name]),

            Expression.Add add => Simplify(new Expression.Add(BuildTree(indexed, add.Left), BuildTree(indexed, add.Right))),
            Expression.Sub sub => Simplify(new Expression.Sub(BuildTree(indexed, sub.Left), BuildTree(indexed, sub.Right))),
            Expression.Mul mul => Simplify(new Expression.Mul(BuildTree(indexed, mul.Left), BuildTree(indexed, mul.Right))),
            Expression.Div div => Simplify(new Expression.Div(BuildTree(indexed, div.Left), BuildTree(indexed, div.Right))),
            Expression.Equal equal => Simplify(new Expression.Equal(BuildTree(indexed, equal.Left), BuildTree(indexed, equal.Right))),
            
            _ => throw new ArgumentOutOfRangeException(nameof(expression))
        };
    }

    public record Model(string Target, Expression Value);

    public abstract record Expression
    {
        public record Variable(string Name) : Expression;
        public record Number(BigInteger Value) : Expression;
        
        public record Add(Expression Left, Expression Right) : Expression;
        public record Sub(Expression Left, Expression Right) : Expression;
        public record Mul(Expression Left, Expression Right) : Expression;
        public record Div(Expression Left, Expression Right) : Expression;

        public record Human : Expression;
        public record Equal(Expression Left, Expression Right) : Expression;

        public static Expression CreateBinary(char op, Expression left, Expression right) =>
            op switch
            {
                '+' => new Add(left, right),
                '-' => new Sub(left, right),
                '*' => new Mul(left, right),
                '/' => new Div(left, right),
            };
    }
}
