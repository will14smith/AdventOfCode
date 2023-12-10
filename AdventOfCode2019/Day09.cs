using System.Text;
using AdventOfCode2019.IntCode;

namespace AdventOfCode2019;

[Day]
public partial class Day09 : Day<Day09.Model, string, string>
{
    protected override Model Parse(string input) => new (Evaluator.Parse(input));

    [Sample("109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99", "1091204-1100110011001008100161011006101099")]
    [Sample("1102,34915192,34915192,7,4,7,99,0", "1219070632396864")]
    [Sample("104,1125899906842624,99", "1125899906842624")]
    protected override string Part1(Model model)
    {
        var eval = new Evaluator(model.InitialMemory);
        var buffer = new StringBuilder();
        
        Evaluator.State? state = null;
        while (true)
        {
            var result = eval.Run(state);
            switch (result)
            {
                case Result.Halted: return buffer.ToString();
                case Result.Input input: state = input.SetInput(1); break;
                case Result.Output output: 
                    buffer.Append(output.Value);
                    state = output.State; 
                    break;
                
                default: throw new ArgumentOutOfRangeException(nameof(result));
            }
        }
    }

    protected override string Part2(Model model)
    {
        var eval = new Evaluator(model.InitialMemory);
        var buffer = new StringBuilder();
        
        Evaluator.State? state = null;
        while (true)
        {
            var result = eval.Run(state);
            switch (result)
            {
                case Result.Halted: return buffer.ToString();
                case Result.Input input: state = input.SetInput(2); break;
                case Result.Output output: 
                    buffer.Append(output.Value);
                    state = output.State; 
                    break;
                
                default: throw new ArgumentOutOfRangeException(nameof(result));
            }
        }
    }

    public record Model(IReadOnlyList<long> InitialMemory);
}
