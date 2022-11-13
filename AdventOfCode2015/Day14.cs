using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2015;

[Day]
public partial class Day14 : ParseLineDay<Day14.Model, int, int>
{
    private static readonly TextParser<string> Name = Identifier.CStyle.Select(x => x.ToStringValue());
    protected override TextParser<Model> LineParser => Name.ThenIgnore(Span.EqualTo(" can fly ")).Then(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" km/s for ")).Then(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" seconds, but then must rest for ")).Then(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" seconds."))
        .Select(x => new Model(x.Item1.Item1.Item1, x.Item1.Item1.Item2, x.Item1.Item2, x.Item2));

    protected override int Part1(IEnumerable<Model> input) => input.Max(x => SimulateFor(x, 2503));
    protected override int Part2(IEnumerable<Model> input) => SimulateAllFor(input, 2503);

    private static int SimulateFor(Model model, int duration)
    {
        var distance = 0;
        
        while (duration > 0)
        {
            distance += model.Speed * Math.Min(model.HighDuration, duration);
            duration -= model.HighDuration + model.LowDuration;
        }

        return distance;
    }

    private static int SimulateAllFor(IEnumerable<Model> models, int duration)
    {
        var states = models.Select(x => new State(x, 0, 0, false, x.HighDuration)).ToList();

        for (var t = 0; t < duration; t++)
        {
            var leader = 0;
            for (var i = 0; i < states.Count; i++)
            {
                states[i] = Step(states[i]);
                leader = leader < states[i].Distance ? states[i].Distance : leader;
            }
            
            for (var i = 0; i < states.Count; i++)
            {
                if (states[i].Distance == leader)
                {
                    states[i] = states[i] with { Points = states[i].Points + 1 };
                }
            }
        }
        
        return states.Max(x => x.Points);
    }

    private static State Step(State state)
    {
        if (!state.Resting)
        {
            state = state with { Distance = state.Distance + state.Model.Speed };
        }

        if (state.StateDuration == 1)
        {
            return state with { Resting = !state.Resting, StateDuration = state.Resting ? state.Model.HighDuration : state.Model.LowDuration };
        }
        else
        {
            return state with { StateDuration = state.StateDuration - 1 };
        }
    }

    private record State(Model Model, int Distance, int Points, bool Resting, int StateDuration);
    
    public record Model(string Name, int Speed, int HighDuration, int LowDuration);
}