namespace AdventOfCode.Core;

public static class CycleDetection
{
    public static (int Offset, int Length) Detect<T>(Func<T> initialStateFn, Func<T, T> applyFn, Func<T, T, bool> equalFn)
    {
        // https://en.wikipedia.org/wiki/Cycle_detection#Floyd's_tortoise_and_hare
        
        var tortoise = initialStateFn();
        var hare = initialStateFn();

        do
        {
            tortoise = applyFn(tortoise);
            hare = applyFn(applyFn(hare));
        } while (!equalFn(tortoise, hare));

        var offset = 0;
        tortoise = initialStateFn();
        while (!equalFn(tortoise, hare))
        {
            tortoise = applyFn(tortoise);
            hare = applyFn(hare);
            offset++;
        }

        var length = 1;
        hare = applyFn(tortoise);
        while (!equalFn(tortoise, hare))
        {
            hare = applyFn(hare);
            length++;
        }

        return (offset, length);
    }
}