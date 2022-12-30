namespace AdventOfCode.Core;

public static class OptimisedSearch
{
    public static TItem Solve<TItem, THash, TPriority>(TItem initialItem, Search<TItem, THash, TPriority> search)
    {
        return Solve(initialItem, search.IsGoal, search.Next, search.ShouldSkip, search.GetHash, search.GetPriority);
    }
    
    public static TItem Solve<TItem, THash, TPriority>(TItem initialItem, Func<TItem, bool> isGoal, Func<TItem, IEnumerable<TItem>> next, Func<TItem, bool> shouldSkip, Func<TItem, THash> getHash, Func<TItem, TPriority> getPriority)
    {
        var states = new PriorityQueue<TItem, TPriority>();
        states.Enqueue(initialItem, getPriority(initialItem));

        var visited = new HashSet<THash>();

        while (states.Count > 0)
        {
            var current = states.Dequeue();
            if (isGoal(current))
            {
                return current;
            }

            var nextItems = next(current);
            foreach (var nextItem in nextItems)
            {
                if (shouldSkip(nextItem))
                {
                    continue;
                }

                if (visited.Add(getHash(nextItem)))
                {
                    states.Enqueue(nextItem, getPriority(nextItem));
                }
            }
        }

        throw new Exception("solution not found");
    }

    public abstract class Search<TItem, THash, TPriority>
    {
        public abstract IEnumerable<TItem> Next(TItem item);
        public abstract THash GetHash(TItem item);
        public abstract TPriority GetPriority(TItem item);

        public abstract bool IsGoal(TItem item);
        public abstract bool ShouldSkip(TItem item);
    }
    
    public abstract class Search<TItem> : Search<TItem, TItem, int>
    {
        public override TItem GetHash(TItem item) => item;
    }
}