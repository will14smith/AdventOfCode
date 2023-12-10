using AdventOfCode2019.IntCode;

namespace AdventOfCode2019;

[Day]
public partial class Day05 : Day<Day05.Model, long, long>
{
    protected override Model Parse(string input) => new(Evaluator.Parse(input));

    protected override long Part1(Model input)
    {
        var eval = new Evaluator(input.InitialMemory);

        var lastOutput = 0L;
        
        Evaluator.State? state = null;
        while (true)
        {
            var result = eval.Run(state);
            state = result.State;
            
            switch (result)
            {
                case Result.Input inp: state = inp.SetInput(1); break;
                case Result.Output output: lastOutput = output.Value; break;
                
                case Result.Halted: return lastOutput;
                default: throw new ArgumentOutOfRangeException(nameof(result));
            }
        }
    }
    
    protected override long Part2(Model input)
    {
        var eval = new Evaluator(input.InitialMemory);

        var lastOutput = 0L;
        
        Evaluator.State? state = null;
        while (true)
        {
            var result = eval.Run(state);
            state = result.State;
            
            switch (result)
            {
                case Result.Input inp: state = inp.SetInput(5); break;
                case Result.Output output: lastOutput = output.Value; break;
                
                case Result.Halted: return lastOutput;
                default: throw new ArgumentOutOfRangeException(nameof(result));
            }
        }
    }

    public record Model(IReadOnlyList<long> InitialMemory);
}
