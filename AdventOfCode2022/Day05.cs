using System.Collections.Immutable;
using System.Text;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2022;

[Day]
public partial class Day05 : ParseDay<Day05.Model, string, string>
{
    private const string Sample = "    [D]    \n[N] [C]    \n[Z] [M] [P]\n 1   2   3 \n\nmove 1 from 2 to 1\nmove 3 from 1 to 3\nmove 2 from 2 to 1\nmove 1 from 1 to 2";

    private static readonly TextParser<char> Space = Character.EqualTo(' ');

    private static readonly TextParser<char?> BoxParser = Character.EqualTo('[').IgnoreThen(Character.Upper).ThenIgnore(Character.EqualTo(']')).Select(x => (char?)x).Or(Span.EqualTo("   ").Select(x => (char?)null));
    private static readonly TextParser<char?[]> BoxLineParser = BoxParser.ManyDelimitedBy(Space);
    private static readonly TextParser<int> BoxNameParser = Space.IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Space);
    private static readonly TextParser<int[]> BoxNamesParser = BoxNameParser.ManyDelimitedBy(Space);
    
    private static readonly TextParser<Layout> LayoutParser = 
        BoxLineParser.Try().ManyDelimitedBy(SuperpowerExtensions.NewLine)
            .ThenIgnore(SuperpowerExtensions.NewLine)
            .Then(BoxNamesParser)
            .Select(x => Layout.FromParser(x.Item1, x.Item2));

    private static readonly TextParser<Instruction> InstructionParser = 
            Span.EqualTo("move ").IgnoreThen(Numerics.IntegerInt32)
                .ThenIgnore(Span.EqualTo(" from ")).Then(Numerics.IntegerInt32)
                .ThenIgnore(Span.EqualTo(" to ")).Then(Numerics.IntegerInt32)
                .Select(x => new Instruction(x.Item1.Item1, x.Item1.Item2, x.Item2));
    
    private static readonly TextParser<Model> ModelParser = 
        LayoutParser
            .ThenIgnore(SuperpowerExtensions.NewLine.Repeat(2))
            .Then(InstructionParser.ManyDelimitedBy(SuperpowerExtensions.NewLine))
            .AtEnd()
        .Select(x => new Model(x.Item1, x.Item2));

    protected override TextParser<Model> Parser => ModelParser;
    
    [Sample(Sample, "CMZ")]
    protected override string Part1(Model input) => Solve(input, true);

    [Sample(Sample, "MCD")]
    protected override string Part2(Model input) => Solve(input, false);

    private static string Solve(Model input, bool singleBoxMovesOnly)
    {
        var layout = input.Layout;

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

    public record Layout(ImmutableDictionary<int, ImmutableStack<char>> Boxes)
    {
        public static Layout FromParser(char?[][] boxesInput, int[] names)
        {
            var boxes = ImmutableDictionary<int, ImmutableStack<char>>.Empty;

            foreach (var name in names)
            {
                boxes = boxes.SetItem(name, ImmutableStack<char>.Empty);
            }
            
            for (var lineIndex = boxesInput.Length - 1; lineIndex >= 0; lineIndex--)
            {
                var line = boxesInput[lineIndex];
                for (var nameIndex = 0; nameIndex < names.Length; nameIndex++)
                {
                    var name = names[nameIndex];
                    var box = line[nameIndex];

                    if (box != null)
                    {
                        boxes = boxes.SetItem(name, boxes[name].Push(box.Value));
                    }
                }
            }

            return new Layout(boxes);
        }
    }

    public record Instruction(int Count, int From, int To);
}
