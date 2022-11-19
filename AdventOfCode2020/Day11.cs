using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2020;

[Day]
public partial class Day11 : ParseDay<Day11.State[,], long, long>
{
    private const string Sample = "L.LL.LL.LL\nLLLLLLL.LL\nL.L.L..L..\nLLLL.LL.LL\nL.LL.LL.LL\nL.LLLLL.LL\n..L.L.....\nLLLLLLLLLL\nL.LLLLLL.L\nL.LLLLL.LL";
    
    private static readonly TextParser<State> Seat = Character.In('L','.').Select(x => x == 'L' ? State.Empty : State.NoSeat);
    private static readonly TextParser<State[]> Row = Seat.AtLeastOnce();
    private static readonly TextParser<State[,]> Room = Row.ManyDelimitedBy(SuperpowerExtensions.NewLine).Select(rows => rows.Combine());
    
    private static readonly IReadOnlyCollection<(int DeltaRow, int DeltaCol)> AdjacencyVectors = new[] { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };
    
    protected override TextParser<State[,]> Parser => Room;

    [Sample(Sample, 37L)]
    protected override long Part1(State[,] seats)
    {
        var room = RoomState.Create(seats, CalculateAdj(seats, CalculateAdjPart1), 4);

        return CountStableOccupiedSeats(room);
    }

    [Sample(Sample, 26L)]
    protected override long Part2(State[,] seats)
    {
        var room = RoomState.Create(seats, CalculateAdj(seats, CalculateAdjPart2), 5);

        return CountStableOccupiedSeats(room);
    }

    private static ReadOnlyMemory<(int, int)> CalculateAdjPart1(State[,] seats, int r, int c)
    {
        var rowCount = seats.GetLength(0);
        var colCount = seats.GetLength(1);
        
        return AdjacencyVectors.Select(x => (R: r + x.DeltaRow, C: c + x.DeltaCol))
            .Where(x => x.R >= 0 && x.R < rowCount && x.C >= 0 && x.C < colCount)
            .Where(x => seats[x.R, x.C] != State.NoSeat).ToArray();
    }

    private static ReadOnlyMemory<(int R, int C)> CalculateAdjPart2(State[,] seats, int r, int c)
    {
        var cellAdj = new List<(int R, int C)>();

        foreach (var (deltaRow, deltaCol) in AdjacencyVectors)
        {
            TryFindAdjSeat2(seats, r, c, deltaRow, deltaCol, cellAdj);
        }

        return cellAdj.ToArray();
    }

    private static void TryFindAdjSeat2(State[,] seats, int r, int c, int dr, int dc, List<(int R, int C)> res)
    {
        r += dr;
        c += dc;

        var rowCount = seats.GetLength(0);
        var colCount = seats.GetLength(1);
        
        while (r >= 0 && r < rowCount && c >= 0 && c < colCount)
        {
            if (seats[r, c] != State.NoSeat)
            {
                res.Add((r, c));
                break;
            }

            r += dr;
            c += dc;
        }
    }


    private static ReadOnlyMemory<(int R, int C)> CalculateAdj(State[,] seats, Func<State[,], int, int, ReadOnlyMemory<(int R, int C)>> calculateAdj)
    {
        var rowCount = seats.GetLength(0);
        var colCount = seats.GetLength(1);

        var adjArr = new (int R, int C)[rowCount * colCount * 8];
        Array.Fill(adjArr, (-1, -1));
        var adj = adjArr.AsMemory();
        
        for (var r = 0; r < rowCount; r++)
        {
            for (var c = 0; c < colCount; c++)
            {
                var seatAdj = calculateAdj(seats, r, c);
                seatAdj.CopyTo(adj.Slice((r * colCount + c) * 8, 8));
            }
        }

        return adj;
    }

    private static long CountStableOccupiedSeats(RoomState room)
    {
        var changed = true;
        while (changed)
        {
            (room, changed) = Next(room);
        }

        return room.CountOccupied();
    }

    private static (RoomState Next, bool Changed) Next(RoomState input)
    {
        var changed = false;
        
        var seats = input.Seats.Span;
        var next = new State[input.Seats.Length];

        for (var i = 0; i < next.Length; i++)
        {
                next[i] = Next(input, i);
                changed |= seats[i] != next[i];
        }

        return (input.WithSeats(next), changed);
    }

    private static State Next(in RoomState input, in int i)
    {
        var seats = input.Seats.Span;
        
        var occ = 0;
        foreach (var iAdj in input.GetAdjacent(i))
        {
            if (iAdj < 0)
            {
                break;
            }
            
            occ += seats[iAdj] == State.Occupied ? 1 : 0;
        }

        if (occ == 0) return State.Occupied;
        if (occ >= input.Tolerance) return State.Empty;
        return seats[i];
    }

    private class RoomState
    {
        private readonly ReadOnlyMemory<int> _adjacent;

        public ReadOnlyMemory<State> Seats { get; }
        public int Tolerance { get; }

        private RoomState(ReadOnlyMemory<State> seats, ReadOnlyMemory<int> adjacent, int tolerance)
        {
            Seats = seats;
            _adjacent = adjacent;
            Tolerance = tolerance;
        }

        public static RoomState Create(State[,] seats, ReadOnlyMemory<(int R, int C)> adjacent, int tolerance)
        {
            var seatMap = new Dictionary<(int, int), int>();
            var newSeats = new List<State>();
            var newAdj = new List<int>();

            var rowCount = seats.GetLength(0);
            var colCount = seats.GetLength(1);
            
            for (var r = 0; r < rowCount; r++)
            {
                for (var c = 0; c < colCount; c++)
                {
                    if (seats[r, c] != State.NoSeat)
                    {
                        var i = seatMap.Count;
                        seatMap.Add((r, c), i);
                        newSeats.Add(seats[r, c]);
                    }
                }
            }

            foreach (var ((r, c), i) in seatMap)
            {
                for (var ai = 0; ai < 8; ai++)
                {
                    var (ar, ac) = adjacent.Span[(r * colCount + c) * 8 + ai];
                    if (seatMap.TryGetValue((ar, ac), out var ax))
                    {
                        newAdj.Add(ax);
                    }
                }
                
                while(newAdj.Count % 8 != 0) newAdj.Add(-1);
            }
            
            return new RoomState(newSeats.ToArray(), newAdj.ToArray(), tolerance);
        }
        
        public ReadOnlySpan<int> GetAdjacent(int i)
        {
            return _adjacent.Span.Slice(i * 8, 8);
        }

        public RoomState WithSeats(ReadOnlyMemory<State> next)
        {
            return new(next, _adjacent, Tolerance);
        }

        public long CountOccupied()
        {
            var c = 0;
            
            foreach (var state in Seats.Span)
            {
                c += state == State.Occupied ? 1 : 0;
            }

            return c;
        }
    }
    
    public enum State
    {
        NoSeat,
        Empty,
        Occupied
    }
}
