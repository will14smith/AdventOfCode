using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2015;

[Day]
public partial class Day12 : ParseDay<Day12.Model, int, int>
{
    private static readonly TextParser<Model> Number = Numerics.IntegerInt32.Select(x => (Model)new Model.Number(x));
    private static readonly TextParser<Model.String> String = QuotedString.CStyle.Select(x => new Model.String(x));
    private static readonly TextParser<Model> Array = Span.EqualTo('[').IgnoreThen(Superpower.Parse.Ref(() => Value).ManyDelimitedBy(Span.EqualTo(','))).ThenIgnore(Span.EqualTo(']')).Select(x => (Model)new Model.Array(x));
    private static readonly TextParser<(string, Model)> ObjectEntry = String.ThenIgnore(Span.EqualTo(':')).Then(Superpower.Parse.Ref(() => Value)).Select(x => (x.Item1.Value, x.Item2)); 
    private static readonly TextParser<Model> Object = Span.EqualTo('{').IgnoreThen(ObjectEntry.ManyDelimitedBy(Span.EqualTo(','))).ThenIgnore(Span.EqualTo('}')).Select(x => (Model)new Model.Object(x.ToDictionary(e => e.Item1, e => e.Item2)));

    private static readonly TextParser<Model> Value = Number.Or(String.Cast<Model.String, Model>()).Or(Array).Or(Object);
    protected override TextParser<Model> Parser => Value;

    [Sample("2", 2)]
    [Sample("\"a\"", 0)]
    [Sample("[1,2,3]", 6)]
    [Sample("{\"a\":2,\"b\":4}", 6)]
    [Sample("[[[3]]]", 3)]
    [Sample("{\"a\":{\"b\":4},\"c\":-1}", 3)]
    protected override int Part1(Model input) => Sum(input);

    [Sample("[1,2,3]", 6)]
    [Sample("[1,{\"c\":\"red\",\"b\":2},3]", 4)]
    [Sample("{\"d\":\"red\",\"e\":[1,2,3,4],\"f\":5}", 0)]
    [Sample("[1,\"red\",5]", 6)]
    protected override int Part2(Model input) => SumNotRed(input);

    private static int Sum(Model input) =>
        input switch
        {
            Model.Number n => n.Value,
            Model.String => 0,
            Model.Array a => a.Items.Sum(Sum),
            Model.Object o => o.Entries.Values.Sum(Sum),
        }; 
    
    private static int SumNotRed(Model input) =>
        input switch
        {
            Model.Number n => n.Value,
            Model.String => 0,
            Model.Array a => a.Items.Sum(SumNotRed),
            Model.Object o => o.Entries.Any(x => x.Value is Model.String { Value: "red" }) ? 0 : o.Entries.Values.Sum(SumNotRed),
        };

    public abstract record Model
    {
        public record Number(int Value) : Model;
        public record String(string Value) : Model;

        public record Array(IReadOnlyList<Model> Items) : Model;
        public record Object(IReadOnlyDictionary<string, Model> Entries) : Model;
    }
}