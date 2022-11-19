namespace AdventOfCode2020;

[Day]
public partial class Day12 : ParseDay<Day12.Op[], Day12.TokenType, int, int>
{
    [Sample("F10\nN3\nF7\nR90\nF11\n", 25)]
    protected override int Part1(Op[] input)
    {
        var state = input.Aggregate(StatePart1.Initial, Apply);

        return Math.Abs(state.X) + Math.Abs(state.Y);
    }
    
    private static StatePart1 Apply(StatePart1 state, Op instruction)
    {
        return instruction.Type switch
        {
            'F' => state.Heading switch
            {
                0 => state.IncY(instruction.Value),
                90 => state.IncX(instruction.Value),
                180 => state.IncY(-instruction.Value),
                270 => state.IncX(-instruction.Value),

                _ => throw new ArgumentOutOfRangeException($"heading: {state.Heading}")
            },

            'L' => state.Rotate(-instruction.Value),
            'R' => state.Rotate(instruction.Value),

            'N' => state.IncY(instruction.Value),
            'E' => state.IncX(instruction.Value),
            'S' => state.IncY(-instruction.Value),
            'W' => state.IncX(-instruction.Value),

            _ => throw new ArgumentOutOfRangeException($"instruction: {instruction.Type}")
        };
    }

    private record StatePart1(int X, int Y, int Heading)
    {
        public static readonly StatePart1 Initial = new(0, 0, 90);
        
        public StatePart1 IncX(in int delta) => this with { X = X + delta };
        public StatePart1 IncY(in int delta) => this with { Y = Y + delta };
        public StatePart1 Rotate(int delta) => this with { Heading = NormaliseHeading(Heading + delta) };

        internal static int NormaliseHeading(int heading) => heading < 0 ? heading % 360 + 360 : heading % 360;
    }
}