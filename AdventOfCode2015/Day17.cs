namespace AdventOfCode2015;

[Day]
public partial class Day17 : LineDay<int, int, int>
{
    protected override int ParseLine(string input) => int.Parse(input);
    
    // [Sample("20\n15\n10\n5\n5", 4)] // set total to 25
    protected override int Part1(IEnumerable<int> input)
    {
        var total = 150;
        var buckets = input.OrderByDescending(x => x).ToArray();

        return Solve(buckets, total).Solutions;
    }
    
    // [Sample("20\n15\n10\n5\n5", 3)] // set total to 25
    protected override int Part2(IEnumerable<int> input)
    {
        var total = 150;
        var buckets = input.OrderByDescending(x => x).ToArray();
        var minBuckets = Solve(buckets, total).MinBuckets;

        var bucketCombinations = Combinations.Get<int>(buckets, minBuckets);
        return bucketCombinations.Sum(x => Solve(x.ToArray(), total).Solutions);
    }
    
    private static (int MinBuckets, int Solutions) Solve(ReadOnlySpan<int> buckets, int remaining)
    {
        if (buckets.Length == 0)
        {
            return (1000, 0);
        }
        
        var bucketNotUsed = Solve(buckets[1..], remaining);
        
        var head = buckets[0];
        if (remaining < head)
        {
            return bucketNotUsed;
        }
        if (remaining == head)
        {
            return (1, 1 + bucketNotUsed.Solutions);
        }

        if (buckets.Length == 1)
        {
            return head == remaining ? (1, 1) : (1000, 0);
        }
        
        var bucketUsed = Solve(buckets[1..], remaining - head);

        return (Math.Min(bucketUsed.MinBuckets + 1, bucketNotUsed.MinBuckets), bucketUsed.Solutions + bucketNotUsed.Solutions);
    }
}