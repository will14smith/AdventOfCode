namespace AdventOfCode.Core;

public static class OptimisedSearch
{
    public static TItem Solve<TItem, TPriority>(TItem initialItem, TPriority initialPriority, Search<TItem, TPriority> search)
    {
        return Solve(initialItem, initialPriority, search.IsGoal, search.Next, search.ShouldSkip, search.GetPriority);
    }
    
    public static TItem Solve<TItem, TPriority>(TItem initialItem, TPriority initialPriority, Func<TItem, bool> isGoal, Func<TItem, IEnumerable<TItem>> next, Func<TItem, bool> shouldSkip, Func<TItem, TPriority> getPriority)
    {
        var states = new PriorityQueue<TItem, TPriority>();
        states.Enqueue(initialItem, initialPriority);

        var visited = new HashSet<TItem>();

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

                if (visited.Add(nextItem))
                {
                    states.Enqueue(nextItem, getPriority(nextItem));
                }
            }
        }

        throw new Exception("solution not found");
    }

    public abstract class Search<TItem, TPriority>
    {
        public abstract IEnumerable<TItem> Next(TItem item);
        public abstract TPriority GetPriority(TItem item);

        public abstract bool IsGoal(TItem item);
        public abstract bool ShouldSkip(TItem item);
    }
    
    public abstract class Search<TItem> : Search<TItem, int>
    {
    }
}