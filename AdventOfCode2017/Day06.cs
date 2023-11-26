namespace AdventOfCode2017;

[Day]
public partial class Day06 : Day<Day06.Model, int, int>
{
    protected override Model Parse(string input) => new(input.Split('\t').Select(int.Parse).ToList());

    [Sample("0\t2\t7\t0", 5)]
    protected override int Part1(Model input)
    {
        var (offset, length) = CycleDetection.Detect(() => input.Blocks.ToArray(), Redistribute, StateEquals);
        return offset + length;
    }
    
    [Sample("2\t4\t1\t2", 4)]
    protected override int Part2(Model input) => CycleDetection.Detect(() => input.Blocks.ToArray(), Redistribute, StateEquals).Length;

    private static int[] Redistribute(int[] blocks)
    {
        var newBlocks = blocks.ToArray();
        
        // find highest
        var max = newBlocks.Max();
        var maxIndex = Array.IndexOf(newBlocks, max);
            
        // re-distribute
        var (all, rem) = Math.DivRem(max, newBlocks.Length);
            
        newBlocks[maxIndex] = 0;
        for (var i = 0; i < newBlocks.Length; i++)
        {
            newBlocks[i] += all;
        }
            
        for (var i = 0; i < rem; i++)
        {
            newBlocks[(maxIndex+1+i) % newBlocks.Length]++;
        }

        return newBlocks;
    }
    
    private static bool StateEquals(int[] a, int[] b) => string.Join(",", a) == string.Join(",", b);

    public record Model(IReadOnlyList<int> Blocks);
}
