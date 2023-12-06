using System.Collections;
using System.Numerics;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2017;

[Day]
public partial class Day25 : ParseDay<Day25.Model, int, int>
{
    private static readonly TextParser<char> StartStateParser = Span.EqualTo("Begin in state ").IgnoreThen(Character.Letter).ThenIgnore(Span.EqualTo(".\n"));
    private static readonly TextParser<int> StepsParser = Span.EqualTo("Perform a diagnostic checksum after ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" steps.\n"));
    
    private static readonly TextParser<char> StateDescriptorHeaderParser = Span.EqualTo("\nIn state ").IgnoreThen(Character.Letter).ThenIgnore(Span.EqualTo(":\n"));
    private static readonly TextParser<int> StateActionHeaderParser = Span.EqualTo("  If the current value is ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(":\n"));
    private static readonly TextParser<bool> LeftOrRightParser = Span.EqualTo("left").Select(_ => false).Or(Span.EqualTo("right").Select(_ => true));
    private static readonly TextParser<StateAction> StateActionParser = 
        Span.EqualTo("    - Write the value ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(".\n"))
            .Then(Span.EqualTo("    - Move one slot to the ").IgnoreThen(LeftOrRightParser).ThenIgnore(Span.EqualTo(".\n")))
            .Then(Span.EqualTo("    - Continue with state ").IgnoreThen(Character.Letter).ThenIgnore(Span.EqualTo(".\n")))
            .Select(x => new StateAction(x.Item1.Item1 != 0, x.Item1.Item2, x.Item2));
    private static readonly TextParser<StateDescriptor> StateDescriptorParser = StateDescriptorHeaderParser.Then(StateActionHeaderParser.Then(StateActionParser).Many())
        .Select(x => new StateDescriptor(x.Item1, x.Item2.Single(y => y.Item1 == 0).Item2, x.Item2.Single(y => y.Item1 == 1).Item2));

    protected override TextParser<Model> Parser { get; } = 
        StartStateParser
            .Then(StepsParser)
            .Then(StateDescriptorParser.Many())
            .Select(x => new Model(x.Item1.Item1, x.Item1.Item2, x.Item2.ToDictionary(y => y.State)));

    [Sample("Begin in state A.\nPerform a diagnostic checksum after 6 steps.\n\nIn state A:\n  If the current value is 0:\n    - Write the value 1.\n    - Move one slot to the right.\n    - Continue with state B.\n  If the current value is 1:\n    - Write the value 0.\n    - Move one slot to the left.\n    - Continue with state B.\n\nIn state B:\n  If the current value is 0:\n    - Write the value 1.\n    - Move one slot to the left.\n    - Continue with state A.\n  If the current value is 1:\n    - Write the value 1.\n    - Move one slot to the right.\n    - Continue with state A.\n", 3)]
    protected override int Part1(Model input)
    {
        var state = input.StartState;
        var head = 0;

        var tapePositive = new BitArray(input.Steps);
        var tapeNegative = new BitArray(input.Steps);

        var descriptors = Enumerable.Range(0, 255).Select(c => input.StateDescriptors.TryGetValue((char)c, out var descriptor) ? descriptor : null).ToArray();
        
        for (var i = 0; i < input.Steps; i++)
        {
            var descriptor = descriptors[state]!;

            var current = head < 0 ? tapeNegative[-head] : tapePositive[head];
            var action = current ? descriptor.CurrentIsOne : descriptor.CurrentIsZero;

            if (head < 0)
            {
                tapeNegative[-head] = action.ValueToWrite;
            }
            else
            {
                tapePositive[head] = action.ValueToWrite;
            }
            
            
            head += action.MoveRight ? 1 : -1;
            state = action.NextState;
        }
        
        var ints = new int[((tapePositive.Count >> 5) + 1) * 2];
        tapePositive.CopyTo(ints, 0);
        tapeNegative.CopyTo(ints, (tapePositive.Count >> 5) + 1);
        return ints.Sum(x => BitOperations.PopCount((uint)x));
    }

    protected override int Part2(Model input) => 0;

    public record Model(char StartState, int Steps, IReadOnlyDictionary<char, StateDescriptor> StateDescriptors);
    public record StateDescriptor(char State, StateAction CurrentIsZero, StateAction CurrentIsOne);
    public record StateAction(bool ValueToWrite, bool MoveRight, char NextState);
}
