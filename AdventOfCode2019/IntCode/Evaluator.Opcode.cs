namespace AdventOfCode2019.IntCode;

public partial class Evaluator
{
    private record Opcode(
        Operation Operation,
        ParameterMode ParameterMode1,
        ParameterMode ParameterMode2,
        ParameterMode ParameterMode3)
    {
        public static Opcode Read(int value)
        {
            (value, var operation) = Math.DivRem(value, 100);
            (value, var param1) = Math.DivRem(value, 10);
            (value, var param2) = Math.DivRem(value, 10);
            var (_, param3) = Math.DivRem(value, 10);

            return new Opcode(
                (Operation)operation,
                (ParameterMode)param1,
                (ParameterMode)param2,
                (ParameterMode)param3
            );
        }
    }

    public enum Operation
    {
        Add = 1,
        Mul = 2,

        Input = 3,
        Output = 4,

        JumpIfTrue = 5,
        JumpIfFalse = 6,
        LessThan = 7,
        Equals = 8,

        Halt = 99,
    }

    public enum ParameterMode
    {
        PositionMode = 0,
        ImmediateMode = 1,
    }
}