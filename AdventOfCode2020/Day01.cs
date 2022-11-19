namespace AdventOfCode2020;

[Day]
public partial class Day01 : LineDay<int, int, int>
{
    protected override int ParseLine(string input) => int.Parse(input);

    [Sample("1721\n979\n366\n299\n675\n1456", 514579)]
    protected override int Part1(IEnumerable<int> input)
    {
        var set = input.ToHashSet();

        foreach (var item in set)
        {
            var pair = 2020 - item;
            if (set.Contains(pair))
            {
                return item * pair;
            }
        }

        throw new Exception("Failed to find a matching pair");
    }

    [Sample("1721\n979\n366\n299\n675\n1456", 241861950)]
    protected override int Part2(IEnumerable<int> input)
    {
        var set = input.ToHashSet();

        foreach (var item1 in set)
        foreach (var item2 in set)
        {
            var item3 = 2020 - item1 - item2;
            if (set.Contains(item3))
            {
                return item1 * item2 * item3;
            }
        }

        throw new Exception("Failed to find a matching pair");
    }
}
