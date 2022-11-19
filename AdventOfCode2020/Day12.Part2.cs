namespace AdventOfCode2020;

public partial class Day12
{
    [Sample("F10\nN3\nF7\nR90\nF11\n", 286)]
    protected override int Part2(Op[] input)
    {
        var state = input.Aggregate(StatePart2.Initial, Apply);

        return Math.Abs(state.Ship.X) + Math.Abs(state.Ship.Y);
    }
    
    private static StatePart2 Apply(StatePart2 state, Op instruction)
    {
        return instruction.Type switch
        {
            'F' => state.MoveShipToWaypoint(instruction.Value),

            'L' => state.RotateWaypoint(-instruction.Value),
            'R' => state.RotateWaypoint(instruction.Value),

            'N' => state.IncWaypointY(instruction.Value),
            'E' => state.IncWaypointX(instruction.Value),
            'S' => state.IncWaypointY(-instruction.Value),
            'W' => state.IncWaypointX(-instruction.Value),

            _ => throw new ArgumentOutOfRangeException($"instruction: {instruction.Type}")
        };
    }

    private record StatePart2(Position Ship, Position Waypoint)
    {
        public static readonly StatePart2 Initial = new(Position.Identity, new Position(10, 1));

        private static readonly IReadOnlyDictionary<int, (int, int)> Trig = new Dictionary<int, (int Cos, int Sin)>
        {
            { 0, (1, 0) },
            { 90, (0, -1) },
            { 180, (-1, 0) },
            { 270, (0, 1) },
        };

        public StatePart2 IncWaypointX(in int delta) => this with { Waypoint = Waypoint + new Position(delta, 0) };
        public StatePart2 IncWaypointY(in int delta) => this with { Waypoint = Waypoint + new Position(0, delta) };

        public StatePart2 RotateWaypoint(int delta)
        {
            var (cosDelta, sinDelta) = Trig[StatePart1.NormaliseHeading(delta)];

            var waypoint = new Position(
                Waypoint.X * cosDelta - Waypoint.Y * sinDelta, 
                Waypoint.X * sinDelta + Waypoint.Y * cosDelta);
            
            return this with { Waypoint = waypoint };
        }

        public StatePart2 MoveShipToWaypoint(in int count) => this with { Ship = Ship + Waypoint * count };
    }
}