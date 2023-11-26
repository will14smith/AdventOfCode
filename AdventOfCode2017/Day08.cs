using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace AdventOfCode2017;

[Day]
public partial class Day08 : ParseLineDay<Day08.Model, int, int>
{
    private static readonly TextParser<string> RegisterName = Character.Letter.Many().Select(x => new string(x));
    private static readonly TextParser<bool> Increment = Character.EqualTo(' ').IgnoreThen(Span.EqualTo("inc ").Select(_ => true).Or(Span.EqualTo("dec ").Select(_ => false)));
    private static readonly TextParser<TextSpan> If = Span.EqualTo(" if ");
    private static readonly TextParser<Condition> ConditionMatcher = 
        Span.EqualTo(">=").Try().Select(_ => Condition.GreaterThanOrEqual)
            .Or(Span.EqualTo("<=").Try().Select(_ => Condition.LessThanOrEqual))
            .Or(Span.EqualTo("==").Select(_ => Condition.Equal))
            .Or(Span.EqualTo("!=").Select(_ => Condition.NotEqual))
            .Or(Span.EqualTo(">").Select(_ => Condition.GreaterThan))
            .Or(Span.EqualTo("<").Select(_ => Condition.LessThan))
        ;

    protected override TextParser<Model> LineParser { get; } =
        from targetRegister in RegisterName
        from inc in Increment
        from value in Numerics.IntegerInt32
        from conditionRegister in If.IgnoreThen(RegisterName)
        from condition in Span.WhiteSpace.IgnoreThen(ConditionMatcher).ThenIgnore(Span.WhiteSpace)
        from conditionValue in Numerics.IntegerInt32
        select new Model(targetRegister, inc, value, conditionRegister, condition, conditionValue);

    [Sample("b inc 5 if a > 1\na inc 1 if b < 5\nc dec -10 if a >= 1\nc inc -20 if c == 10", 1)]
    protected override int Part1(IEnumerable<Model> input)
    {
        var machine = new Machine();
        foreach (var model in input) machine.Run(model);
        return machine.Registers.Values.Max();
    }

    [Sample("b inc 5 if a > 1\na inc 1 if b < 5\nc dec -10 if a >= 1\nc inc -20 if c == 10", 10)]
    protected override int Part2(IEnumerable<Model> input)
    {
        var machine = new Machine();
        foreach (var model in input) machine.Run(model);
        return machine.Max;
    }
    
    public record Model(string TargetRegister, bool Increment, int Value, string ConditionRegister, Condition Condition, int ConditionValue);
    public enum Condition
    {
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        Equal,
        NotEqual,
    }

    private class Machine
    {
        public Dictionary<string, int> Registers { get; } = new();
        public int Max { get; private set; }

        public void Run(Model model)
        {
            if (!Registers.TryGetValue(model.ConditionRegister, out var conditionValue))
            {
                conditionValue = 0;
            }
            
            switch (model.Condition)
            {
                case Condition.LessThan: if (conditionValue >= model.ConditionValue) return; break;
                case Condition.LessThanOrEqual: if (conditionValue > model.ConditionValue) return; break;
                case Condition.GreaterThan: if (conditionValue <= model.ConditionValue) return; break;
                case Condition.GreaterThanOrEqual: if (conditionValue < model.ConditionValue) return; break;
                case Condition.Equal: if (conditionValue != model.ConditionValue) return; break;
                case Condition.NotEqual: if (conditionValue == model.ConditionValue) return; break;

                default: throw new ArgumentOutOfRangeException();
            }
            
            if (!Registers.TryGetValue(model.TargetRegister, out var targetValue))
            {
                targetValue = 0;
            }

            if (model.Increment)
            {
                targetValue += model.Value;
            }
            else
            {
                targetValue -= model.Value;
            }
            
            Registers[model.TargetRegister] = targetValue;
            if (Max < targetValue) Max = targetValue;
        }
    }
}