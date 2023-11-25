namespace AdventOfCode2017;

[Day]
public partial class Day04 : Day<Day04.Model, int, int>
{
    protected override Model Parse(string input) => new(input.Split('\n').Select(x => new Passphrase(x.Split(' ').ToList())).ToList());

    [Sample("aa bb cc dd ee", 1)]
    [Sample("aa bb cc dd aa", 0)]
    [Sample("aa bb cc dd aaa", 1)]
    protected override int Part1(Model input) => input.Passphrases.Count(x => x.AllWordsAreUnique);
    
    [Sample("abcde fghij", 1)]
    [Sample("abcde xyz ecdab", 0)]
    [Sample("a ab abc abd abf abj", 1)]
    [Sample("iiii oiii ooii oooi oooo", 1)]
    [Sample("oiii ioii iioi iiio", 0)]
    protected override int Part2(Model input) => input.Passphrases.Select(x => x.AnagrammedPassphrase).Count(x => x.AllWordsAreUnique);

    public record Model(IReadOnlyList<Passphrase> Passphrases);

    public record Passphrase(IReadOnlyList<string> Words)
    {
        public bool AllWordsAreUnique => Words.Distinct().Count() == Words.Count;
        public Passphrase AnagrammedPassphrase => new(Words.Select(w => new string(w.OrderBy(c => c).ToArray())).ToList());
    }
}
