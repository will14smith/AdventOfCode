namespace AdventOfCode.Core;

public static class Permutations
{
    public static IEnumerable<IEnumerable<T>> Get<T>(IReadOnlyList<T> input) => Get(input, input.Count);

    public static IEnumerable<IEnumerable<T>> Get<T>(IReadOnlyList<T> input, int outputLength)
    {
        var inputLength = input.Count;
        if (inputLength <= 0)
        {
            yield break;
        }
        
        if (outputLength > inputLength)
        {
            yield break;
        }


        var indices = Enumerable.Range(0, inputLength).ToArray();
        var cycles = Enumerable.Range(inputLength - outputLength + 1, outputLength).Reverse().ToArray();

        yield return Build(input, indices, outputLength);

        while (true)
        {
            var isFinished = true;
            
            for (var i = outputLength - 1; i >= 0; i--)
            {
                cycles[i]--;
                if (cycles[i] == 0)
                {
                    var tmp = indices[i];
                    for (var j = i + 1; j < indices.Length; j++)
                    {
                        indices[j - 1] = indices[j];
                    }
                    indices[^1] = tmp;
                    
                    cycles[i] = inputLength - i;
                }
                else
                {
                    var j = cycles[i];
                    (indices[i], indices[^j]) = (indices[^j], indices[i]);
                    yield return Build(input, indices, outputLength);
                    isFinished = false;
                    break;
                }
            }

            if (isFinished)
            {
                yield break;
            }
        }
        
        static IEnumerable<T> Build(IReadOnlyList<T> input, in ReadOnlySpan<int> indices, int outputLength)
        {
            var result = new T[outputLength];

            for (var i = 0; i < outputLength; i++)
            {
                result[i] = input[indices[i]];
            }
            
            return result;
        }
    }
}