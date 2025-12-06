using Superpower;
using Superpower.Parsers;

namespace AdventOfCode.Core;

public abstract class ParseLineDay<TModel, TResult1, TResult2> : ParseDay<IEnumerable<TModel>, TResult1, TResult2>
{
    protected ParseLineDay(int dayNumber, ITestOutputHelper output) : base(dayNumber, output) { }

    protected override TextParser<IEnumerable<TModel>> Parser => LineParser.ThenIgnoreOptional(Span.EqualTo('\n')).Many().AtEnd().Select(IEnumerable<TModel> (x) => x);
    protected abstract TextParser<TModel> LineParser { get; }
}