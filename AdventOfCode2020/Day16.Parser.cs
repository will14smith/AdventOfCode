using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2020;

public partial class Day16
{
    protected override Tokenizer<TokenType> Tokenizer => new TokenizerBuilder<TokenType>()
        .Ignore(Span.WhiteSpace)
        .Match(Span.EqualTo("or"), TokenType.Or)
        .Match(Span.EqualTo("your ticket"), TokenType.YourTicket)
        .Match(Span.EqualTo("nearby tickets"), TokenType.NearbyTickets)
        
        .Match(Character.EqualTo(':'), TokenType.Colon)
        .Match(Character.EqualTo('-'), TokenType.Dash)
        .Match(Character.EqualTo(','), TokenType.Comma)

        .Match(Character.Letter.AtLeastOnce(), TokenType.Identifier)
        .Match(Numerics.Integer, TokenType.Number)
        
        .Build();
    
    private static readonly TokenListParser<TokenType, long> Number = Token.EqualTo(TokenType.Number).Apply(Numerics.IntegerInt64);
    private static readonly TokenListParser<TokenType, (long Start, long End)> Range = Number.ThenIgnore(Token.EqualTo(TokenType.Dash)).Then(start => Number.Select(end => (start, end)));
    private static readonly TokenListParser<TokenType, (long Start, long End)[]> Ranges = Range.AtLeastOnceDelimitedBy(Token.EqualTo(TokenType.Or)); 
    
    private static readonly TokenListParser<TokenType, string> RuleName = Token.EqualTo(TokenType.Identifier).AtLeastOnce().Select(xs => string.Join(" ", xs.Select(x => x.ToStringValue()))); 
    private static readonly TokenListParser<TokenType, Rule> RuleParser = RuleName.ThenIgnore(Token.EqualTo(TokenType.Colon)).Then(name => Ranges.Select(ranges => new Rule(name, ranges))); 
    
    private static readonly TokenListParser<TokenType, Ticket> TicketParser = Number.AtLeastOnceDelimitedBy(Token.EqualTo(TokenType.Comma)).Select(x => new Ticket(x)); 

    private static readonly TokenListParser<TokenType, Spec> SpecParser = 
            from rules in RuleParser.AtLeastOnce()
            from _your in Token.Sequence(TokenType.YourTicket, TokenType.Colon)
            from your in TicketParser
            from _nearby in Token.Sequence(TokenType.NearbyTickets, TokenType.Colon)
            from nearby in TicketParser.AtLeastOnce()
            select new Spec(rules, your, nearby);

    protected override TokenListParser<TokenType, Spec> Parser => SpecParser;

    public enum TokenType
    {
        Or,
        YourTicket,
        NearbyTickets,
        
        Colon,
        Dash,
        Comma,
        
        Identifier,
        Number,
    }
}