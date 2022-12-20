namespace AdventOfCode2022;

[Day]
public partial class Day20 : LineDay<long, long, long>
{
    private const string Sample = "1\n2\n-3\n3\n-2\n0\n4";
    
    protected override long ParseLine(string input) => long.Parse(input);
    
    [Sample(Sample, 3L)]
    protected override long Part1(IEnumerable<long> input)
    {
        var items = BuildList(input);
        Mix(items);
        return GetSolution(items);
    }
    
    [Sample(Sample, 1623178306L)]
    protected override long Part2(IEnumerable<long> input)
    {
        var items = BuildList(input.Select(x => x * 811589153L));

        for (var i = 0; i < 10; i++)
        {
            Mix(items);
        }
        
        return GetSolution(items);
    }
    
    private static Item[] BuildList(IEnumerable<long> input)
    {
        var items = input.Select(x => new Item(x)).ToArray();
        
        for (var i = 0; i < items.Length; i++)
        {
            var next = i + 1 == items.Length ? items[0] : items[i + 1];
            var prev = i == 0 ? items[^1] : items[i - 1];

            items[i].OriginalNext = next;
            items[i].OriginalPrevious = prev;
            
            items[i].MixedNext = next;
            items[i].MixedPrevious = prev;
        }

        return items;
    }

    private static void Mix(Item[] items)
    {
        var iterator = items[0];
        do
        {
            // remove from list
            iterator.MixedPrevious.MixedNext = iterator.MixedNext;
            iterator.MixedNext.MixedPrevious = iterator.MixedPrevious;

            // find where to put it
            var insertAfter = iterator.Value switch
            {
                // use len-1 because we removed an item
                < 0 => ReverseNthMixedItem(iterator, -iterator.Value + 1, items.Length - 1),
                > 0 => NthMixedItem(iterator, iterator.Value, items.Length - 1),
                _ => iterator.MixedPrevious
            };

            // insert into position
            iterator.MixedNext = insertAfter.MixedNext;
            iterator.MixedPrevious = insertAfter;

            insertAfter.MixedNext = iterator;
            iterator.MixedNext.MixedPrevious = iterator;

            iterator = iterator.OriginalNext;
        } while (iterator != items[0]);
    }
    
    private static long GetSolution(Item[] items)
    {
        var zero = items.First(x => x.Value == 0);

        var a = NthMixedItem(zero, 1000, items.Length);
        var b = NthMixedItem(zero, 2000, items.Length);
        var c = NthMixedItem(zero, 3000, items.Length);

        return a.Value + b.Value + c.Value;
    }

    private static Item NthMixedItem(Item start, long offset, long count)
    {
        offset %= count;

        var iterator = start;
        while (offset-- > 0)
        {
            iterator = iterator.MixedNext;
        }

        return iterator;
    }
    
    private static Item ReverseNthMixedItem(Item start, long offset, long count)
    {
        offset %= count;

        var iterator = start;
        while (offset-- > 0)
        {
            iterator = iterator.MixedPrevious;
        }

        return iterator;
    }

    public class Item
    {
        public Item(long value)
        {
            Value = value;
        }

        public long Value { get; }
        
        public Item OriginalNext { get; set; }
        public Item OriginalPrevious { get; set; }
        
        public Item MixedNext { get; set; }
        public Item MixedPrevious { get; set; }
    }
}
