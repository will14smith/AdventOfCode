namespace AdventOfCode.Core;

public static class Combinations
{
    public static IEnumerable<IEnumerable<T>> Get<T>(ReadOnlySpan<T> list)
    {
        var combinations = Enumerable.Empty<IEnumerable<T>>();
        
        for (var i = 1; i <= list.Length; i++)
        {
            combinations = combinations.Concat(Get(list, i));
        }
    
        return combinations;
    }
    
    //
    // public static IEnumerable<IEnumerable<T>> Get<T>(IEnumerable<T> list, int length) where T : IComparable
    // {
    //     if (length == 1)
    //     {
    //         return list.Select(t => new [] { t });
    //     }
    //     
    //     return Get(list, length - 1)
    //         .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0), 
    //             (t1, t2) => t1.Concat(new [] { t2 }));
    // }

    public static IEnumerable<IEnumerable<T>> Get<T>(ReadOnlySpan<T> list, int length)
    {
        if (length == 0)
        {
            return new[] { Array.Empty<T>() };
        }

        if (list.Length == 0)
        {
            return Array.Empty<IEnumerable<T>>();
        }

        var head = list[0];
        var sub = Get(list[1..], length - 1);
        var headPlusSub = sub.Select(x => x.Prepend(head));

        var comb = Get(list[1..], length);

        return headPlusSub.Concat(comb);
    }
}