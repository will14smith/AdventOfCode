namespace AdventOfCode2019.IntCode;

public partial class Evaluator
{
    private readonly IReadOnlyList<int> _initialMemory;

    public Evaluator(IReadOnlyList<int> initialMemory)
    {
        _initialMemory = initialMemory;
    }

    public static IReadOnlyList<int> Parse(string input) => input.Split(',').Select(int.Parse).ToArray();
    
    public Result Run(State? state = null)
    {
        var memory = state?.Memory ?? _initialMemory.ToArray();
        var memorySpan = memory.Span;
        var ip = state?.Ip ?? 0;
        
        while (true)
        {
            var opcode = Opcode.Read(memorySpan[ip]);
            switch (opcode)
            {
                case { Operation: Operation.Add }: WriteParameter(3, ReadParameter(1) + ReadParameter(2)); ip += 4; break;
                case { Operation: Operation.Mul }: WriteParameter(3, ReadParameter(1) * ReadParameter(2)); ip += 4; break;

                case { Operation: Operation.Input }: return new Result.Input(new State(memory, ip + 2), memorySpan[ip + 1]);
                case { Operation: Operation.Output }: return new Result.Output(new State(memory, ip + 2), ReadParameter(1));
                
                case { Operation: Operation.JumpIfTrue }: ip = ReadParameter(1) != 0 ? ReadParameter(2) : ip + 3; break;
                case { Operation: Operation.JumpIfFalse }: ip = ReadParameter(1) == 0 ? ReadParameter(2) : ip + 3; break;
                
                case { Operation: Operation.LessThan }: WriteParameter(3, ReadParameter(1) < ReadParameter(2) ? 1 : 0); ip += 4; break;
                case { Operation: Operation.Equals }: WriteParameter(3, ReadParameter(1) == ReadParameter(2) ? 1 : 0); ip += 4; break;

                
                case { Operation: Operation.Halt }: return new Result.Halted(new State(memory, ip));
                
                default: throw new NotImplementedException($"unhandled opcode: {opcode}");
            }
            
            int ReadParameter(int parameterIndex)
            {
                var (parameterOffset, parameterMode) = parameterIndex switch
                {
                    1 => (ip + 1, opcode.ParameterMode1),
                    2 => (ip + 2, opcode.ParameterMode2),
                    3 => (ip + 3, opcode.ParameterMode3),
                };

                return parameterMode switch
                {
                    ParameterMode.PositionMode => memory.Span[memory.Span[parameterOffset]],
                    ParameterMode.ImmediateMode => memory.Span[parameterOffset],
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            void WriteParameter(int parameterIndex, int value)
            {
                var (parameterOffset, parameterMode) = parameterIndex switch
                {
                    1 => (ip + 1, opcode.ParameterMode1),
                    2 => (ip + 2, opcode.ParameterMode2),
                    3 => (ip + 3, opcode.ParameterMode3),
                };

                switch (parameterMode)
                {
                    case ParameterMode.PositionMode:
                        memory.Span[memory.Span[parameterOffset]] = value;
                        break;
                
                    case ParameterMode.ImmediateMode: throw new Exception("cannot write value to immediate");
                
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public class State
    {
        public Memory<int> Memory { get; }
        public int Ip { get; }

        public State(Memory<int> memory, int ip)
        {
            Memory = memory;
            Ip = ip;
        }
    }
}