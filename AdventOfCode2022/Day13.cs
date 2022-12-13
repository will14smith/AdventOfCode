using Superpower;

namespace AdventOfCode2022;

[Day]
public partial class Day13 : ParseDay<Day13.Model, Day13.TokenType, int, int>
{
    private const string Sample = "[1,1,3,1,1]\n[1,1,5,1,1]\n\n[[1],[2,3,4]]\n[[1],4]\n\n[9]\n[[8,7,6]]\n\n[[4,4],4,4]\n[[4,4],4,4,4]\n\n[7,7,7,7]\n[7,7,7]\n\n[]\n[3]\n\n[[[]]]\n[[]]\n\n[1,[2,[3,[4,[5,6,7]]]],8,9]\n[1,[2,[3,[4,[5,6,0]]]],8,9]";
    
    [Sample(Sample, 13)]
    protected override int Part1(Model input) => input.Packets.Select((x, i) => (Index: i + 1, Pair: x)).Where(x => PacketComparer.Instance.Compare(x.Pair.Item1, x.Pair.Item2) <= 0).Sum(x => x.Index);

    [Sample(Sample, 140)]
    protected override int Part2(Model input)
    {
        var dividers = Parse("[[2]]\n[[6]]");
        var packets = input.Packets.Concat(dividers.Packets).SelectMany(x => new[] { x.Item1, x.Item2 });
        var sorted = packets.OrderBy(x => x, PacketComparer.Instance).ToList();

        var index1 = sorted.IndexOf(dividers.Packets[0].Item1) + 1;
        var index2 = sorted.IndexOf(dividers.Packets[0].Item2) + 1;

        return index1 * index2;
    }

    private class PacketComparer : IComparer<Packet>
    {
        public static readonly PacketComparer Instance = new();
        
        // Less than zero:    left < right
        // Less than zero:    left = right
        // Greater than zero: left > right
        public int Compare(Packet? left, Packet? right) => Compare(left!.Data, right!.Data);
        private int Compare(Data left, Data right)
        {
            return left switch
            {
                Data.Number leftNumber => right switch
                {
                    Data.Number rightNumber => leftNumber.Value - rightNumber.Value,
                    Data.Array rightArray => CompareArray(new Data.Array(new[] { leftNumber }), rightArray),
                    _ => throw new ArgumentOutOfRangeException(nameof(right))
                },
            
                Data.Array leftArray => right switch
                {
                    Data.Number rightNumber => CompareArray(leftArray, new Data.Array(new[] { rightNumber })),
                    Data.Array rightArray => CompareArray(leftArray, rightArray),
                    _ => throw new ArgumentOutOfRangeException(nameof(right))
                },
            
                _ => throw new ArgumentOutOfRangeException(nameof(left))
            };
        }

        private int CompareArray(Data.Array left, Data.Array right)
        {
            for (var i = 0; i < left.Items.Count; i++)
            {
                if (i >= right.Items.Count)
                {
                    return 1;
                }
            
                var comparison = Compare(left.Items[i], right.Items[i]);
                if (comparison != 0)
                {
                    return comparison;
                }
            }

            return left.Items.Count == right.Items.Count ? 0 : -1;
        }
    }
    
    public record Model(IReadOnlyList<(Packet, Packet)> Packets);
    public record Packet(Data Data);
    public abstract record Data
    {
        public record Array(IReadOnlyList<Data> Items) : Data;
        public record Number(int Value) : Data;
    }
}
