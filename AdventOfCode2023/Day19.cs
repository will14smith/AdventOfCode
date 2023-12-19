using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2023;

[Day]
public partial class Day19 : ParseDay<Day19.Model, int, long>
{
    private static readonly TextParser<Action> ActionAcceptParser = Span.EqualTo('A').Select(_ => (Action) new Action.Accept());
    private static readonly TextParser<Action> ActionRejectParser = Span.EqualTo('R').Select(_ => (Action) new Action.Reject());
    private static readonly TextParser<Action> ActionWorkflowParser = Span.Regex("[a-z]+").Select(x => (Action) new Action.Next(x.ToStringValue()));
    private static readonly TextParser<Action> ActionParser = ActionAcceptParser.Or(ActionRejectParser).Or(ActionWorkflowParser);

    private static readonly TextParser<Rule> RuleConditionalParser = Span.Regex("[xmas]").Then(Span.Regex("[<>]")).Then(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(':')).Then(ActionParser)
        .Select(x =>
        {
            var variable = x.Item1.Item1.Item1.ToStringValue()[0];
            var lessThan = x.Item1.Item1.Item2.ToStringValue() == "<";
            var value = x.Item1.Item2;
            var condition = new Condition(variable, lessThan, value);
            
            var action = x.Item2;

            return new Rule(condition, action);
        });
    private static readonly TextParser<Rule> RuleUnconditionalParser = ActionParser.Select(x => new Rule(null, x));
    private static readonly TextParser<Rule> RuleParser = RuleConditionalParser.Try().Or(RuleUnconditionalParser);
    
    private static readonly TextParser<Workflow> WorkflowParser = Span.Regex("[a-z]+").ThenIgnore(Span.EqualTo('{')).Then(RuleParser.ManyDelimitedBy(Span.EqualTo(','))).ThenIgnore(Span.EqualTo("}\n")).Select(x => new Workflow(x.Item1.ToStringValue(), x.Item2));
    
    private static readonly TextParser<Part> PartParser = Span.EqualTo("{x=").IgnoreThen(Numerics.IntegerInt32)
            .ThenIgnore(Span.EqualTo(",m=")).Then(Numerics.IntegerInt32)
            .ThenIgnore(Span.EqualTo(",a=")).Then(Numerics.IntegerInt32)
            .ThenIgnore(Span.EqualTo(",s=")).Then(Numerics.IntegerInt32)
            .ThenIgnore(Span.EqualTo("}"))
            .Select(x => new Part(x.Item1.Item1.Item1, x.Item1.Item1.Item2, x.Item1.Item2, x.Item2))
        ;

    protected override TextParser<Model> Parser { get; } = WorkflowParser.Many().ThenIgnore(Span.EqualTo('\n')).Then(PartParser.ManyDelimitedBy(Span.EqualTo('\n'))).Select(x => new Model(x.Item1, x.Item2));

    [Sample("px{a<2006:qkq,m>2090:A,rfg}\npv{a>1716:R,A}\nlnx{m>1548:A,A}\nrfg{s<537:gd,x>2440:R,A}\nqs{s>3448:A,lnx}\nqkq{x<1416:A,crn}\ncrn{x>2662:A,R}\nin{s<1351:px,qqz}\nqqz{s>2770:qs,m<1801:hdj,R}\ngd{a>3333:R,R}\nhdj{m>838:A,pv}\n\n{x=787,m=2655,a=1222,s=2876}\n{x=1679,m=44,a=2067,s=496}\n{x=2036,m=264,a=79,s=2244}\n{x=2461,m=1339,a=466,s=291}\n{x=2127,m=1623,a=2188,s=1013}", 19114)]
    protected override int Part1(Model input)
    {
        var workflows = input.Workflows.ToDictionary(x => x.Name);
        var initialWorkflow = workflows["in"];
        return input.Parts.Where(part => ShouldAccept(workflows, initialWorkflow, part)).Sum(part => part.X + part.M + part.A + part.S);
    }

    private static bool ShouldAccept(IReadOnlyDictionary<string, Workflow> workflows, Workflow current, Part part)
    {
        foreach (var rule in current.Rules)
        {
            if (!MatchesCondition(rule.Condition))
            {
                continue;
            }
            
            switch (rule.Action)
            {
                case Action.Accept: return true;
                case Action.Reject: return false;
                case Action.Next next: return ShouldAccept(workflows, workflows[next.TargetWorkflow], part);
            }
        }

        throw new Exception("no.");
        
        bool MatchesCondition(Condition? condition) => condition?.Variable switch
        {
            'x' => condition.LessThan ? part.X < condition.Value : part.X > condition.Value,
            'm' => condition.LessThan ? part.M < condition.Value : part.M > condition.Value,
            'a' => condition.LessThan ? part.A < condition.Value : part.A > condition.Value,
            's' => condition.LessThan ? part.S < condition.Value : part.S > condition.Value,
            
            null => true,
            
            _ => throw new Exception("no.")
        };
    }

