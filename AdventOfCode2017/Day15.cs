namespace AdventOfCode2017;

[Day]
public partial class Day15 : Day<Day15.Model, int, int>
{
    protected override Model Parse(string input)
    {
        var parts = input.Split('\n').Select(x => int.Parse(new string(x.Where(char.IsDigit).ToArray()))).ToArray();
        return new Model(parts[0], parts[1]);
    }

    [Sample("16807\n48271", 581)]
    protected override int Part1(Model input)
    {
        var machineA = new Machine(input.A, 16807);
        var machineB = new Machine(input.B, 48271);

        var matches = 0;
        for (var i = 0; i < 40_000_000; i++)
        {
            machineA = machineA.Next();
            machineB = machineB.Next();

            if ((machineA.State & 0xffff) == (machineB.State & 0xffff))
            {
                matches++;
            }
        }

        return matches;
    }
    
    protected override int Part2(Model input)
    {
        var machineA = new Machine(input.A, 16807);
        var machineB = new Machine(input.B, 48271);

        var matches = 0;
        for (var i = 0; i < 5_000_000; i++)
        {
            do { machineA = machineA.Next(); } while (machineA.State % 4 != 0);
            do { machineB = machineB.Next(); } while (machineB.State % 8 != 0);
            
            if ((machineA.State & 0xffff) == (machineB.State & 0xffff))
            {
                matches++;
            }
        }

        return matches; 
    }

    public record Machine(int State, int Factor)
    {
        public Machine Next() => this with { State = (int)((long)State * Factor % 2147483647) };
    }
    
    public record Model(int A, int B);
}