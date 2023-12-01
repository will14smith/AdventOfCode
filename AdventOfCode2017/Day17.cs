namespace AdventOfCode2017;

[Day]
public partial class Day17 : Day<Day17.Model, int, int>
{
    protected override Model Parse(string input) => new (int.Parse(input));

    [Sample("3", 638)]
    protected override int Part1(Model input)
    {
        var zero = new Entry { Value = 0 };
        zero.Next = zero;

        var zeroCounter = new Dictionary<int, int>();
        
        var head = zero; 
        
        for (var i = 1; i <= 2017; i++)
        {
            for (var s = 0; s < input.StepSize; s++)
            {
                head = head.Next;
            }

            var entry = new Entry{ Value = i, Next = head.Next };
            head.Next = entry;
            head = entry;

            if (zeroCounter.TryGetValue(zero.Next.Value, out var count))
            {
                zeroCounter[zero.Next.Value] = count + 1;
            }
            else
            {
                zeroCounter[zero.Next.Value] = 1;
            }
        }

        foreach (var keyValuePair in zeroCounter)
        {
            Output.WriteLine(keyValuePair.Key + ": " + keyValuePair.Value);
        }
        
        return head.Next.Value;
    }
    protected override int Part2(Model input)
    {
        var position = 0;
        var afterZero = 0;

        for (int i = 1; i < 50_000_000; i++)
        {
            position = (position + input.StepSize) % i + 1;
            if (position == 1) afterZero = i;
        }

        return afterZero;
    }

    public record Model(int StepSize);

    public class Entry
    {
        public int Value { get; init;  }
        public Entry Next { get; set; } = null!;
    }
}
