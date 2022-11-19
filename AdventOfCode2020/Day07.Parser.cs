using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2020;

public partial class Day07
{
    protected override Tokenizer<Token> Tokenizer => new TokenizerBuilder<Token>()
        .Ignore(Span.WhiteSpace)
        .Match(Character.EqualTo('.'), Token.RuleEnd)
        .Match(Character.EqualTo(','), Token.ListSep)
        .Match(Numerics.Integer, Token.Number)
        .Match(Span.EqualTo("bag").Then(_ => Character.EqualTo('s').Optional()), Token.Bags)
        .Match(Span.EqualTo("contain"), Token.Contain)
        .Match(Span.EqualTo("no other"), Token.NoOther)
        .Match(Character.Letter.AtLeastOnce(), Token.Identifier)
        .Build();

    private static readonly TokenListParser<Token, string> Bag =
        from ident in Superpower.Parsers.Token.EqualTo(Token.Identifier).AtLeastOnce()
        from _bags in Superpower.Parsers.Token.EqualTo(Token.Bags)
        select string.Join(" ", ident.Select(x => x.ToStringValue()));

    private static readonly TokenListParser<Token, (int Count, string Bag)> CountedBag =
        from count in Superpower.Parsers.Token.EqualTo(Token.Number).Apply(Numerics.IntegerInt32)
        from bag in Bag
        select (count, bag);

    private static readonly TokenListParser<Token, Dictionary<string, int>> Inner =
        CountedBag.AtLeastOnceDelimitedBy(Superpower.Parsers.Token.EqualTo(Token.ListSep)).Select(xs => xs.ToDictionary(x => x.Bag, x => x.Count))
            .Or(Superpower.Parsers.Token.Sequence(Token.NoOther, Token.Bags).Select(_ => new Dictionary<string, int>()));

    private static readonly TokenListParser<Token, Rule> RuleParser =
        from outer in Bag
        from _contain in Superpower.Parsers.Token.EqualTo(Token.Contain)
        from inner in Inner
        from _end in Superpower.Parsers.Token.EqualTo(Token.RuleEnd)
        select new Rule(outer, inner);

    private static readonly TokenListParser<Token, Rule[]> Rules = RuleParser.AtLeastOnce();

    protected override TokenListParser<Token, Rule[]> Parser => Rules;

    public enum Token
    {
        RuleEnd,
        ListSep,
        Number,
        Bags,
        Contain,
        NoOther,
        Identifier,
    }
}