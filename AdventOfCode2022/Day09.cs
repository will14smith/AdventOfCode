using System.Collections.Immutable;

namespace AdventOfCode2022;

[Day]
public partial class Day09 : LineDay<Day09.Model, int, int>
{
    private const string Sample1 = "R 4\nU 4\nL 3\nD 1\nR 4\nD 1\nL 5\nR 2";
    private const string Sample2 = "R 5\nU 8\nL 8\nD 3\nR 17\nD 10\nL 25\nU 20";
    
    protected override Model ParseLine(string input)
    {
        var parts = input.Split(" ");
        var direction = parts[0] switch
        {
            "U" => Direction.Up,
            "D" => Direction.Down,
            "L" => Direction.Left,
            "R" => Direction.Right,
        };
        return new Model(direction, int.Parse(parts[1]));
    }

    [Sample(Sample1, 13)]
    protected override int Part1(IEnumerable<Model> input)
    {
        var state = new State(ImmutableHashSet<Position>.Empty, Position.Identity, Position.Identity);
        
        foreach (var model in input)
        {
            state = Apply(state, model.Direction, model.Amount);
        }

        return state.Visited.Count;
    }

    [Sample(Sample1, 1)]
    [Sample(Sample2, 36)]
    protected override int Part2(IEnumerable<Model> input)
    {
        var states = Enumerable.Range(1, 9).Select(_ => new State(ImmutableHashSet<Position>.Empty, Position.Identity, Position.Identity)).ToArray();
        
        foreach (var model in input)
        {
            for (var _ = 0; _ < model.Amount; _++)
            {
                states[0] = Apply(states[0], model.Direction);
                
                for (var stateIndex = 1; stateIndex < states.Length; stateIndex++)
                {
                    states[stateIndex] = Apply(states[stateIndex], states[stateIndex - 1].Tail);
                }
            } 
        }

        
        return states[^1].Visited.Count;
    }
    
    private State Apply(State state, Direction direction, int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            state = Apply(state, direction);
        }
        return state;
    }    
    
    private State Apply(State state, Direction direction)
    {
        var newHead = Apply(state.Head, direction);

        return Apply(state, newHead);
    }
    
    private static Position Apply(Position position, Direction direction) =>
        position + direction switch
        {
            Direction.Up => new Position(0, -1),
            Direction.Down => new Position(0, 1),
            Direction.Left => new Position(-1, 0),
            Direction.Right => new Position(1, 0),
        };

    private static readonly IReadOnlyDictionary<Position, Position> TailCatchUpDelta = new Dictionary<Position, Position>
    {
        // nop
        { new Position(0, 0), new Position(0, 0) },
        { new Position(1, -1), new Position(0, 0) },
        { new Position(0, -1), new Position(0, 0) },
        { new Position(-1, -1), new Position(0, 0) },
        { new Position(1, 0), new Position(0, 0) },
        { new Position(-1, 0), new Position(0, 0) },
        { new Position(1, 1), new Position(0, 0) },
        { new Position(0, 1), new Position(0, 0) },
        { new Position(-1, 1), new Position(0, 0) },

        // same line
        { new Position(2, 0), new Position(1, 0) },
        { new Position(-2, 0), new Position(-1, 0) },
        { new Position(0, 2), new Position(0, 1) },
        { new Position(0, -2), new Position(0, -1) },
        
        // diagonal
        { new Position(2, 1), new Position(1, 1) },
        { new Position(2, -1), new Position(1, -1) },
        { new Position(-2, 1), new Position(-1, 1) },
        { new Position(-2, -1), new Position(-1, -1) },
        { new Position(1, 2), new Position(1, 1) },
        { new Position(1, -2), new Position(1, -1) },
        { new Position(-1, 2), new Position(-1, 1) },
        { new Position(-1, -2), new Position(-1, -1) },
        
        // big diagonal
        { new Position(2, 2), new Position(1, 1) },
        { new Position(2, -2), new Position(1, -1) },
        { new Position(-2, 2), new Position(-1, 1) },
        { new Position(-2, -2), new Position(-1, -1) },
    };
    
    private static State Apply(State state, Position newHead)
    {
        var delta = newHead - state.Tail;
        var newTail = state.Tail + TailCatchUpDelta[delta];

        return new State(state.Visited.Add(newTail), newHead, newTail);
    }

    private record State(ImmutableHashSet<Position> Visited, Position Head, Position Tail);
    
    public record Model(Direction Direction, int Amount);

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }
}
