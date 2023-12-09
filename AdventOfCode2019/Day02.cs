namespace AdventOfCode2019;

[Day]
public partial class Day02 : Day<Day02.Model, int, int>
{
    protected override Model Parse(string input) => new(input.Split(',').Select(int.Parse).ToArray());

    [Sample("1,9,10,3,2,3,11,0,99,30,40,50", 3500)]
    [Sample("1,0,0,0,99", 2)]
    [Sample("2,5,0,0,99,3", 6)]
    [Sample("2,4,4,0,99,0", 9801)]
    protected override int Part1(Model input)
    {
        var memory = input.InitialMemory.ToArray();

        // this only applies to the actual puzzle, not the samples
        if (memory.Length > 20)
        {
            memory[1] = 12;
            memory[2] = 2;
        }

        return Run(memory);
    }
    
    protected override int Part2(Model input)
    {
        for(var noun = 0; noun < 100; noun++)
        for (var verb = 0; verb < 100; verb++)
        {
            var memory = input.InitialMemory.ToArray();

            memory[1] = noun;
            memory[2] = verb;

            if (Run(memory) == 19690720) return noun * 100 + verb;
        }
        
        throw new Exception("no.");
    }

    private static int Run(int[] memory)
    {
        var ip = 0;
        while (true)
        {
            switch (memory[ip])
            {
                case 1:
                {
                    memory[memory[ip + 3]] = memory[memory[ip + 1]] + memory[memory[ip + 2]];
                    ip += 4;
                    break;
                }
                case 2:    
                {
                    memory[memory[ip + 3]] = memory[memory[ip + 1]] * memory[memory[ip + 2]];
                    ip += 4;
                    break;
                }

                
                case 99: return memory[0];
                
                default: throw new NotImplementedException($"unhandled opcode: {memory[ip]}");
            }
        }
    }
    
    public record Model(IReadOnlyList<int> InitialMemory);
}