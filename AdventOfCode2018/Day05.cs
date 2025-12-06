using System.Text;

namespace AdventOfCode2018;

[Day]
public partial class Day05 : Day<Day05.Model, int, int>
{
    protected override Model Parse(string input) => new(input);

    [Sample("dabAcCaCBAcCcaDA", 10)]
    protected override int Part1(Model input) => ReactUntilDone(input.Polymer);
    
    [Sample("dabAcCaCBAcCcaDA", 4)]
    protected override int Part2(Model input) =>
        input.Polymer
            .Select(char.ToLowerInvariant)
            .Distinct()
            .Select(x => input.Polymer.Replace(x.ToString(), "").Replace(char.ToUpperInvariant(x).ToString(), ""))
            .Min(ReactUntilDone);

    private static int ReactUntilDone(string polymer)
    {
        while (true)
        {
            var next = React(polymer);
            if (next.Length == polymer.Length)
            {
                return next.Length;
            }
            
            polymer = next;
        }
    }
    
    private static string React(ReadOnlySpan<char> polymer)
    {
        var result = new StringBuilder(polymer.Length);
        
        for (var i = 0; i < polymer.Length; i++)
        {
            if (i < polymer.Length - 1 && (polymer[i] + 32 == polymer[i + 1] || polymer[i] - 32 == polymer[i + 1]))
            {
                i++;
            }
            else
            {
                result.Append(polymer[i]);
            }
        }

        return result.ToString();
    }
    
    public record Model(string Polymer);
}
