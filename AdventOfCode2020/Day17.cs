using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2020;

[Day]
public partial class Day17 : ParseDay<Day17.Model, int, int>
{
    public const string Sample = ".#.\n..#\n###";
    
    private static readonly TextParser<bool> Cell = Character.In('#','.').Select(x => x == '#');
    private static readonly TextParser<bool[]> Row = Cell.AtLeastOnce();
    private static readonly TextParser<bool[][]> Slice = Row.ManyDelimitedBy(SuperpowerExtensions.NewLine);

    protected override TextParser<Model> Parser => Slice.Select(x => new Model(x));

    [Sample(Sample, 112)]
    protected override int Part1(Model input) => Solve(input.Slice, false);

    [Sample(Sample, 848)]
    protected override int Part2(Model input) => Solve(input.Slice, true);

    public record Model(bool[][] Slice);
    
    private static int Solve(bool[][] input, bool includeW)
    {
        var state = BuildInitialState(input);

        for (var i = 0; i < 6; i++)
        {
            state = Step(state, includeW);
        }

        return state.Active.Count;
    }
    
    private static State BuildInitialState(bool[][] input)
    {
        var active = new HashSet<Loc>();

        for (var y = 0; y < input.Length; y++)
        for (var x = 0; x < input[y].Length; x++)
        {
            if (!input[y][x]) continue;

            var loc = new Loc(x, y, 0, 0);
            active.Add(loc);
        }

        return new State(active);
    }
    
    private static State Step(State state, bool includeW)
    {
        var next = new HashSet<Loc>(state.Active);

        var minW = includeW ? state.Min.W - 1 : 0;
        var maxW = includeW ? state.Max.W + 1 : 0;
        
        for (var w = minW; w <= maxW; w++)
        for (var z = state.Min.Z - 1; z <= state.Max.Z + 1; z++)
        for (var y = state.Min.Y - 1; y <= state.Max.Y + 1; y++)
        for (var x = state.Min.X - 1; x <= state.Max.X + 1; x++)
        {
            var loc = new Loc(x, y, z, w);

            var isActive = state.Active.Contains(loc);
            var count = state.CountAdjacent(loc);

            if (count is not (2 or 3) && isActive)
            {
                next.Remove(loc);
            }
            else if (count == 3 && !isActive)
            {
                next.Add(loc);
            }
        }

        return new State(next);
    }
    
    private record Loc(int X, int Y, int Z, int W);
    private class State
    {
        public State(ISet<Loc> active)
        {
            Active = active;

            var (min, max) = CalculateBounds(active);                

            Min = min;
            Max = max;
        }
        
        public Loc Min { get; }
        public Loc Max { get; }
        public ISet<Loc> Active { get; }

        public int CountAdjacent(Loc loc)
        {
            var (x, y, z, w) = loc;
            var count = 0;

            for (var dw = -1; dw <= 1; dw++)
            for (var dz = -1; dz <= 1; dz++)
            for (var dy = -1; dy <= 1; dy++)
            for (var dx = -1; dx <= 1; dx++)
            {
                if (dw == 0 && dz == 0 && dy == 0 && dx == 0) continue;

                count += Active.Contains(new Loc(x + dx, y + dy, z + dz, w + dw)) ? 1 : 0;
            }

            return count;
        }
        
        private static (Loc Min, Loc Max) CalculateBounds(ISet<Loc> active)
        {
            var min = new Loc(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
            var max = new Loc(int.MinValue, int.MinValue, int.MinValue, int.MinValue);

            foreach (var loc in active)
            {
                UpdateBounds(ref min, ref max, loc);
            }
            
            return (min, max);
        }

        private static void UpdateBounds(ref Loc min, ref Loc max, Loc loc)
        {
            var (x, y, z, w) = loc;
            min = new Loc(X: Math.Min(x, min.X), Y: Math.Min(y, min.Y), Z: Math.Min(z, min.Z), W: Math.Min(w, min.W));
            max = new Loc(X: Math.Max(x, max.X), Y: Math.Max(y, max.Y), Z: Math.Max(z, max.Z), W: Math.Max(w, max.W));
        }
    }
}
