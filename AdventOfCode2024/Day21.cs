namespace AdventOfCode2024;

[Day]
public partial class Day21 : LineDay<Day21.Model, long, long>
{
    public record Model(IReadOnlyList<char> Digits);

    protected override Model ParseLine(string input) => new(input.ToCharArray());

    private static readonly IReadOnlyDictionary<char, Position> DigitPositions = new Dictionary<char, Position>
    {
        ['7'] = new(0, 0), ['8'] = new(1, 0), ['9'] = new(2, 0),
        ['4'] = new(0, 1), ['5'] = new(1, 1), ['6'] = new(2, 1),
        ['1'] = new(0, 2), ['2'] = new(1, 2), ['3'] = new(2, 2),
        ['0'] = new(1, 3), ['A'] = new(2, 3),
    };

    public enum Action
    {
        Left,
        Right,
        Up,
        Down,
        Push,
    }
    
    private static readonly IReadOnlyDictionary<Action, Position> ActionPositions = new Dictionary<Action, Position>
    {
        [Action.Up] = new(1, 0), [Action.Push] = new(2, 0),
        [Action.Left] = new(0, 1), [Action.Down] = new(1, 1), [Action.Right] = new(2, 1),
    };

    [Sample("029A\n980A\n179A\n456A\n379A", 126384L)]
    protected override long Part1(IEnumerable<Model> input) => Solve(input, 2);
    
    protected override long Part2(IEnumerable<Model> input) => Solve(input, 25);

    private long Solve(IEnumerable<Model> input, long depth)
    {
        var digitMoves = BuildDigitMoves();
        var actionMoves = BuildActionMoves();
        
        var result = 0L;
        foreach (var model in input)
        {
            var state = 'A';
            var len = 0L;
            
            foreach (var next in model.Digits)
            {
                var movesForNext = digitMoves[(state, next)];
                var encodedActions = movesForNext.Select(x => EncodeActions(x, actionMoves, depth)).MinBy(x => x);
                len += encodedActions;

                state = next;
            }
            
            result += len * long.Parse(new string(model.Digits.Take(model.Digits.Count - 1).ToArray()));
        }

        return result;
    }

    private static Dictionary<(char From, char To), IReadOnlyList<IReadOnlyList<Action>>> BuildDigitMoves()
    {
        var moves = new Dictionary<(char From, char To), IReadOnlyList<IReadOnlyList<Action>>>();
        foreach (var from in DigitPositions.Keys)
        {
            var fromPosition = DigitPositions[from];
            foreach (var to in DigitPositions.Keys)
            {
                var toPosition = DigitPositions[to];
                var diff = toPosition - fromPosition;
                var x = Enumerable.Repeat(diff.X < 0 ? Action.Left : Action.Right, Math.Abs(diff.X)).ToArray();
                var y = Enumerable.Repeat(diff.Y < 0 ? Action.Up : Action.Down, Math.Abs(diff.Y)).ToArray();

                // (0, 3) is not a valid position, so don't allow movements through it 
                if (fromPosition.X == 0 && toPosition.Y == 3)
                {
                    moves[(from, to)] = [x.Concat(y).Append(Action.Push).ToArray()];
                }
                else if (toPosition.X == 0 && fromPosition.Y == 3)
                {
                    moves[(from, to)] = [y.Concat(x).Append(Action.Push).ToArray()];
                }
                else
                {
                    moves[(from, to)] = diff.X == 0 ? [y.Append(Action.Push).ToArray()] : diff.Y == 0 ? [x.Append(Action.Push).ToArray()] : [x.Concat(y).Append(Action.Push).ToArray(), y.Concat(x).Append(Action.Push).ToArray()];
                }
            }
        }

        return moves;
    }
    
    private static Dictionary<(Action From, Action To), IReadOnlyList<IReadOnlyList<Action>>> BuildActionMoves()
    {
        var moves = new Dictionary<(Action From, Action To), IReadOnlyList<IReadOnlyList<Action>>>();
        foreach (var from in ActionPositions.Keys)
        {
            var fromPosition = ActionPositions[from];
            foreach (var to in ActionPositions.Keys)
            {
                var toPosition = ActionPositions[to];
                var diff = toPosition - fromPosition;
                var x = Enumerable.Repeat(diff.X < 0 ? Action.Left : Action.Right, Math.Abs(diff.X)).ToArray();
                var y = Enumerable.Repeat(diff.Y < 0 ? Action.Up : Action.Down, Math.Abs(diff.Y)).ToArray();

                // (0, 0) is not a valid position, so don't allow movements through it 
                if (fromPosition.X == 0 && toPosition.Y == 0)
                {
                    moves[(from, to)] = [x.Concat(y).Append(Action.Push).ToArray()];
                }
                else if (toPosition.X == 0 && fromPosition.Y == 0)
                {
                    moves[(from, to)] = [y.Concat(x).Append(Action.Push).ToArray()];
                }
                else
                {
                    moves[(from, to)] = diff.X == 0 ? [y.Append(Action.Push).ToArray()] : diff.Y == 0 ? [x.Append(Action.Push).ToArray()] : [x.Concat(y).Append(Action.Push).ToArray(), y.Concat(x).Append(Action.Push).ToArray()];
                }
            }
        }

        return moves;
    }
    
    private readonly Dictionary<(string Key, long Depth), long> _cache = new();
    private long EncodeActions(IReadOnlyList<Action> actionsToEncode, Dictionary<(Action From, Action To), IReadOnlyList<IReadOnlyList<Action>>> movements, long depth)
    {
        if (depth == 0)
        {
            return actionsToEncode.Count;
        }

        var key = string.Create(actionsToEncode.Count, actionsToEncode, static (dst, actions) =>
        {
            for (var index = 0; index < actions.Count; index++)
            {
                var action = actions[index];
                dst[index] = action switch
                {
                    Action.Left => '<',
                    Action.Right => '>',
                    Action.Up => '^',
                    Action.Down => 'v',
                    Action.Push => 'A',
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        });
        if (_cache.TryGetValue((key, depth), out var cached))
        {
            return cached;
        }
        
        var len = 0L;
        
        var state = Action.Push;
        foreach (var action in actionsToEncode)
        {
            var movesForNext = movements[(state, action)];
            var encodedActions = movesForNext.Select(x => EncodeActions(x, movements, depth - 1)).MinBy(x => x);
            len += encodedActions;

            state = action;
        }

        _cache.Add((key, depth), len);
        
        return len;
    }
}