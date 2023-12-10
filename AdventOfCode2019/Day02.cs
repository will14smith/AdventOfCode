using AdventOfCode2019.IntCode;

namespace AdventOfCode2019;

[Day]
public partial class Day02 : Day<Day02.Model, long, long>
{
    protected override Model Parse(string input) => new(Evaluator.Parse(input));

    [Sample("1,9,10,3,2,3,11,0,99,30,40,50", 3500L)]
    [Sample("1,0,0,0,99", 2L)]
    [Sample("2,5,0,0,99,3", 6L)]
    [Sample("2,4,4,0,99,0", 9801L)]
    protected override long Part1(Model input)
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
    
    protected override long Part2(Model input)
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

    private static long Run(IReadOnlyList<long> memory) => new Evaluator(memory).Run().State.Memory[0];

    public record Model(IReadOnlyList<long> InitialMemory);
}