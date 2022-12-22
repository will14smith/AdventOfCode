using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2022;

public partial class Day22
{
    protected override Tokenizer<TokenType> Tokenizer => new TokenizerBuilder<TokenType>()
        .Match(Character.EqualTo(' '), TokenType.Space)
        .Match(Character.EqualTo('\n'), TokenType.NewLine)
        .Match(Character.EqualTo('.'), TokenType.Dot)
        .Match(Character.EqualTo('#'), TokenType.Hash)
        .Match(Numerics.Integer, TokenType.Number)
        .Match(Character.EqualTo('L'), TokenType.Left)
        .Match(Character.EqualTo('R'), TokenType.Right)
        .Build();

    private static readonly TokenListParser<TokenType, Cell> CellParser = 
            Token.EqualTo(TokenType.Space).Select(_ => Cell.Void)
        .Or(Token.EqualTo(TokenType.Dot).Select(_ => Cell.Open))
        .Or(Token.EqualTo(TokenType.Hash).Select(_ => Cell.Wall));

    private static readonly TokenListParser<TokenType, Cell[]> MapLineParser = CellParser.AtLeastOnce().ThenIgnore(Token.EqualTo(TokenType.NewLine));
    private static readonly TokenListParser<TokenType, Cell[][]> MapParser = MapLineParser.Many().ThenIgnore(Token.EqualTo(TokenType.NewLine));
    
    private static readonly TokenListParser<TokenType, Instruction> InstructionParser = 
            Token.EqualTo(TokenType.Number).Select(x => (Instruction)new Instruction.Forward(int.Parse(x.ToStringValue())))
        .Or(Token.EqualTo(TokenType.Left).Select(_ => (Instruction)new Instruction.Left()))
        .Or(Token.EqualTo(TokenType.Right).Select(_ => (Instruction)new Instruction.Right()));
    private static readonly TokenListParser<TokenType, Instruction[]> InstructionsParser = InstructionParser.AtLeastOnce(); 

    protected override TokenListParser<TokenType, Model> Parser => MapParser.Then(InstructionsParser).Select(x => Model.Create(x.Item1, x.Item2));
    
    public enum TokenType
    {
        Space,
        NewLine,
        Dot,
        Hash,
        Number,
        Left,
        Right,
    }
}