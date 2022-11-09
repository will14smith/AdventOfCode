using Superpower;

namespace AdventOfCode.Core;

public abstract class ParseDay<TModel, TResult1, TResult2> : Day<TModel, TResult1, TResult2>
{
    protected ParseDay(int dayNumber, ITestOutputHelper output) : base(dayNumber, output) { }

    protected override TModel Parse(string input) => Parser.MustParse(input);
    protected abstract TextParser<TModel> Parser { get; }
}