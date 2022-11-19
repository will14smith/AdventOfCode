using System.Collections.Immutable;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2020;

[Day]
public partial class Day06 : ParseDay<Day06.Group[], int, int>
{
    private static readonly TextParser<char> Answer = Character.Lower;
    private static readonly TextParser<Person> PersonAnswer = Answer.AtLeastOnce().Select(xs => new Person(xs.ToImmutableHashSet()));
    private static readonly TextParser<Group> AnswerGroup = PersonAnswer.ThenIgnoreOptional(SuperpowerExtensions.NewLine).AtLeastOnce().Select(ps => new Group(ps));
    private static readonly TextParser<Group[]> AnswerGroups = AnswerGroup.AtLeastOnceDelimitedBy(SuperpowerExtensions.NewLine).ThenIgnoreOptional(SuperpowerExtensions.NewLine);

    protected override TextParser<Group[]> Parser => AnswerGroups;

    [Sample("abc\n\na\nb\nc\n\nab\nac\n\na\na\na\na\n\nb", 11)]
    protected override int Part1(Group[] input) => input.Sum(x => x.CountUnique());

    [Sample("abc\n\na\nb\nc\n\nab\nac\n\na\na\na\na\n\nb", 6)]
    protected override int Part2(Group[] input) => input.Sum(x => x.CountAll());

    public record Person(ImmutableHashSet<char> Answers);

    public record Group(IReadOnlyCollection<Person> People)
    {
        public int CountUnique() => People.Select(x => x.Answers).Aggregate((a, p) => a.Union(p)).Count;
        public int CountAll() => People.Select(x => x.Answers).Aggregate((a, p) => a.Intersect(p)).Count;
    }
}
