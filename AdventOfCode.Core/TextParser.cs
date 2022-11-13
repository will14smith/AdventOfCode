using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace AdventOfCode.Core; 

public static class SuperpowerExtensions
{
    public static readonly TextParser<string> NewLine = Character.EqualTo('\n').Select(_ => "\n").Or(Character.EqualTo('\r').Then(_ => Character.EqualTo('\n').Select(_ => "\n")));
    public static readonly TextParser<string> Name = Identifier.CStyle.Select(x => x.ToStringValue());

    public static T MustParse<T>(this TextParser<T> parser, string input)
    {
        var result = parser(new TextSpan(input));
        if(!result.HasValue) throw new Exception(result.ToString());

        return result.Value;
    }

    public static T MustParse<TToken, T>(this TokenListParser<TToken, T> parser, Tokenizer<TToken> tokenizer, string input)
    {
        var tokens = tokenizer.Tokenize(input);
        var result = parser(tokens);
        if(!result.HasValue) throw new Exception(result.ToString());

        return result.Value;
    }

    public static TextParser<T> IgnoreWhitespace<T>(this TextParser<T> parser)
    {
        return
            from _1 in Character.WhiteSpace.Many()
            from result in parser
            from _2 in Character.WhiteSpace.Many()
            select result;
    }

    public static TextParser<TResult> Then<T, T2, TResult>(this TextParser<T> parser1, TextParser<T2> parser2, Func<T, T2, TResult> combine)
    {
        return
            from result1 in parser1
            from result2 in parser2
            select combine(result1, result2);
    }  
    public static TextParser<(T1, T2)> Then<T1, T2>(this TextParser<T1> parser1, TextParser<T2> parser2)
    {
        return parser1.Then(parser2, (a, b) => (a, b));
    }     
    
    public static TokenListParser<TToken, TResult> Then<TToken, T, T2, TResult>(this TokenListParser<TToken, T> parser1, TokenListParser<TToken, T2> parser2, Func<T, T2, TResult> combine)
    {
        return
            from result1 in parser1
            from result2 in parser2
            select combine(result1, result2);
    }  
    public static TokenListParser<TToken, (T1, T2)> Then<TToken, T1, T2>(this TokenListParser<TToken, T1> parser1, TokenListParser<TToken, T2> parser2)
    {
        return parser1.Then(parser2, (a, b) => (a, b));
    }     
    
    public static TextParser<T> ThenIgnore<T, T2>(this TextParser<T> parser, TextParser<T2> ignoreParser)
    {
        return
            from result in parser
            from _ in ignoreParser
            select result;
    }     
    
    public static TokenListParser<TToken, T> ThenIgnore<TToken, T, T2>(this TokenListParser<TToken, T> parser, TokenListParser<TToken, T2> ignoreParser)
    {
        return
            from result in parser
            from _ in ignoreParser
            select result;
    }

    public static TextParser<T> ThenIgnoreOptional<T, T2>(this TextParser<T> parser, TextParser<T2> ignoreParser)
    {
        return
            from result in parser
            from _ in ignoreParser.OptionalOrDefault()
            select result;
    }
}