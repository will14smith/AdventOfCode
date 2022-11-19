using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2020;

public partial class Day19
{
    protected override Tokenizer<TokenType> Tokenizer => new TokenizerBuilder<TokenType>()
        .Match(Character.EqualTo('\n'), TokenType.NewLine)
        .Ignore(Span.WhiteSpace)
        .Match(Numerics.Integer, TokenType.Number)
        .Match(Character.Letter.AtLeastOnce(), TokenType.Message)
        .Match(Character.EqualTo(':'), TokenType.Colon)
        .Match(Character.EqualTo('|'), TokenType.Pipe)
        .Match(Character.EqualTo('"'), TokenType.Quote)
        .Build();

    private static readonly TokenListParser<TokenType, int> Number = Token.EqualTo(TokenType.Number).Apply(Numerics.IntegerInt32); 
    private static readonly TokenListParser<TokenType, Rule> Reference = Number.Select(x => (Rule)new Rule.Reference(x)); 
    private static readonly TokenListParser<TokenType, Rule> Match = Token.Sequence(TokenType.Quote, TokenType.Message, TokenType.Quote).Select(xs => (Rule)new Rule.Match(xs[1].ToStringValue())); 
    private static readonly TokenListParser<TokenType, Rule> Sequence = Reference.AtLeastOnce().Select(Rule.CreateSequence); 
    private static readonly TokenListParser<TokenType, Rule> Alternative = Sequence.AtLeastOnceDelimitedBy(Token.EqualTo(TokenType.Pipe)).Select(Rule.CreateAlternative); 
    private static readonly TokenListParser<TokenType, Rule> RuleParser = Alternative.Or(Match); 
    private static readonly TokenListParser<TokenType, (int Tag, Rule Rule)> TaggedRule = Number.ThenIgnore(Token.EqualTo(TokenType.Colon)).Then(i => RuleParser.Select(rule => (i, rule))).ThenIgnore(Token.EqualTo(TokenType.NewLine));
    private static readonly TokenListParser<TokenType, Dictionary<int, Rule>> Rules = TaggedRule.AtLeastOnce().Select(rules => rules.ToDictionary(x => x.Tag, x => x.Rule));
    private static readonly TokenListParser<TokenType, string[]> Messages = Token.Sequence(TokenType.Message, TokenType.NewLine).Select(xs => xs[0].ToStringValue()).AtLeastOnce();
    private static readonly TokenListParser<TokenType, Spec> SpecParser = Rules.ThenIgnore(Token.EqualTo(TokenType.NewLine)).Then(rules => Messages.Select(messages => new Spec(rules, messages)));

    protected override TokenListParser<TokenType, Spec> Parser => SpecParser;

    public enum TokenType
    {
        NewLine,
        Number,
        Message,

        Colon,
        Pipe,
        Quote,
    }
}