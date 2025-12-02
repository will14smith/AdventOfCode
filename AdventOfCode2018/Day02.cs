namespace AdventOfCode2018;

[Day]
public partial class Day02 : Day<Day02.Model, int, string>
{
    protected override Model Parse(string input) => new(input.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList());

    [Sample("abcdef\nbababc\nabbcde\nabcccd\naabcdd\nabcdee\nababab\n", 12)]
    protected override int Part1(Model input)
    {
        var twos = 0;
        var threes = 0;
        
        foreach (var id in input.Ids)
        {
            var charCounts = id.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

            if (charCounts.Values.Any(v => v == 2))
            {
                twos++;
            }

            if (charCounts.Values.Any(v => v == 3))
            {
                threes++;
            }
        }
        
        return twos * threes;
    }

    [Sample("abcde\nfghij\nklmno\npqrst\nfguij\naxcye\nwvxyz\n", "fgij")]
    protected override string Part2(Model input)
    {
        for (var i = 0; i < input.Ids.Count; i++)
        {
            for(var j = i + 1; j < input.Ids.Count; j++)
            {
                var id1 = input.Ids[i];
                var id2 = input.Ids[j];
                
                var diffs = 0;
                var commonChars = new List<char>();
                
                for (var k = 0; k < id1.Length; k++)
                {
                    if (id1[k] != id2[k])
                    {
                        diffs++;
                        if (diffs > 1)
                        {
                            break;
                        }
                    }
                    else
                    {
                        commonChars.Add(id1[k]);
                    }
                }

                if (diffs == 1)
                {
                    return new string(commonChars.ToArray());
                }
            }
        }

        throw new NotImplementedException();
    }

    public record Model(IReadOnlyList<string> Ids);
}
