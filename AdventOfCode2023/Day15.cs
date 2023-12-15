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
        var hashMap = new HashMap();
        
        foreach (var value in input.Values)
        {
            var parts = value.Split('=');
            if (parts.Length == 2)
            {
                var label = parts[0];
                var lens = byte.Parse(parts[1]);

                hashMap.Set(label, lens);
            }
            else
            {
                var label = value[..^1];
                
                hashMap.Remove(label);
            }
        }

        return hashMap.FocusingPower();
    }

    private static int Hash(string value) => (byte)value.Aggregate(0, (h, c) => ((h + c) * 17) & 0xFF);
    
    public record Model(IReadOnlyList<string> Values);

    public class HashMap
    {
        private readonly List<string>[] _boxLabels = Enumerable.Range(0, 256).Select(_ => new List<string>()).ToArray();
        private readonly List<int>[] _boxLenses = Enumerable.Range(0, 256).Select(_ => new List<int>()).ToArray();

        public void Set(string label, int lens)
        {
            var boxIndex = Hash(label);

            var boxLabels = _boxLabels[boxIndex];
            var boxLenses = _boxLenses[boxIndex];

            var existingIndex = boxLabels.IndexOf(label);
            if (existingIndex == -1)
            {
                boxLabels.Add(label);
                boxLenses.Add(lens);
            }
            else
            {
                boxLenses[existingIndex] = lens;
            }
        }

        public void Remove(string label)
        {
            var boxIndex = Hash(label); 

            var boxLabels = _boxLabels[boxIndex];
            
            var existingIndex = boxLabels.IndexOf(label);
            if (existingIndex == -1) return;
                
            var boxLenses = _boxLenses[boxIndex];
            
            boxLabels.RemoveAt(existingIndex);
            boxLenses.RemoveAt(existingIndex);
        }

        public int FocusingPower() => _boxLenses.SelectMany((box, boxIndex) => box.Select((lens, lensIndex) => (boxIndex + 1) * (lensIndex + 1) * lens)).Sum();
    }
}