using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2022;

[Day]
public partial class Day21 : ParseLineDay<Day21.Model, long, long>
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

    [Sample(Sample, 152L)]
    protected override long Part1(IEnumerable<Model> input)
    {
        var indexed = input.ToDictionary(x => x.Target, x => x.Value);
        var root = BuildTree(indexed, indexed["root"]);
        return ((Expression.Number)root).Value;
    }
    
    [Sample(Sample, 301L)]
    protected override long Part2(IEnumerable<Model> input)
    {
        var indexed = input.ToDictionary(x => x.Target, x => x.Value);

        var previousRoot = (Expression.Binary)indexed["root"];
        indexed["root"] = new Expression.Binary(Operation.Eql, previousRoot.Left, previousRoot.Right);
        indexed["humn"] = new Expression.Human();
        
        var root = (Expression.Binary) BuildTree(indexed, indexed["root"]);
        root = Rearrange(root);

        return ((Expression.Number)root.Right).Value;
    }

    private static Expression.Binary Rearrange(Expression.Binary root)
    {
        if (root.Left is Expression.Number)
        {
            root = new Expression.Binary(Operation.Eql, root.Right, root.Left);
        }

        while (true)
        {
            if (root.Left is Expression.Human)
            {
                return root;
            }

            if (root.Left is not Expression.Binary binary || binary.Operation == Operation.Eql)
            {
                throw new InvalidOperationException("invalid expression");
            }

            root = binary switch
            {
                { Left: Expression.Number leftNumber, Right: var rightExpr } => binary.Operation switch
                {
                    Operation.Add => new Expression.Binary(Operation.Eql, rightExpr, Simplify(new Expression.Binary(Operation.Sub, root.Right, leftNumber))),
                    Operation.Sub => new Expression.Binary(Operation.Eql, rightExpr, Simplify(new Expression.Binary(Operation.Sub, leftNumber, root.Right))),
                    Operation.Mul => new Expression.Binary(Operation.Eql, rightExpr, Simplify(new Expression.Binary(Operation.Div, root.Right, leftNumber))),
                    Operation.Div => new Expression.Binary(Operation.Eql, rightExpr, Simplify(new Expression.Binary(Operation.Div, leftNumber, root.Right))),
                },

                { Left: var leftExpr, Right: Expression.Number rightNumber } => binary.Operation switch
                {
                    Operation.Add => new Expression.Binary(Operation.Eql, leftExpr, Simplify(new Expression.Binary(Operation.Sub, root.Right, rightNumber))),
                    Operation.Sub => new Expression.Binary(Operation.Eql, leftExpr, Simplify(new Expression.Binary(Operation.Add, root.Right, rightNumber))),
                    Operation.Mul => new Expression.Binary(Operation.Eql, leftExpr, Simplify(new Expression.Binary(Operation.Div, root.Right, rightNumber))),
                    Operation.Div => new Expression.Binary(Operation.Eql, leftExpr, Simplify(new Expression.Binary(Operation.Mul, root.Right, rightNumber))),
                },

                _ => throw new InvalidOperationException("invalid expression")
            };
        }
    }

    private static Expression BuildTree(IReadOnlyDictionary<string, Expression> indexed, Expression expression)
    {
        return expression switch
        {
            Expression.Human human => human,
            Expression.Number number => number,
            Expression.Variable variable => BuildTree(indexed, indexed[variable.Name]),

            Expression.Binary binary => Simplify(new Expression.Binary(binary.Operation, BuildTree(indexed, binary.Left), BuildTree(indexed, binary.Right))),
            
            _ => throw new ArgumentOutOfRangeException(nameof(expression))
        };
    }
    
    private static Expression Simplify(Expression expression)
    {
        if(expression is not Expression.Binary { Operation: var operation, Left: Expression.Number left, Right: Expression.Number right })
        {
            return expression;
        }

        return operation switch
        {
            Operation.Add => new Expression.Number(left.Value + right.Value),
            Operation.Sub => new Expression.Number(left.Value - right.Value),
            Operation.Mul => new Expression.Number(left.Value * right.Value),
            Operation.Div => new Expression.Number(left.Value / right.Value),
            
            _ => expression,
        };
    }

    public record Model(string Target, Expression Value);

    public abstract record Expression
    {
        public record Variable(string Name) : Expression;
        public record Number(long Value) : Expression;
        public record Binary(Operation Operation, Expression Left, Expression Right) : Expression;
        public record Human : Expression;

        public static Expression CreateBinary(char op, Expression left, Expression right) =>
            op switch
            {
                '+' => new Binary(Operation.Add, left, right),
                '-' => new Binary(Operation.Sub, left, right),
                '*' => new Binary(Operation.Mul, left, right),
                '/' => new Binary(Operation.Div, left, right),
            };
    }
    public enum Operation
    {
        Add,
        Sub,
        Mul,
        Div,
        Eql,
    }
}
