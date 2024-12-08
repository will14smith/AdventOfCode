namespace AdventOfCode.Core;

public static class Combinations
{
    public static IEnumerable<ReadOnlyMemory<T>> Get<T>(IReadOnlyList<T> list)
    {
        var combinations = Enumerable.Empty<ReadOnlyMemory<T>>();
        
        for (var i = 1; i <= list.Count; i++)
        {
            combinations = combinations.Concat(Get(list, i));
        }
    
        return combinations;
    }
    
    public static IEnumerable<ReadOnlyMemory<T>> Get<T>(IReadOnlyList<T> input, int outputLength)
    {
        var inputLength = input.Count;
        if (outputLength > inputLength)
        {
            yield break;
        }

        var indices = Enumerable.Range(0, outputLength).ToArray();

        yield return Build(input, indices);

        while (true)
        {
            var i = outputLength - 1;
            var didBreak = false;
            
            for (; i >= 0; i--)
            {
                if (indices[i] == i + inputLength - outputLength) continue;
                
                didBreak = true;
                break;
            }

            if (!didBreak)
            {
                yield break;
            }

            indices[i]++;
            
            for (var j = i + 1; j < outputLength; j++)
            {
                indices[j] = indices[j - 1] + 1;
            }  
            
            yield return Build(input, indices);
        }
        
        static ReadOnlyMemory<T> Build(IReadOnlyList<T> input, in ReadOnlySpan<int> indices)
        {
            var outputLength = indices.Length;
            var result = new T[outputLength];

            for (var i = 0; i < outputLength; i++)
            {
                result[i] = input[indices[i]];
            }
            
            return result;
        }
    }
}