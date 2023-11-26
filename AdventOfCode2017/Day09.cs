using System.Text;

namespace AdventOfCode2017;

[Day]
public partial class Day09 : Day<Day09.Model, int, int>
{
    private ref struct Parser
    {
        public int Offset;
    }
    
    protected override Model Parse(string input)
    {
        var parser = new Parser();
        return Parse(input, ref parser);
    }

    private Model Parse(string input, ref Parser parser)
    {
        switch (input[parser.Offset])
        {
            case '{':
                parser.Offset++;

                // group
                var children = new List<Model>();
                if (input[parser.Offset] == '}')
                {
                    parser.Offset++;
                    return new Model.Group(children);
                }

                while (true)
                {
                    children.Add(Parse(input, ref parser));

                    var next = input[parser.Offset++];
                    if (next == '}') return new Model.Group(children);
                    if (next != ',') throw new Exception();
                }
                break;
            case '<':
                // garbage
                var sb = new StringBuilder();
                
                while (input[++parser.Offset] != '>')
                {
                    if (input[parser.Offset] == '!')
                    {
                        parser.Offset++;
                    }
                    else
                    {
                        sb.Append(input[parser.Offset]);
                    }
                }

                parser.Offset++;
                return new Model.Garbage(sb.ToString());

            default: throw new Exception("invalid");
        }
    }

    [Sample("{}", 1)]
    [Sample("{{{}}}", 6)]
    [Sample("{{},{}}", 5)]
    [Sample("{{{},{},{{}}}}", 16)]
    [Sample("{<a>,<a>,<a>,<a>}", 1)]
    [Sample("{{<ab>},{<ab>},{<ab>},{<ab>}}", 9)]
    [Sample("{{<!!>},{<!!>},{<!!>},{<!!>}}", 9)]
    [Sample("{{<a!>},{<a!>},{<a!>},{<ab>}}", 3)]
    protected override int Part1(Model input) => Score(input, 1);
    private static int Score(Model input, int parentScore) =>
        input switch
        {
            Model.Garbage => 0,
            Model.Group group => parentScore + group.Children.Sum(c => Score(c, parentScore + 1)),
            
            _ => throw new ArgumentOutOfRangeException(nameof(input))
        };

    [Sample("<>", 0)]
    [Sample("<random characters>", 17)]
    [Sample("<<<<>", 3)]
    [Sample("<{!>}>", 2)]
    [Sample("<!!>", 0)]
    [Sample("<!!!>>", 0)]
    [Sample("<{o\"i!a,<{i<a>", 10)]
    protected override int Part2(Model input) => Count(input);
    private static int Count(Model input) =>
        input switch
        {
            Model.Garbage garbage => garbage.Value.Length,
            Model.Group group => group.Children.Sum(Count),
            
            _ => throw new ArgumentOutOfRangeException(nameof(input))
        };

    public abstract record Model
    {
        public record Group(IReadOnlyList<Model> Children) : Model;
        public record Garbage(string Value) : Model;
    }
}
