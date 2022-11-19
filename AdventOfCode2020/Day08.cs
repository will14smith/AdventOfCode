using System.Collections.Immutable;

namespace AdventOfCode2020;

[Day]
public partial class Day08 : ParseDay<ImmutableList<Day08.Op>, Day08.Token, int, int>
{
    private const string Sample = "nop +0\nacc +1\njmp +4\nacc +3\njmp -3\nacc -99\nacc +1\njmp -4\nacc +6";
    
    [Sample(Sample, 5)]
    protected override int Part1(ImmutableList<Op> input) => EvaluateUntilFirstRepeat(input).Acc;

    [Sample(Sample, 8)]
    protected override int Part2(ImmutableList<Op> input) => FixAndEvaluate(input).FinalAcc;

    private static State EvaluateUntilFirstRepeat(ImmutableList<Op> instructions)
    {
        var state = State.InitialFrom(instructions);
        var nextState = state;

        var visited = new HashSet<int>();
        do
        {
            state = nextState;
            nextState = Step(state);
        } while (visited.Add(state.IP));

        return state;
    }

    private static (int FinalAcc, int BrokenIP) FixAndEvaluate(ImmutableList<Op> instructions)
    {
        for (var i = 0; i < instructions.Count; i++)
        {
            var state = TryFixAndEvaluate(instructions, i);
            if (IsComplete(state))
            {
                return (state.Acc, i);
            }
        }

        throw new Exception("Failed to fix it");

        static bool IsComplete(State state) => state.IP >= state.Instructions.Count;
    }

    private static State TryFixAndEvaluate(ImmutableList<Op> instructions, int ip)
    {
        var op = instructions[ip] switch
        {
            Op.Nop nop => (Op) new Op.Jmp(nop.Value),
            Op.Acc acc => acc,
            Op.Jmp jmp => new Op.Nop(jmp.Value),

            _ => throw new ArgumentOutOfRangeException()
        };

        return EvaluateUntilFirstRepeat(instructions.SetItem(ip, op));
    }
}
