namespace AdventOfCode.Core;

public abstract class LineDay<TModel, TResult1, TResult2> : Day<IEnumerable<TModel>, TResult1, TResult2>
{
    protected LineDay(int dayNumber, ITestOutputHelper output) : base(dayNumber, output) { }

    protected override IEnumerable<TModel> Parse(string input) => input.Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(ParseLine).ToArray();
    protected abstract TModel ParseLine(string input);
}