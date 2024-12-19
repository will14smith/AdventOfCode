using System.Text.RegularExpressions;

namespace AdventOfCode2024;

[Day]
public partial class Day19 : Day<Day19.Model, int, long>
{
    public record Model(IReadOnlyList<string> Patterns, IReadOnlyList<string> Designs);
    
    protected override Model Parse(string input)
    {
        var parts = input.Split("\n\n");
        return new Model(parts[0].Split(", "), parts[1].Split("\n"));
    }

    [Sample("r, wr, b, g, bwu, rb, gb, br\n\nbrwrr\nbggr\ngbbr\nrrbgbr\nubwu\nbwurrg\nbrgr\nbbrgwb", 6)]
    protected override int Part1(Model input)
    {
        var matcher = new Regex($"^(?:{string.Join("|", input.Patterns)})*$", RegexOptions.Compiled | RegexOptions.NonBacktracking);

        return input.Designs.Count(x => matcher.IsMatch(x));
    }
    
    [Sample("r, wr, b, g, bwu, rb, gb, br\n\nbrwrr\nbggr\ngbbr\nrrbgbr\nubwu\nbwurrg\nbrgr\nbbrgwb", 16L)]
    protected override long Part2(Model input)
    {
        var cache = new Dictionary<string, long>();
        var patternsByStart = input.Patterns.ToLookup(x => x[0]);
        
        return input.Designs.Sum(x => CountNumber(x, patternsByStart, cache));
    }

    private static long CountNumber(string design, ILookup<char, string> patternsByStart, Dictionary<string, long> cache)
    {
        if (cache.TryGetValue(design, out var existing)) return existing;
        
        var start = design[0];
        var patterns = patternsByStart[start];

        var count = 0L;
        foreach (var pattern in patterns)
        {
            if (design.StartsWith(pattern))
            {
                if (design.Length == pattern.Length)
                {
                    count++;
                }
                else
                {
                    count += CountNumber(design[pattern.Length..], patternsByStart, cache);
                }
            }
        }
        
        cache[design] = count;
        
        return count;
    }
}