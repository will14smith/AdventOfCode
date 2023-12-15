namespace AdventOfCode2023;

[Day]
public partial class Day15 : Day<Day15.Model, int, int>
{
    protected override Model Parse(string input) => new Model(input.Split(','));

    [Sample("HASH", 52)]
    [Sample("HASH,HASH", 104)]
    protected override int Part1(Model input) => input.Values.Sum(Hash);
    
    [Sample("rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7", 145)]
    protected override int Part2(Model input)
    {
        var boxKeys = Enumerable.Range(0, 256).Select(_ => new List<string>()).ToArray();
        var boxValues = Enumerable.Range(0, 256).Select(_ => new List<int>()).ToArray();
        
        foreach (var value in input.Values)
        {
            var parts = value.Split('=');
            if (parts.Length == 2)
            {
                var label = parts[0];
                var box = Hash(label); 

                var lens = byte.Parse(parts[1]);

                var existingIndex = boxKeys[box].IndexOf(label);
                if (existingIndex == -1)
                {
                    boxKeys[box].Add(label);
                    boxValues[box].Add(lens);
                }
                else
                {
                    boxValues[box][existingIndex] = lens;
                }
            }
            else
            {
                var label = value[..^1];
                var box = Hash(label); 

                var existingIndex = boxKeys[box].IndexOf(label);
                if (existingIndex == -1) continue;
                
                boxKeys[box].RemoveAt(existingIndex);
                boxValues[box].RemoveAt(existingIndex);
            }
        }

        return boxValues.SelectMany((box, boxIndex) => box.Select((lens, lensIndex) => (boxIndex + 1) * (lensIndex + 1) * lens)).Sum();
    }

    private int Hash(string value) => (byte)value.Aggregate(0, (h, c) => ((h + c) * 17) & 0xFF);
    
    public record Model(IReadOnlyList<string> Values);
}