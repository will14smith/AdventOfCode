namespace AdventOfCode.Core;

public static class ArrayExtensions
{
    public static unsafe T[,] Combine<T>(this T[][] input) where T : unmanaged
    {
        var output = new T[input.Length, input[0].Length];
        var length = output.GetLength(0) * output.GetLength(1);
            
        fixed (T* p = output)
        {
            var span = new Span<T>(p, length);
            
            var offset = 0;
            foreach (var line in input)
            {
                line.CopyTo(span.Slice(offset));
                offset += line.Length;
            }
        }

        return output;
    }

    public static IEnumerable<T> ToEnumerable<T>(this T[,] input)
    {
        var dim1 = input.GetLength(0);
        var dim2 = input.GetLength(1);
            
        for (var i = 0; i < dim1; i++)
        {
            for (var j = 0; j < dim2; j++)
            {
                yield return input[i, j];
            }
        }
    }

    public static void UpdateTop<T>(this Stack<T> stack, Func<T, T> updateFn) => stack.Push(updateFn(stack.Pop()));

    public static string Join<T>(this IEnumerable<T> input) => string.Join("", input);
    public static string Join<T>(this IEnumerable<T> input, string sep) => string.Join(sep, input);

    public static IReadOnlyDictionary<T, int> ToFrequency<T>(this IEnumerable<T> input) where T : notnull => input.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
}