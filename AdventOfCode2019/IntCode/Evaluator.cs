namespace AdventOfCode2019.IntCode;

public partial class Evaluator
{
    private readonly IReadOnlyList<long> _initialMemory;

    public Evaluator(IReadOnlyList<long> initialMemory)
    {
        _initialMemory = initialMemory;
    }

    public static IReadOnlyList<long> Parse(string input) => input.Split(',').Select(long.Parse).ToArray();
    
    public Result Run(State? state = null)
    {
        var memory = state?.Memory ?? _initialMemory.Select((x, i) => (Address: (long)i, Value: x)).ToDictionary(x => x.Address, x => x.Value);
        var ip = state?.Ip ?? 0;
        var relativeBase = state?.RelativeBase ?? 0;
        
        while (true)
        {
            var opcode = Opcode.Read(ReadMemory(ip));
            switch (opcode)
            {
                case { Operation: Operation.Add }: WriteParameter(3, ReadParameter(1) + ReadParameter(2)); ip += 4; break;
                case { Operation: Operation.Mul }: WriteParameter(3, ReadParameter(1) * ReadParameter(2)); ip += 4; break;

                case { Operation: Operation.Input }: return new Result.Input(new State(memory, ip + 2, relativeBase), ReadParameterAddress(1));
                case { Operation: Operation.Output }: return new Result.Output(new State(memory, ip + 2, relativeBase), ReadParameter(1));
                
                case { Operation: Operation.JumpIfTrue }: ip = ReadParameter(1) != 0 ? ReadParameter(2) : ip + 3; break;
                case { Operation: Operation.JumpIfFalse }: ip = ReadParameter(1) == 0 ? ReadParameter(2) : ip + 3; break;
                
                case { Operation: Operation.LessThan }: WriteParameter(3, ReadParameter(1) < ReadParameter(2) ? 1 : 0); ip += 4; break;
                case { Operation: Operation.Equals }: WriteParameter(3, ReadParameter(1) == ReadParameter(2) ? 1 : 0); ip += 4; break;

                case { Operation: Operation.AddRelativeBase }: relativeBase += ReadParameter(1); ip += 2; break;
                
                case { Operation: Operation.Halt }: return new Result.Halted(new State(memory, ip, relativeBase));
                
                default: throw new NotImplementedException($"unhandled opcode: {opcode}");
            }

            long ReadMemory(long address) => memory.GetValueOrDefault(address, 0);
            
            long ReadParameter(int parameterIndex) => ReadMemory(ReadParameterAddress(parameterIndex));
            long ReadParameterAddress(int parameterIndex)
            {
                var (parameterOffset, parameterMode) = parameterIndex switch
                {
                    1 => (ip + 1, opcode.ParameterMode1),
                    2 => (ip + 2, opcode.ParameterMode2),
                    3 => (ip + 3, opcode.ParameterMode3),
                };

                return parameterMode switch
                {
                    ParameterMode.Position => ReadMemory(parameterOffset),
                    ParameterMode.Immediate => parameterOffset,
                    ParameterMode.Relative => relativeBase + ReadMemory(parameterOffset),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            
            void WriteParameter(int parameterIndex, long value)
            {
                var parameterMode = parameterIndex switch
                {
                    1 => opcode.ParameterMode1,
                    2 => opcode.ParameterMode2,
                    3 => opcode.ParameterMode3,
                };

                if (parameterMode == ParameterMode.Immediate)
                {
                    throw new NotSupportedException("cannot write value to immediate");
                }
                
                var parameterAddress = ReadParameterAddress(parameterIndex);
                memory[parameterAddress] = value;
            }
        }
    }

    public class State
    {
        public Dictionary<long, long> Memory { get; }
        public long Ip { get; }
        public long RelativeBase { get; }

        public State(Dictionary<long, long> memory, long ip, long relativeBase)
        {
            Memory = memory;
            Ip = ip;
            RelativeBase = relativeBase;
        }
    }
}