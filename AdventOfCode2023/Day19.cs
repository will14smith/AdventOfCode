using System.Collections.Immutable;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2023;

[Day]
public partial class Day19 : ParseDay<Day19.Model, int, long>
{
    public static TextParser<Rule> RuleAcceptParser = Span.EqualTo('A').Select(_ => (Rule) new Rule.Accept());
    public static TextParser<Rule> RuleRejectParser = Span.EqualTo('R').Select(_ => (Rule) new Rule.Reject());
    public static TextParser<Rule> RuleConditionalParser = Span.Regex("[xmas]").Then(Span.Regex("[<>]")).Then(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(':')).Then(Span.Regex("A|R|[a-z]+"))
        .Select(x =>
        {
            var variable = x.Item1.Item1.Item1.ToStringValue()[0];
            var lessThan = x.Item1.Item1.Item2.ToStringValue() == "<";
            var value = x.Item1.Item2;
            var action = x.Item2.ToStringValue();

            return action switch
            {
                "A" => (Rule) new Rule.ConditionalAccept(new Condition(variable, lessThan, value)),
                "R" => new Rule.ConditionalReject(new Condition(variable, lessThan, value)),
                _ => new Rule.ConditionalWorkflow(new Condition(variable, lessThan, value), action)
            };
        });
    public static TextParser<Rule> RuleWorkflowParser = Span.Regex("[a-z]+").Select(x => (Rule) new Rule.Workflow(x.ToStringValue()));
    public static TextParser<Rule> RuleParser = RuleAcceptParser.Or(RuleRejectParser).Or(RuleConditionalParser.Try()).Or(RuleWorkflowParser);
    public static TextParser<Workflow> WorkflowParser = Span.Regex("[a-z]+").ThenIgnore(Span.EqualTo('{')).Then(RuleParser.ManyDelimitedBy(Span.EqualTo(','))).ThenIgnore(Span.EqualTo("}\n")).Select(x => new Workflow(x.Item1.ToStringValue(), x.Item2));
    
    public static TextParser<Part> PartParser = Span.EqualTo("{x=").IgnoreThen(Numerics.IntegerInt32)
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

