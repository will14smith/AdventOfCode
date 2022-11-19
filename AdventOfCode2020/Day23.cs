using System.Text;

namespace AdventOfCode2020;

[Day]
public partial class Day23 : Day<Day23.Model, string, string>
{
    protected override Model Parse(string input) => new(input.Select(x => x - (byte) '0').ToList());

    [Sample("389125467", "67384529")]
    protected override string Part1(Model input)
    {
        var nodes = input.Digits.Select(x => new L(x)).ToList();
        var finalNode = FindNode1AfterNIterations(nodes, 100);

        var output = new StringBuilder();

        var node = finalNode.Next;
        while (node != finalNode)
        {
            output.Append(node.Value);
            node = node.Next;
        }
            
        return output.ToString();
    }

    [Sample("389125467", "149245887792")]
    protected override string Part2(Model input)
    {
        var digits = input.Digits;

        var nodes = new int[1_000_001];
        nodes[0] = digits[0];
        for (var index = 0; index < digits.Count; index++)
        {
            if (index == 0)
            {
                nodes[1_000_000] = digits[index];
            }
            else
            {
                nodes[digits[index - 1]] = digits[index];
            }
        }

        nodes[digits[^1]] = digits.Count + 1;

        for (var index = digits.Count + 2; index < nodes.Length; index++)
        {
            nodes[index - 1] = index;
        }
            
        FindNode1AfterNIterations(nodes, 10_000_000);

        var first = nodes[1];
        var second = nodes[first];
            
        return $"{(long) first * second}";
    }

    private static L FindNode1AfterNIterations(IReadOnlyList<L> nodes, int iterations)
    {
        var max = nodes.Max(x => x.Value);
        var indexedNodes = nodes.ToDictionary(x => x.Value);
        for (var i = 0; i < nodes.Count; i++)
        {
            if (i == 0) nodes[^1].Next = nodes[i];
            else nodes[i - 1].Next = nodes[i];
        }

        var current = nodes[0];
        for (var t = 0; t < iterations; t++)
        {
            var pickup = current.Next;
            var pickupValues = new[] {pickup.Value, pickup.Next.Value, pickup.Next.Next.Value};

            current.Next = pickup.Next.Next.Next;

            var dst = current.Value;
            do
            {
                dst = dst == 1 ? max : dst - 1;
            } while (pickupValues.Contains(dst));

            var target = indexedNodes[dst];

            pickup.Next.Next.Next = target.Next;
            target.Next = pickup;

            current = current.Next;
        }

        while (current.Value != 1)
        {
            current = current.Next;
        }

        return current;
    }
    
    private static void FindNode1AfterNIterations(Span<int> nodes, int iterations)
    {
        var max = nodes.Length - 1;

        var current = nodes[0];
        for (var t = 0; t < iterations; t++)
        {
            var pickup1 = nodes[current];
            var pickup2 = nodes[pickup1];
            var pickup3 = nodes[pickup2];
                
            nodes[current] = nodes[pickup3];

            var dst = current;
            do
            {
                dst = dst == 1 ? max : dst - 1;
            } while (dst == pickup1 || dst == pickup2 || dst == pickup3);
                
            nodes[pickup3] = nodes[dst];
            nodes[dst] = pickup1;

            current = nodes[current];
        }
    }

    private class L
    {
        public L(int value)
        {
            Value = value;
        }

        public int Value { get; }
        public L Next { get; set; }
    }
    
    public record Model(IReadOnlyList<int> Digits);
}
