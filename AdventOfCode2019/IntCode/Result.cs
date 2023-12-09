namespace AdventOfCode2019.IntCode;

public abstract record Result(Evaluator.State State)
{
    public record Halted(Evaluator.State State) : Result(State);

    public record Input(Evaluator.State State, int Target) : Result(State)
    {
        public Evaluator.State SetInput(int value)
        {
            State.Memory.Span[Target] = value;
            return State;
        }
    }
    public record Output(Evaluator.State State, int Value) : Result(State);
}