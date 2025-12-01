namespace AdventOfCode2025;

[Day]
public partial class Day01 : LineDay<Day01.Instruction, int, int>
{
    public record Instruction(Direction Direction, int Distance);
    public enum Direction
    {
        Left,
        Right
    }
    
    protected override Instruction ParseLine(string input) => new(input[0] == 'L' ? Direction.Left : Direction.Right, int.Parse(input[1..]));

    [Sample("L68\nL30\nR48\nL5\nR60\nL55\nL1\nL99\nR14\nL82\n", 3)]
    protected override int Part1(IEnumerable<Instruction> input)
    {
        var count = 0;
        
        var state = 50;
        foreach (var model in input)
        {
            (state, _) = Apply(state, model);
            if (state == 0)
            {
                count++;
            }
        }

        return count;
    }
    
    [Sample("L68\nL30\nR48\nL5\nR60\nL55\nL1\nL99\nR14\nL82\n", 6)]
    protected override int Part2(IEnumerable<Instruction> input)
    {
        var clicks = 0;
        
        var state = 50;
        foreach (var model in input)
        {
            (state, var instructionClicks) = Apply(state, model);
            clicks += instructionClicks;
        }
        
        return clicks;
    }
    
    private static (int NewState, int Clicks) Apply(int position, Instruction instruction)
    {
        var clicks = instruction.Distance / 100;
        var distance = instruction.Distance % 100 * instruction.Direction switch
        {
            Direction.Left => -1,
            Direction.Right => 1,
        };

        if (position + distance >= 100)
        {
            clicks++;
        }
        
        if (position > 0 && position + distance <= 0)
        {
            clicks++;
        }
        
        return ((position + distance + 100) % 100, clicks);
    }
}