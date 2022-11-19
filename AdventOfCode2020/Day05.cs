using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2020;

[Day]
public partial class Day05 : ParseLineDay<Day05.Model, int, int>
{
    private static readonly TextParser<int> RowChar = Character.In('B','F').Select(x => x == 'B' ? 1 : 0);
    private static readonly TextParser<int> ColChar = Character.In('L','R').Select(x => x == 'R' ? 1 : 0);
    private static readonly TextParser<int> Row = RowChar.Repeat(7).Select(x => x.Aggregate(0, (a, x) => (a << 1) | x));
    private static readonly TextParser<int> Col = ColChar.Repeat(3).Select(x => x.Aggregate(0, (a, x) => (a << 1) | x));
    private static readonly TextParser<Model> Seat = Row.Then(r => Col.Select(c => new Model(r, c)));

    protected override TextParser<Model> LineParser => Seat;

    [Sample("BFFFBBFRRR\nFFFBBBFRRR\nBBFFBBFRLL", 820)]
    protected override int Part1(IEnumerable<Model> input) => input.Max(GetSeatId);

    protected override int Part2(IEnumerable<Model> input) => FindMySeatId(input);

    private static int FindMySeatId(IEnumerable<Model> input)
    {
        var seats = input.Select(GetSeatId).ToHashSet();
        
        var minSeatId = seats.Min();
        var maxSeatId = seats.Max();

        for (var seatId = minSeatId; seatId <= maxSeatId; seatId++)
        {
            if (seats.Contains(seatId))
            {
                continue;
            }

            if (seats.Contains(seatId - 1) && seats.Contains(seatId + 1))
            {
                return seatId;
            }
        }

        throw new Exception("Couldn't find seat");
    }

    private static int GetSeatId(Model pos) => pos.Row * 8 + pos.Col;
    
    public record Model(int Row, int Col);

}
