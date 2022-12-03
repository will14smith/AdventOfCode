namespace AdventOfCode2022;

[Day]
public partial class Day03 : LineDay<Day03.Bag, int, int>
{
    private const string Sample = "vJrwpWtwJgWrhcsFMMfFFhFp\njqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL\nPmmdzqPrVvPwwTWBwg\nwMqvLMZHhHMvwLHjbvcjnnSBnvTQFn\nttgJtRGJQctTZtZT\nCrZsJsPPZsGzwwsLwLmpwMDw";
    
    protected override Bag ParseLine(string input)
    {
        var halfLen = input.Length >> 1;
        return new Bag(input[..halfLen].ToCharArray(), input[halfLen..].ToCharArray());
    }
    
    [Sample(Sample, 157)]
    protected override int Part1(IEnumerable<Bag> input) => input.Sum(ScorePart1);

    [Sample(Sample, 70)]
    protected override int Part2(IEnumerable<Bag> input) => input.BatchesOf3().Sum(ScorePart2);

    private static int ScorePart1(Bag bag)
    {
        var items = bag.First.ToHashSet();
        items.IntersectWith(bag.Second);
        return items.Sum(Score);
    }
    private static int ScorePart2((Bag A, Bag B, Bag C) group)
    {
        var items = group.A.All.ToHashSet();
        items.IntersectWith(group.B.All);
        items.IntersectWith(group.C.All);
        return items.Sum(Score);
    }
    
    private static int Score(char item)
    {
        return item switch
        {
            >= 'a' and <= 'z' => item - 'a' + 1,
            >= 'A' and <= 'Z' => item - 'A' + 27,
            _ => throw new ArgumentOutOfRangeException(nameof(item))
        };
    }
    
    public record Bag(IReadOnlyList<char> First, IReadOnlyList<char> Second)
    {
        public IReadOnlySet<char> All => First.Concat(Second).ToHashSet();
    }
}