        return input.Parts.Where(part => ShouldAccept(workflows, part)).Sum(part => part.X + part.M + part.A + part.S);
    }

    private bool ShouldAccept(Dictionary<string, Workflow> workflows, Part part)
    {
        var current = workflows["in"];

        while (true)
        {
            foreach (var rule in current.Rules)
            {
                switch (rule)
                {
                    case Rule.Accept: return true;
                    case Rule.Reject: return false;
                    case Rule.Workflow workflow: current = workflows[workflow.TargetWorkflow]; goto next;

                    case Rule.ConditionalAccept conditionalAccept when MatchesCondition(conditionalAccept.Condition):
                        return true;
                    case Rule.ConditionalReject conditionalReject when MatchesCondition(conditionalReject.Condition):
                        return false;
                    case Rule.ConditionalWorkflow conditionalWorkflow when MatchesCondition(conditionalWorkflow.Condition):
                        current = workflows[conditionalWorkflow.TargetWorkflow]; goto next;
                }
            }
            
            next: { }
        }

        bool MatchesCondition(Condition condition) => condition.Variable switch
        {
            'x' => condition.LessThan ? part.X < condition.Value : part.X > condition.Value,
            'm' => condition.LessThan ? part.M < condition.Value : part.M > condition.Value,
            'a' => condition.LessThan ? part.A < condition.Value : part.A > condition.Value,
            's' => condition.LessThan ? part.S < condition.Value : part.S > condition.Value,
        };
    }

    [Sample("in{A}\n\n{x=1,m=1,a=1,s=1}", 256000000000000L)]
    [Sample("px{a<2006:qkq,m>2090:A,rfg}\npv{a>1716:R,A}\nlnx{m>1548:A,A}\nrfg{s<537:gd,x>2440:R,A}\nqs{s>3448:A,lnx}\nqkq{x<1416:A,crn}\ncrn{x>2662:A,R}\nin{s<1351:px,qqz}\nqqz{s>2770:qs,m<1801:hdj,R}\ngd{a>3333:R,R}\nhdj{m>838:A,pv}\n\n{x=787,m=2655,a=1222,s=2876}\n{x=1679,m=44,a=2067,s=496}\n{x=2036,m=264,a=79,s=2244}\n{x=2461,m=1339,a=466,s=291}\n{x=2127,m=1623,a=2188,s=1013}", 167409079868000L)]
    protected override long Part2(Model input)
    {
        var workflows = input.Workflows.ToDictionary(x => x.Name);

        var initial = new PartRange(new Range(1, 4000), new Range(1, 4000), new Range(1, 4000), new Range(1, 4000));
        var ranges = Build(workflows, workflows["in"], new [] { initial });

        return ranges.Sum(range =>
        {
            var x = range.X.Max - range.X.Min + 1;
            var m = range.M.Max - range.M.Min + 1;
            var a = range.A.Max - range.A.Min + 1;
            var s = range.S.Max - range.S.Min + 1;

            return (long)x * m * a * s;
        });
    }

    private static IEnumerable<PartRange> Build(Dictionary<string, Workflow> workflows, Workflow current, IEnumerable<PartRange> parts)
    {
        var output = Enumerable.Empty<PartRange>();
        
        foreach (var rule in current.Rules)
        {
            switch (rule)
            {
                case Rule.Accept: return output.Concat(parts);
                case Rule.Reject: return output;
                case Rule.Workflow workflow: return output.Concat(Build(workflows, workflows[workflow.TargetWorkflow], parts));

                case Rule.ConditionalAccept accept:
                {
                    var (matchedParts, unmatchedParts) = MatchCondition(accept.Condition, parts);
                    output = output.Concat(matchedParts);
                    parts = unmatchedParts;
                    break;
                }
                case Rule.ConditionalReject reject:
                {
                    var (_, unmatchedParts) = MatchCondition(reject.Condition, parts);
                    parts = unmatchedParts;
                    break;
                }          
                case Rule.ConditionalWorkflow workflow:
                {
                    var (matchedParts, unmatchedParts) = MatchCondition(workflow.Condition, parts);
                    output = output.Concat(Build(workflows, workflows[workflow.TargetWorkflow], matchedParts));
                    parts = unmatchedParts;
                    break;
                }
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(rule));
            }
        }

        return output;
        
        static (IEnumerable<PartRange> MatchedParts, IEnumerable<PartRange> UnmatchedParts) MatchCondition(Condition condition, IEnumerable<PartRange> parts)
        {
            var matched = new List<PartRange>();
            var unmatched = new List<PartRange>();
            
            switch (condition)
            {
                case { Variable: 'x', LessThan: true }:
                {
                    foreach (var part in parts)
                    {
                        var mr = part.X with { Max = condition.Value - 1 };
                        if (mr.Valid) matched.Add(part with { X = mr });

                        var ur = part.X with { Min = condition.Value };
                        if (ur.Valid) unmatched.Add(part with { X = ur });
                    }

                    return (matched, unmatched);
                }
                case { Variable: 'm', LessThan: true }:
                {
                    foreach (var part in parts)
                    {
                        var mr = part.M with { Max = condition.Value - 1 };
                        if (mr.Valid) matched.Add(part with { M = mr });

                        var ur = part.M with { Min = condition.Value };
                        if (ur.Valid) unmatched.Add(part with { M = ur });
                    }

                    return (matched, unmatched);
                }
                case { Variable: 'a', LessThan: true }:
                {
                    foreach (var part in parts)
                    {
                        var mr = part.A with { Max = condition.Value - 1 };
                        if (mr.Valid) matched.Add(part with { A = mr });

                        var ur = part.A with { Min = condition.Value };
                        if (ur.Valid) unmatched.Add(part with { A = ur });
                    }

                    return (matched, unmatched);
                }
                case { Variable: 's', LessThan: true }:
                {
                    foreach (var part in parts)
                    {
                        var mr = part.S with { Max = condition.Value - 1 };
                        if (mr.Valid) matched.Add(part with { S = mr });

                        var ur = part.S with { Min = condition.Value };
                        if (ur.Valid) unmatched.Add(part with { S = ur });
                    }

                    return (matched, unmatched);
                }
                
                case { Variable: 'x', LessThan: false }:
                {
                    foreach (var part in parts)
                    {
                        var mr = part.X with { Min = condition.Value + 1 };
                        if (mr.Valid) matched.Add(part with { X = mr });

                        var ur = part.X with { Max = condition.Value };
                        if (ur.Valid) unmatched.Add(part with { X = ur });
                    }

                    return (matched, unmatched);
                }
                case { Variable: 'm', LessThan: false }:
                {
                    foreach (var part in parts)
                    {
                        var mr = part.M with { Min = condition.Value + 1 };
                        if (mr.Valid) matched.Add(part with { M = mr });

                        var ur = part.M with { Max = condition.Value };
                        if (ur.Valid) unmatched.Add(part with { M = ur });
                    }

                    return (matched, unmatched);
                }
                case { Variable: 'a', LessThan: false }:
                {
                    foreach (var part in parts)
                    {
                        var mr = part.A with { Min = condition.Value + 1 };
                        if (mr.Valid) matched.Add(part with { A = mr });

                        var ur = part.A with { Max = condition.Value };
                        if (ur.Valid) unmatched.Add(part with { A = ur });
                    }

                    return (matched, unmatched);
                }
                case { Variable: 's', LessThan: false }:
                {
                    foreach (var part in parts)
                    {
                        var mr = part.S with { Min = condition.Value + 1 };
                        if (mr.Valid) matched.Add(part with { S = mr });

                        var ur = part.S with { Max = condition.Value };
                        if (ur.Valid) unmatched.Add(part with { S = ur });
                    }

                    return (matched, unmatched);
                }
                
                default: throw new NotImplementedException(condition.ToString());
            }
        }
    }

    public record Model(IReadOnlyList<Workflow> Workflows, IReadOnlyList<Part> Parts);

    public record Workflow(string Name, IReadOnlyList<Rule> Rules);
    public abstract record Rule
    {
        public record Workflow(string TargetWorkflow) : Rule;
        public record Accept : Rule;
        public record Reject : Rule;
        public record ConditionalWorkflow(Condition Condition, string TargetWorkflow) : Rule;
        public record ConditionalAccept(Condition Condition) : Rule;
        public record ConditionalReject(Condition Condition) : Rule;
    }
    public record Condition(char Variable, bool LessThan, int Value);

    public record Part(int X, int M, int A, int S);

    public record Range(int Min, int Max)
    {
        public bool Valid => Min <= Max;
    }
    public record PartRange(Range X, Range M, Range A, Range S);
}