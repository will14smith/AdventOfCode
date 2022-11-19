using System.Collections.Immutable;

namespace AdventOfCode2020;

public partial class Day08
{
    private static State Step(State state)
    {
        if (state.IP >= state.Instructions.Count)
        {
            return state;
        }

        var instruction = state.Instructions[state.IP];

        return instruction switch
        {
            Op.Nop => state.IncIP(1),
            Op.Acc acc => state.IncAcc(acc.Value).IncIP(1),
            Op.Jmp jmp => state.IncIP(jmp.Value),

            _ => throw new ArgumentOutOfRangeException(nameof(instruction))
        };
    }

    private record State(ImmutableList<Op> Instructions, int IP, int Acc)
    {
        public static State InitialFrom(ImmutableList<Op> instructions) => new(instructions, 0, 0);

        public State IncIP(int delta) => this with { IP = IP + delta };
        public State IncAcc(int delta) => this with { Acc = Acc + delta };
    }
}