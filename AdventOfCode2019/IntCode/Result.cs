namespace AdventOfCode2019.IntCode;

public abstract record Result(Evaluator.State State)
{
    public record Halted(Evaluator.State State) : Result(State);

    public record Input(Evaluator.State State, long Target) : Result(State)
    {
        public Evaluator.State SetInput(long value)
        {
            State.Memory[Target] = value;
            return State;
        }
    }
    public record Output(Evaluator.State State, long Value) : Result(State);
}