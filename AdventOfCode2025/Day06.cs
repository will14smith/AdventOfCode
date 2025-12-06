namespace AdventOfCode2025;

[Day]
public partial class Day06 : Day<Day06.Model, long, long>
{
    public enum Operation { Add, Multiply }
    public record Problem(Operation Operation, IReadOnlyList<long> Numbers);
    public record Model(IReadOnlyList<Problem> Problems1, IReadOnlyList<Problem> Problems2);

    protected override Model Parse(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        var current = new ProblemBuilder(0);
        var problemBuilders = new List<ProblemBuilder> { current };

        for (var column = 0; column < lines[0].Length; column++)
        {
            var hasDigit = false;
            for (var row = 0; row < lines.Length - 1; row++)
            {
                var ch = lines[row][column];
                if (!char.IsDigit(ch))
                {
                    continue;
                }
                
                current.AddDigitForLeftToRight(row, ch - '0');
                current.AddDigitForTopToBottom(column, ch - '0');
                
                hasDigit = true;
            }

            if (!hasDigit)
            {
                current = new ProblemBuilder(column + 1);
                problemBuilders.Add(current);
                continue;
            }
            
            current.Operation = lines[^1][column] switch
            {
                '+' => Operation.Add,
                '*' => Operation.Multiply,
                ' ' => current.Operation,
                
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
        
        var problems1 = problemBuilders.Select(pb => new Problem(pb.Operation, pb.LeftToRight)).ToList();
        var problems2 = problemBuilders.Select(pb => new Problem(pb.Operation, pb.TopToBottom)).ToList();
        
        return new Model(problems1, problems2);
    }

    [Sample("123 328  51 64 \n 45 64  387 23 \n  6 98  215 314\n*   +   *   +  \n", 4277556)]
    protected override long Part1(Model input) => input.Problems1.Sum(SolveProblem);

    [Sample("123 328  51 64 \n 45 64  387 23 \n  6 98  215 314\n*   +   *   +  \n", 3263827)]
    protected override long Part2(Model input) => input.Problems2.Sum(SolveProblem);

    private static long SolveProblem(Problem problem) =>
        problem.Numbers.Aggregate((acc, x) => problem.Operation switch
        {
            Operation.Add => acc + x,
            Operation.Multiply => acc * x,
            _ => throw new ArgumentOutOfRangeException()
        });
    
    private class ProblemBuilder(int startColumn)
    {
        public Operation Operation { get; set; }

        public List<long> LeftToRight { get; } = [];
        public List<long> TopToBottom { get; } = [];
        
        public void AddDigitForLeftToRight(int row, long digit)
        {
            while (LeftToRight.Count <= row)
            {
                LeftToRight.Add(0);
            }
            
            LeftToRight[row] = LeftToRight[row] * 10 + digit;
        }

        public void AddDigitForTopToBottom(int column, long digit)
        {
            column -= startColumn;
            
            while (TopToBottom.Count <= column)
            {
                TopToBottom.Add(0);
            }
            
            TopToBottom[column] = TopToBottom[column] * 10 + digit;
        }
    }
}