    [Sample("in{A}\n\n{x=1,m=1,a=1,s=1}", 256000000000000L)]
    [Sample("px{a<2006:qkq,m>2090:A,rfg}\npv{a>1716:R,A}\nlnx{m>1548:A,A}\nrfg{s<537:gd,x>2440:R,A}\nqs{s>3448:A,lnx}\nqkq{x<1416:A,crn}\ncrn{x>2662:A,R}\nin{s<1351:px,qqz}\nqqz{s>2770:qs,m<1801:hdj,R}\ngd{a>3333:R,R}\nhdj{m>838:A,pv}\n\n{x=787,m=2655,a=1222,s=2876}\n{x=1679,m=44,a=2067,s=496}\n{x=2036,m=264,a=79,s=2244}\n{x=2461,m=1339,a=466,s=291}\n{x=2127,m=1623,a=2188,s=1013}", 167409079868000L)]
    protected override long Part2(Model input)
    {
        var workflows = input.Workflows.ToDictionary(x => x.Name);

        var initial = new PartRange(new Range(1, 4000), new Range(1, 4000), new Range(1, 4000), new Range(1, 4000));
        var ranges = FindAcceptedRanges(workflows, workflows["in"], new [] { initial });

        return ranges.Sum(range => range.X.Size * range.M.Size * range.A.Size * range.S.Size);
    }

    private static IEnumerable<PartRange> FindAcceptedRanges(IReadOnlyDictionary<string, Workflow> workflows, Workflow current, IEnumerable<PartRange> parts)
    {
        var output = Enumerable.Empty<PartRange>();
        
        foreach (var rule in current.Rules)
        {
            var (matchedParts, unmatchedParts) = SplitRanges(rule.Condition, parts);
            parts = unmatchedParts;
            
            switch (rule.Action)
            {
                case Action.Accept: output = output.Concat(matchedParts); break;
                case Action.Reject: break;
                case Action.Next next: output = output.Concat(FindAcceptedRanges(workflows, workflows[next.TargetWorkflow], matchedParts)); break;
                
                default: throw new ArgumentOutOfRangeException(nameof(rule));
            }
        }

        return output;
        
        static (IEnumerable<PartRange> MatchedParts, IEnumerable<PartRange> UnmatchedParts) SplitRanges(Condition? condition, IEnumerable<PartRange> parts)
        {
            if (condition == null)
            {
                return (parts, Enumerable.Empty<PartRange>());
            }
            
            var matched = new List<PartRange>();
            var unmatched = new List<PartRange>();

            foreach (var part in parts)
            {
                var (matchedRange, unmatchedRange) = SplitRange(condition, part[condition.Variable]);
                
                matched.Add(part.With(condition.Variable, matchedRange));
                unmatched.Add(part.With(condition.Variable, unmatchedRange));
            }

            return (matched, unmatched);
        }

        static (Range Matched, Range Unmatched) SplitRange(Condition condition, Range range) => 
            condition.LessThan 
                ? (range with { Max = condition.Value - 1 }, range with { Min = condition.Value }) 
                : (range with { Min = condition.Value + 1 }, range with { Max = condition.Value });
    }

    public record Model(IReadOnlyList<Workflow> Workflows, IReadOnlyList<Part> Parts);

    public record Workflow(string Name, IReadOnlyList<Rule> Rules);
    public record Rule(Condition? Condition, Action Action);
    public record Condition(char Variable, bool LessThan, int Value);
    public abstract record Action
    {
        public record Next(string TargetWorkflow) : Action;
        public record Accept : Action;
        public record Reject : Action;
    }

    public record Part(int X, int M, int A, int S);

    public record Range(int Min, int Max)
    {
        public long Size => Min > Max ? 0 : Max - Min + 1;
    }
    public record PartRange(Range X, Range M, Range A, Range S)
    {
        public Range this[char variable] => variable switch
        {
            'x' => X,
            'm' => M,
            'a' => A,
            's' => S,
        };

        public PartRange With(char variable, Range value) => variable switch
        {
            'x' => this with { X = value },
            'm' => this with { M = value },
            'a' => this with { A = value },
            's' => this with { S = value },
        };
    }
}