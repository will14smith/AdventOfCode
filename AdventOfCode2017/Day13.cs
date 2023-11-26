namespace AdventOfCode2017;

[Day]
public partial class Day13 : LineDay<Day13.Model, int, int>
{
    public record Model(int Depth, int Range);

    public record Scanner(int Position, bool Down, int Size)
    {
        public Scanner Next => Down ? Position + 1 == Size ? new Scanner(Position - 1, false, Size) : this with { Position = Position + 1 } : Position == 0 ? new Scanner(1, true, Size) : this with { Position = Position - 1 };

        public int DangerousEveryN => (Size - 1) * 2;
    }

    [Sample("0: 3\n1: 2\n4: 4\n6: 4", 24)]
    protected override int Part1(IEnumerable<Model> input)
    {
        var scanners = input.ToDictionary(x => x.Depth, x=> new Scanner(0, true, x.Range));
        var size = scanners.Keys.Max();

        var severity = 0;
        for (var i = 0; i <= size; i++)
        {
            if (scanners.TryGetValue(i, out var scanner))
            {
                if (scanner.Position == 0)
                {
                    severity += i * scanner.Size;
                }
            }
            
            scanners = Next(scanners);
        }

        return severity;
    }
    private static Dictionary<int, Scanner> Next(Dictionary<int, Scanner> input) => input.ToDictionary(x => x.Key, x => x.Value.Next);

    [Sample("0: 3\n1: 2\n4: 4\n6: 4", 10)]
    protected override int Part2(IEnumerable<Model> input)
    {
        var scanners = input.ToDictionary(x => x.Depth, x=> new Scanner(0, true, x.Range));
        
        var size = scanners.Keys.Max();
        var scannerModuli = Enumerable.Range(0, size + 1).Select(x => scanners.TryGetValue(x, out var scanner) ? scanner.DangerousEveryN : 0).ToArray();
        
        var delay = 0;
        
        while (true)
        {
            var success = true;
            for (var i = 0; i <= size; i++)
            {
                var scannerModulus = scannerModuli[i];
                if(scannerModulus == 0)
                {
                    continue;
                }

                if ((i + delay) % scannerModulus != 0)
                {
                    continue;
                }
                
                success = false;
                break;
            }

            if (success)
            {
                return delay;
            }

            delay++;
        }
    }

    protected override Model ParseLine(string input)
    {
        var parts = input.Split(": ").Select(int.Parse).ToArray();
        return new Model(parts[0], parts[1]);
    }
}
