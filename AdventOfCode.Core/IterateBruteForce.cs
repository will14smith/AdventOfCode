using System.Collections;
using System.Collections.Concurrent;

namespace AdventOfCode.Core;

public static class IterateBruteForce
{
    public static int SolveMin(Func<int, bool> loop)
    {
        var results = new ConcurrentBag<int>();

        Parallel.ForEach(new Iterator(), (i, state) =>
        {
            var result = loop(i);
            if (result)
            {
                state.Break();
                results.Add(i);
            }
        });
        
        return results.Min();
    }
    
    public static int SolveMin<TLocal>(Func<TLocal> init, Func<int, TLocal, bool> loop)
    {
        var results = new ConcurrentBag<int>();

        Parallel.ForEach(new Iterator(), init, (i, state, local) =>
        {
            var result = loop(i, local);
            if (result)
            {
                state.Break();
                results.Add(i);
            }

            return local;
        }, _ => { });
        
        return results.Min();
    }
    
    private class Iterator : IEnumerable<int>, IEnumerator<int>
    {
        public IEnumerator<int> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool MoveNext()
        {
            Current++;
            return true;
        }

        public void Reset()
        {
            Current = 0;
        }

        public int Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose() { }
    }

}