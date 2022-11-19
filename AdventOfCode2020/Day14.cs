namespace AdventOfCode2020;

[Day]
public partial class Day14 : ParseDay<Day14.Op[], Day14.TokenType, long, long>
{
    private const string Sample1 = "mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X\nmem[8] = 11\nmem[7] = 101\nmem[8] = 0\n";
    private const string Sample2 = "mask = 000000000000000000000000000000X1001X\nmem[42] = 100\nmask = 00000000000000000000000000000000X0XX\nmem[26] = 1\n";
    
    [Sample(Sample1, 165L)]
    protected override long Part1(Op[] program)
    {
        var mem = new Dictionary<long, long>();
        var mask = Op.SetMask.Clear;

        foreach (var op in program)
        {
            switch (op)
            {
                case Op.SetMask maskOp: mask = maskOp; break;
                case Op.SetMem memOp: mem[memOp.Addr] = (memOp.Value & mask.AndMask) | mask.OrMask; break;
                default: throw new ArgumentOutOfRangeException(nameof(op));
            }
        }

        return mem.Sum(x => x.Value);
    }

    [Sample(Sample2, 208L)]
    protected override long Part2(Op[] program)
    {
        var mem = new Dictionary<long, long>();
        var mask = Op.SetMask.Clear;

        foreach (var op in program)
        {
            switch (op)
            {
                case Op.SetMask maskOp: mask = maskOp; break;
                case Op.SetMem memOp:

                    var baseAddr = (memOp.Addr & ~mask.AndMask) | mask.OrMask;

                    var floatingMask = mask.AndMask & ~mask.OrMask & ((1L << 36) - 1);

                    foreach (var combination in EnumerateCombinations(floatingMask, 36))
                    {
                        var addr = baseAddr | combination;
                        mem[addr] = memOp.Value;
                    }

                    break;
                default: throw new ArgumentOutOfRangeException(nameof(op));
            }
        }

        return mem.Sum(x => x.Value);
    }
    
    private static IEnumerable<long> EnumerateCombinations(long floatingMask, int i)
    {
        if (i == 0)
        {
            return new[] { 0L };
        }

        var sub = EnumerateCombinations(floatingMask, i - 1).ToList();
        var iMask = 1L << (i - 1);

        if ((floatingMask & iMask) == 0)
        {
            return sub;
        }

        return sub.Concat(sub.Select(x => x | iMask));
    }
}
