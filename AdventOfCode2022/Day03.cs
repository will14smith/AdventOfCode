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
    protected override int Part1(IEnumerable<Bag> input) => input.Sum(x => x.CommonItems.Sum(Score));

    [Sample(Sample, 70)]
    protected override int Part2(IEnumerable<Bag> input) => GroupIntoThrees(input).Select(x => x.A.All.Intersect(x.B.All).Intersect(x.C.All).Sum(Score)).Sum();

    private static IEnumerable<(Bag A, Bag B, Bag C)> GroupIntoThrees(IEnumerable<Bag> input)
    {
        var inputList = input.ToArray();

        for (var i = 0; i < inputList.Length; i+=3)
        {
            yield return (inputList[i], inputList[i + 1], inputList[i + 2]);
        }
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
        public IReadOnlySet<char> CommonItems => First.Intersect(Second).ToHashSet();
        public IReadOnlySet<char> All => First.Concat(Second).ToHashSet();
    }
}
