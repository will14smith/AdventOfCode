namespace AdventOfCode.Core;

public static class Product
{
    public static IEnumerable<(T1, T2)> Get<T1, T2>(IReadOnlyCollection<T1> items1, IReadOnlyCollection<T2> items2)
    {
        foreach (var a in items1)
        {
            foreach (var b in items2)
            {
                yield return (a, b);
            }
        }
    }   
    
    public static IEnumerable<TResult> Get<T1, T2, TResult>(IReadOnlyCollection<T1> items1, IReadOnlyCollection<T2> items2, Func<T1, T2, TResult> selector)
    {
        foreach (var a in items1)
        {
            foreach (var b in items2)
            {
                yield return selector(a, b);
            }
        }
    }
}