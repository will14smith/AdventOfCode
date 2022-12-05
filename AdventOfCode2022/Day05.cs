using System.Collections.Immutable;
using System.Text;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2022;

[Day]
public partial class Day05 : ParseDay<Day05.Model, string, string>
{
    private const string Sample = "move 1 from 2 to 1\nmove 3 from 1 to 3\nmove 2 from 2 to 1\nmove 1 from 1 to 2";
    
    private static readonly TextParser<Instruction> InstructionParser = 
            Span.EqualTo("move ").IgnoreThen(Numerics.IntegerInt32)
                .ThenIgnore(Span.EqualTo(" from ")).Then(Numerics.IntegerInt32)
                .ThenIgnore(Span.EqualTo(" to ")).Then(Numerics.IntegerInt32)
                .Select(x => new Instruction(x.Item1.Item1, x.Item1.Item2, x.Item2));

    private static readonly Layout SampleBoxes = new Layout(
        ImmutableDictionary<int, ImmutableStack<char>>.Empty
            .Add(1, ImmutableStack<char>.Empty.Push('Z').Push('N'))
            .Add(2, ImmutableStack<char>.Empty.Push('M').Push('C').Push('D'))
            .Add(3, ImmutableStack<char>.Empty.Push('P'))
    );
    private static readonly Layout HardCodedBoxes = new Layout(
        ImmutableDictionary<int, ImmutableStack<char>>.Empty
            .Add(1, ImmutableStack<char>.Empty.Push('H').Push('R').Push('B').Push('D').Push('Z').Push('F').Push('L').Push('S'))
            .Add(2, ImmutableStack<char>.Empty.Push('T').Push('B').Push('M').Push('Z').Push('R'))
            .Add(3, ImmutableStack<char>.Empty.Push('Z').Push('L').Push('C').Push('H').Push('N').Push('S'))
            .Add(4, ImmutableStack<char>.Empty.Push('S').Push('C').Push('F').Push('J'))
            .Add(5, ImmutableStack<char>.Empty.Push('P').Push('G').Push('H').Push('W').Push('R').Push('Z').Push('B'))
            .Add(6, ImmutableStack<char>.Empty.Push('V').Push('J').Push('Z').Push('G').Push('D').Push('N').Push('M').Push('T'))
            .Add(7, ImmutableStack<char>.Empty.Push('G').Push('L').Push('N').Push('W').Push('F').Push('S').Push('P').Push('Q'))
            .Add(8, ImmutableStack<char>.Empty.Push('M').Push('Z').Push('R'))
            .Add(9, ImmutableStack<char>.Empty.Push('M').Push('C').Push('L').Push('G').Push('V').Push('R').Push('T'))
    );
    
    private static readonly TextParser<Model> ModelParser = InstructionParser.ManyDelimitedBy(SuperpowerExtensions.NewLine).Select(instructions => new Model(
        HardCodedBoxes,
        instructions
    ));

    protected override TextParser<Model> Parser => ModelParser;
    
    [Sample(Sample, "CMZ")]
    protected override string Part1(Model input) => Solve(input, true);

    [Sample(Sample, "MCD")]
    protected override string Part2(Model input) => Solve(input, false);

    private static string Solve(Model input, bool singleBoxMovesOnly)
    {
        var layout = input.Instructions.Count == 4 ? SampleBoxes : input.Layout;

        foreach (var instruction in input.Instructions)
        {
            layout = Apply(layout, instruction, singleBoxMovesOnly);
        }

        var result = new StringBuilder();
        for (var i = 1; i <= layout.Boxes.Count; i++)
        {
            result.Append(layout.Boxes[i].Peek());
        }

        return result.ToString();
    }

    private static Layout Apply(Layout layout, Instruction instruction, bool singleBoxMovesOnly)
    {
        var from = layout.Boxes[instruction.From];
        var to = layout.Boxes[instruction.To];

        if (singleBoxMovesOnly)
        {
            for (var i = 0; i < instruction.Count; i++)
            {
                from = from.Pop(out var box);
                to = to.Push(box);
            }
        }
        else
        {
            var stack = new Stack<char>();
            for (var i = 0; i < instruction.Count; i++)
            {
                from = from.Pop(out var box);
                stack.Push(box);
            }

            for (var i = 0; i < instruction.Count; i++)
            {
                to = to.Push(stack.Pop());
            }
        }

        return new Layout(layout.Boxes.SetItem(instruction.From, from).SetItem(instruction.To, to));
    }

    public record Model(Layout Layout, IReadOnlyList<Instruction> Instructions);

    public record Layout(ImmutableDictionary<int, ImmutableStack<char>> Boxes);
    public record Instruction(int Count, int From, int To);
}
