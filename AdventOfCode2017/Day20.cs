using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2017;

[Day]
public partial class Day20 : ParseLineDay<Day20.Model, int, int>
{
    public static readonly TextParser<LongPosition3> PositionParser =
        Character.EqualTo('<').IgnoreThen(Numerics.IntegerInt32)
            .ThenIgnore(Character.EqualTo(',')).Then(Numerics.IntegerInt32)
            .ThenIgnore(Character.EqualTo(',')).Then(Numerics.IntegerInt32)
            .ThenIgnore(Character.EqualTo('>'))
            .Select(x => new LongPosition3(x.Item1.Item1, x.Item1.Item2, x.Item2));

    protected override TextParser<Model> LineParser { get; } =
        Span.EqualTo("p=").IgnoreThen(PositionParser)
            .ThenIgnore(Span.EqualTo(", v=")).Then(PositionParser)
            .ThenIgnore(Span.EqualTo(", a=")).Then(PositionParser)
            .Select(x => new Model(x.Item1.Item1, x.Item1.Item2, x.Item2));

    [Sample("p=<3,0,0>, v=<2,0,0>, a=<-1,0,0>\np=<4,0,0>, v=<0,0,0>, a=<-2,0,0>", 0)]
    protected override int Part1(IEnumerable<Model> input)
    {
        var models = input.ToArray();

        for (var i = 0; i < models.Length; i++)
        {
            models[i] = Steps(models[i], 1_000);
        }

        return models.Select((x, i) => (x, i)).MinBy(x => x.x.Position.TaxiDistance()).i;
    }

    protected override int Part2(IEnumerable<Model> input)
    {
        var models = input.ToList();
        
        for (var i = 0; i < 1_000; i++)
        {
            for (var j = 0; j < models.Count; j++)
            {
                models[j] = Step(models[j]);
            }

            models = models.GroupBy(x => x.Position, x => x, (position3, enumerable) => (position3, enumerable.ToList())).Where(x => x.Item2.Count == 1).Select(x => x.Item2[0]).ToList();
        }
        
        return models.Count;
    }

    private Model Step(Model model)
    {
        var newVelocity = model.Velocity + model.Acceleration;
        var newPosition = model.Position + newVelocity;
        
        return new Model(newPosition, newVelocity, model.Acceleration);
    }
    private Model Steps(Model model, int n)
    {
        // p = p0 + v1 + v2 + v3 + ... + vn
        // v1 = v0 + a
        // v2 = v1 + a = v0 + 2a
        // vn = v(n-1) + a = v0 + na
        // p = p0 + (v0 + a) + (v0 + 2a) + ... + (v0 + na)
        // p = p0 + nv0 + (1 + 2 + ... + n)a
        // p = p0 + nv0 + (n(n+1) / 2)a
        
        var newVelocity = model.Velocity + n * model.Acceleration;
        var newPosition = model.Position + n * model.Velocity + (n * (n+1) / 2) * model.Acceleration;
        
        return new Model(newPosition, newVelocity, model.Acceleration);
    }
    
    public record Model(LongPosition3 Position, LongPosition3 Velocity, LongPosition3 Acceleration);
}
