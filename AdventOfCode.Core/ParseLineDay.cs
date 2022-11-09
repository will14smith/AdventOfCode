using Superpower;

namespace AdventOfCode.Core;

public abstract class ParseLineDay<TModel, TResult1, TResult2> : LineDay<TModel, TResult1, TResult2>
{
    protected ParseLineDay(int dayNumber, ITestOutputHelper output) : base(dayNumber, output) { }

    protected override TModel ParseLine(string input) => Parser.MustParse(input); 
    protected abstract TextParser<TModel> Parser { get; }
}