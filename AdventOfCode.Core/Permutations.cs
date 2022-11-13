namespace AdventOfCode.Core;

public static class Permutations
{
    public static IEnumerable<IEnumerable<T>> Get<T>(IEnumerable<T> list) => Get(list, list.Count());

    public static IEnumerable<IEnumerable<T>> Get<T>(IEnumerable<T> list, int length)
    {
        if (length == 1)
        {
            return list.Select(t => new [] { t });
        }
        
        return Get(list, length - 1)
            .SelectMany(t => list.Where(o => !t.Contains(o)),
                (t1, t2) => t1.Concat(new [] { t2 }));
    }
}