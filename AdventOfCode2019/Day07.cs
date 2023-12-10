using AdventOfCode2019.IntCode;

namespace AdventOfCode2019;

[Day]
public partial class Day07 : Day<Day07.Model, long, long>
{
    protected override Model Parse(string input) => new (Evaluator.Parse(input));

    [Sample("3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0", 43210L)]
    [Sample("3,23,3,24,1002,24,10,24,1002,23,-1,23,101,5,23,23,1,24,23,23,4,23,99,0,0", 54321L)]
    [Sample("3,31,3,32,1002,32,10,32,1001,31,-2,31,1007,31,0,33,1002,33,7,33,1,33,31,31,1,32,31,31,4,31,99,0,0,0", 65210L)]
    protected override long Part1(Model model)
    {
        var codes = Permutations.Get(new[] { 0, 1, 2, 3, 4 }, 5);
        var max = 0L;
        
        foreach (var code in codes)
        {
            var outputOfPrevious = 0L;
            foreach (var digit in code)
            {
                var eval = new Evaluator(model.InitialMemory);
                var a = (Result.Input)eval.Run();
                var b = (Result.Input)eval.Run(a.SetInput(digit));
                var c = (Result.Output)eval.Run(b.SetInput(outputOfPrevious));
                outputOfPrevious = c.Value;
            }

            max = Math.Max(max, outputOfPrevious);
        }

        return max;
    }
    
    [Sample("3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5", 139629729L)]
    [Sample("3,52,1001,52,-5,52,3,53,1,52,56,54,1007,54,5,55,1005,55,26,1001,54,-5,54,1105,1,12,1,53,54,53,1008,54,0,55,1001,55,1,55,2,53,55,53,4,53,1001,56,-1,56,1005,56,6,99,0,0,0,0,10", 18216L)]
    protected override long Part2(Model model)
    {
        var codes = Permutations.Get(new[] { 5, 6, 7, 8, 9 }, 5);
        var max = 0L;
        
        foreach (var code in codes)
        {
            var digits = code.ToArray();
            var eval = new Evaluator(model.InitialMemory);
            var states = new Result.Input[digits.Length];

            for (var index = 0; index < digits.Length; index++)
            {
                var digit = digits[index];
                var a = (Result.Input)eval.Run();
                var b = (Result.Input)eval.Run(a.SetInput(digit));
                states[index] = b;
            }

            var outputOfPrevious = 0L;

            var done = false;
            while (!done)
            {
                for (var index = 0; index < states.Length; index++)
                {
                    var a = (Result.Output)eval.Run(states[index].SetInput(outputOfPrevious));
                    outputOfPrevious = a.Value;

                    var b = eval.Run(a.State);
                    switch (b)
                    {
                        case Result.Input input:
                            states[index] = input;
                            break;
                        case Result.Halted:
                            done = true;
                            break;
                    }
                }
            }

            max = Math.Max(max, outputOfPrevious);
        }

        return max;
    }

    public record Model(IReadOnlyList<long> InitialMemory);
}
