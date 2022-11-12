using Superpower;
using Superpower.Parsers;

namespace AdventOfCode.Core;

public abstract class ParseLineDay<TModel, TResult1, TResult2> : ParseDay<IEnumerable<TModel>, TResult1, TResult2>
{
    protected ParseLineDay(int dayNumber, ITestOutputHelper output) : base(dayNumber, output) { }

    protected override TextParser<IEnumerable<TModel>> Parser => LineParser.ManyDelimitedBy(Span.EqualTo('\n')).Select(x => (IEnumerable<TModel>)x);
    protected abstract TextParser<TModel> LineParser { get; }
}