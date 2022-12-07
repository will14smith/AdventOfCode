using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2022;

public partial class Day07
{
    public enum TokenType
    {
        Dollar,
        Cd,
        Ls,
        Dir,
        FileName,
        Size,
    }

    protected override Tokenizer<TokenType> Tokenizer { get; } = new TokenizerBuilder<TokenType>()
        .Ignore(Span.WhiteSpace)
        .Match(Span.EqualTo('$'), TokenType.Dollar)
        .Match(Span.EqualTo("cd"), TokenType.Cd, requireDelimiters: true)
        .Match(Span.EqualTo("ls"), TokenType.Ls, requireDelimiters: true)
        .Match(Span.EqualTo("dir"), TokenType.Dir, requireDelimiters: true)
        .Match(Span.Regex("[/.a-zA-Z]+"), TokenType.FileName, requireDelimiters: true)
        .Match(Numerics.Integer, TokenType.Size)
        .Build();

    private static readonly TokenListParser<TokenType, FileInfo> LsFileLineParser = Token.Sequence(TokenType.Size, TokenType.FileName).Select(x => (FileInfo)new FileInfo.File(x[1].ToStringValue(), int.Parse(x[0].ToStringValue())));
    private static readonly TokenListParser<TokenType, FileInfo> LsDirLineParser = Token.Sequence(TokenType.Dir, TokenType.FileName).Select(x => (FileInfo)new FileInfo.Directory(x[1].ToStringValue()));
    private static readonly TokenListParser<TokenType, FileInfo> LsLineParser = LsFileLineParser.Or(LsDirLineParser);
    private static readonly TokenListParser<TokenType, FileInfo[]> LsBodyParser = LsLineParser.Many();

    private static readonly TokenListParser<TokenType, Command> CommandCdParser = Token.Sequence(TokenType.Cd, TokenType.FileName).Select(x => (Command) new Command.ChangeDirectory(x[1].ToStringValue()));
    private static readonly TokenListParser<TokenType, Command> CommandLsParser = Token.Sequence(TokenType.Ls).IgnoreThen(LsBodyParser).Select(x => (Command) new Command.ListDirectory(x));
    private static readonly TokenListParser<TokenType, Command> CommandParser = Token.EqualTo(TokenType.Dollar).IgnoreThen(CommandCdParser.Try().Or(CommandLsParser));

    private static readonly TokenListParser<TokenType, Model> ModelParser = CommandParser.Many().Select(x => new Model(x));

    protected override TokenListParser<TokenType, Model> Parser => ModelParser;
}