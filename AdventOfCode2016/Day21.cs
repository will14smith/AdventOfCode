using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2016;

[Day]
public partial class Day21 : ParseLineDay<Day21.Instruction, string, string>
{
    private const string Sample = "swap position 4 with position 0\nswap letter d with letter b\nreverse positions 0 through 4\nrotate left 1 step\nmove position 1 to position 4\nmove position 3 to position 0\nrotate based on position of letter b\nrotate based on position of letter d"; 
    
    private static readonly TextParser<Instruction> SwapPositionParser = Span.EqualTo("swap position ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" with position ")).Then(Numerics.IntegerInt32).Select(x => (Instruction) new Instruction.SwapPosition(x.Item1, x.Item2));
    private static readonly TextParser<Instruction> SwapLetterParser = Span.EqualTo("swap letter ").IgnoreThen(Character.AnyChar).ThenIgnore(Span.EqualTo(" with letter ")).Then(Character.AnyChar).Select(x => (Instruction) new Instruction.SwapLetter(x.Item1, x.Item2));
    private static readonly TextParser<Instruction> RotateLeftParser = Span.EqualTo("rotate left ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.Regex(" steps?")).Select(x => (Instruction) new Instruction.RotateLeft(x));
    private static readonly TextParser<Instruction> RotateRightParser = Span.EqualTo("rotate right ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.Regex(" steps?")).Select(x => (Instruction) new Instruction.RotateRight(x));
    private static readonly TextParser<Instruction> RotateOnLetterParser = Span.EqualTo("rotate based on position of letter ").IgnoreThen(Character.AnyChar).Select(x => (Instruction) new Instruction.RotateOnLetter(x));
    private static readonly TextParser<Instruction> ReverseRangeParser = Span.EqualTo("reverse positions ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" through ")).Then(Numerics.IntegerInt32).Select(x => (Instruction) new Instruction.ReverseRange(x.Item1, x.Item2));
    private static readonly TextParser<Instruction> MovePositionParser = Span.EqualTo("move position ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" to position ")).Then(Numerics.IntegerInt32).Select(x => (Instruction) new Instruction.MovePosition(x.Item1, x.Item2));
    
    private static readonly TextParser<Instruction> InstructionParser =
            SwapPositionParser.Try()
        .Or(SwapLetterParser.Try())
        .Or(RotateLeftParser.Try())
        .Or(RotateRightParser.Try())
        .Or(RotateOnLetterParser.Try())
        .Or(ReverseRangeParser.Try())
        .Or(MovePositionParser);

    protected override TextParser<Instruction> LineParser => InstructionParser;

    [Sample(Sample, "decab")]
    protected override string Part1(IEnumerable<Instruction> input)
    {
        var inputList = input.ToList();
        var text = inputList.Count == 8 ? "abcde" : "abcdefgh";
        var chars = text.ToCharArray();

        var result = chars.AsSpan();
        
        foreach (var instruction in inputList)
        {
            result = Apply(instruction, result);
        }
        
        return new string(result);
    }
    
    protected override string Part2(IEnumerable<Instruction> input)
    {
        var inputList = input.ToList();
        var text = "abcdefgh";
        var chars = text.ToCharArray();

        foreach (var combination in Permutations.Get(chars))
        {
            var start = combination.ToArray();
            
            var result = start.ToArray().AsSpan();
            foreach (var instruction in inputList)
            {
                result = Apply(instruction, result);
            }

            if (new string(result) == "fbgdceah")
            {
                return new string(start);
            }
        }

        throw new Exception("no solution");
    }
    
    private static Span<char> Apply(Instruction instruction, Span<char> text)
    {
        switch (instruction)
        {
            case Instruction.MovePosition movePosition:
                var a = text[movePosition.X];
                
                if(movePosition.X < movePosition.Y)
                {
                    text[(movePosition.X + 1)..(movePosition.Y + 1)].CopyTo(text[movePosition.X..]);
                    text[movePosition.Y] = a;
                }
                else
                {
                    text[movePosition.Y..movePosition.X].CopyTo(text[(movePosition.Y + 1)..]);
                    text[movePosition.Y] = a;
                }
                return text;

            case Instruction.ReverseRange(var start, var end):
                var len = (end - start + 1) / 2;
                
                for (var i = 0; i < len; i++)
                {
                    (text[start + i], text[end - i]) = (text[end - i], text[start + i]);
                }
                return text;
            
            case Instruction.RotateLeft rotateLeft:
                var rotateLeftStart = text[..rotateLeft.Offset].ToArray();
                var rotateLeftEnd = text[rotateLeft.Offset..];
                
                rotateLeftEnd.CopyTo(text);
                rotateLeftStart.CopyTo(text[^rotateLeft.Offset..]);
                
                return text;
            
            case Instruction.RotateRight rotateRight:
                var rotateRightStart = text[..^rotateRight.Offset];
                var rotateRightEnd = text[^rotateRight.Offset..].ToArray();
                
                rotateRightStart.CopyTo(text[rotateRight.Offset..]);
                rotateRightEnd.CopyTo(text);

                return text;
            
            case Instruction.RotateOnLetter rotateOnLetter:
                var rotateOnLetterX = text.IndexOf(rotateOnLetter.X);
                var rotateOnLetterOffset = rotateOnLetterX + 1 + (rotateOnLetterX >= 4 ? 1 : 0);
                return Apply(new Instruction.RotateRight(rotateOnLetterOffset % text.Length), text);

            case Instruction.SwapLetter swapLetter:
                var swapLetterX = text.IndexOf(swapLetter.X);
                var swapLetterY = text.IndexOf(swapLetter.Y);
                
                (text[swapLetterX], text[swapLetterY]) = (text[swapLetterY], text[swapLetterX]);
                return text;
                
            case Instruction.SwapPosition swapPosition:
                (text[swapPosition.X], text[swapPosition.Y]) = (text[swapPosition.Y], text[swapPosition.X]);
                return text;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(instruction));
        }
    }

    public abstract record Instruction
    {
        public record SwapPosition(int X, int Y) : Instruction;
        public record SwapLetter(char X, char Y) : Instruction;
        public record RotateLeft(int Offset) : Instruction;
        public record RotateRight(int Offset) : Instruction;
        public record RotateOnLetter(char X) : Instruction;
        public record ReverseRange(int X, int Y) : Instruction;
        public record MovePosition(int X, int Y) : Instruction;
    }
}
