namespace AdventOfCode2020;

[Day]
public partial class Day18 : Day<string, long, long>
{
    protected override string Parse(string input) => input;

    [Sample("2 * 3 + (4 * 5)\n2 * 3 + (4 * 5)\n", 2*26L)]
    [Sample("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 13632L)]
    protected override long Part1(string input) => Solve(ExpressionsPart1.MustParse(Tokenizer, input));

    [Sample("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 23340L)]
    protected override long Part2(string input) => Solve(ExpressionsPart2.MustParse(Tokenizer, input));
    
    private static long Solve(IEnumerable<Expr> arg) => arg.Select(Evaluate).Sum();
    private static long Evaluate(Expr expr) =>
        expr switch
        {
            Expr.Number number => number.Value,
            Expr.Plus plus => Evaluate(plus.Left) + Evaluate(plus.Right),
            Expr.Star star => Evaluate(star.Left) * Evaluate(star.Right),

            _ => throw new ArgumentOutOfRangeException(nameof(expr))
        };
}
