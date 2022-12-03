namespace AdventOfCode.Core;

public static class BatchExtensions
{
    public static IEnumerable<(T A, T B)> BatchesOf2<T>(this IEnumerable<T> items) => BatchesOfN(items, 2).Select(x => (x[0], x[1]));
    public static IEnumerable<(T A, T B, T C)> BatchesOf3<T>(this IEnumerable<T> items) => BatchesOfN(items, 3).Select(x => (x[0], x[1], x[2]));

    public static IEnumerable<T[]> BatchesOfN<T>(this IEnumerable<T> items, int n)
    {
        var currentBatch = new T[n];
        var currentBatchOffset = 0;
        
        foreach (var item in items)
        {
            currentBatch[currentBatchOffset++] = item;
            
            if (currentBatchOffset == n)
            {
                yield return currentBatch;
                currentBatch = new T[n];
                currentBatchOffset = 0;
            }
        }

        if (currentBatchOffset != 0)
        {
            throw new Exception("List doesn't divide up nicely");
        }
    }
}