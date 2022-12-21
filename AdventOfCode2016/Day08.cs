using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2016;

[Day]
public partial class Day08 : ParseLineDay<Day08.Instruction, int, string>
{
    private static readonly TextParser<Instruction> RectParser = Span.EqualTo("rect ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo("x")).Then(Numerics.IntegerInt32).Select(x => (Instruction)new Instruction.Rect(x.Item1, x.Item2));
    private static readonly TextParser<Instruction> RotateRowParser = Span.EqualTo("rotate row y=").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" by ")).Then(Numerics.IntegerInt32).Select(x => (Instruction)new Instruction.RotateRow(x.Item1, x.Item2));
    private static readonly TextParser<Instruction> RotateColParser = Span.EqualTo("rotate column x=").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" by ")).Then(Numerics.IntegerInt32).Select(x => (Instruction)new Instruction.RotateCol(x.Item1, x.Item2));

    protected override TextParser<Instruction> LineParser => RectParser.Try().Or(RotateRowParser.Try()).Or(RotateColParser);
    
    protected override int Part1(IEnumerable<Instruction> input) => Render(input).Count(x => x);
    
    protected override string Part2(IEnumerable<Instruction> input) => "\n" + Render(input).Print(x => x ? '#' : '.') + "\n";

    private static Grid<bool> Render(IEnumerable<Instruction> input) => input.Aggregate(Grid.Empty<bool>(50, 6), Render);

    private static Grid<bool> Render(Grid<bool> screen, Instruction instruction)
    {
        switch (instruction)
        {
            case Instruction.Rect rect:
                for (var y = 0; y < rect.H; y++)
                for (var x = 0; x < rect.W; x++)
                {
                    screen[x, y] = true;
                }

                break;
            case Instruction.RotateCol rotate:
                screen = screen.Select((current, screen, position) => position.X == rotate.X ? screen[position.X, (position.Y - rotate.Distance % screen.Height + screen.Height) % screen.Height] : current);
                break;
            case Instruction.RotateRow rotate:
                screen = screen.Select((current, screen, position) => position.Y == rotate.Y ? screen[(position.X - rotate.Distance % screen.Width + screen.Width) % screen.Width, position.Y] : current);
                break;

            default: throw new ArgumentOutOfRangeException(nameof(instruction));
        }

        return screen;
    }

    public abstract record Instruction
    {
        public record Rect(int W, int H) : Instruction;
        public record RotateRow(int Y, int Distance) : Instruction;
        public record RotateCol(int X, int Distance) : Instruction;
    }
}
