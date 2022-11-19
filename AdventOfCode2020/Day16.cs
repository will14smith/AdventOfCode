namespace AdventOfCode2020;

[Day]
public partial class Day16 : ParseDay<Day16.Spec, Day16.TokenType, long, long>
{
    private const string Sample1 = "class: 1-3 or 5-7\nrow: 6-11 or 33-44\nseat: 13-40 or 45-50\n\nyour ticket:\n7,1,14\n\nnearby tickets:\n7,3,47\n40,4,50\n55,2,20\n38,6,12\n";

    [Sample(Sample1, 71L)]
    protected override long Part1(Spec input) => 
        input.Nearby.SelectMany(ticket => ticket.Values)
            .Where(field => !IsValid(input.Rules, field))
            .Sum();

    protected override long Part2(Spec input) =>
        SolvePart2Assignments(input)
            .Where(x => x.Key.StartsWith("departure"))
            .Aggregate(1L, (a, b) => a * input.Your.Values[b.Value]);
    
    private static IReadOnlyDictionary<string, int> SolvePart2Assignments(Spec spec)
    {
        var (rules, your, tickets) = spec;
        var validTickets = tickets.Where(ticket => IsValid(rules, ticket)).Prepend(your).ToList();

        var ruleCandidates = rules.ToDictionary(rule => rule.Name, rule => ValidAssignments(validTickets, rule).ToList());

        var assignments = new Dictionary<string, int>();
        var used = new HashSet<int>();
            
        while (ruleCandidates.Count > 0)
        {
            var (rule, candidates) = ruleCandidates.Select(x => (Rule: x.Key, Candidates: StillValidAssignments(x.Value))).First(x => x.Candidates.Count == 1);
                
            var index = candidates[0];
            assignments[rule] = index;
            used.Add(index);
                        
            ruleCandidates.Remove(rule);
        }

        return assignments;

        IReadOnlyList<int> StillValidAssignments(IEnumerable<int> assignments) => assignments.Where(i => !used.Contains(i)).ToList(); 
    }
    
    private static bool IsValid(IEnumerable<Rule> rules, Ticket ticket) => ticket.Values.All(value => IsValid(rules, value));

    private static bool IsValid(IEnumerable<Rule> rules, long value) => rules.Any(rule => IsValid(rule, value));
    private static bool IsValid(Rule rule, long value) => rule.ValidRanges.Any(range => range.Item1 <= value && value <= range.Item2);

    private static IEnumerable<int> ValidAssignments(IReadOnlyList<Ticket> tickets, Rule rule) => Enumerable.Range(0, tickets[0].Values.Count).Where(i => AssignmentIsValid(tickets, rule, i));
    private static bool AssignmentIsValid(IEnumerable<Ticket> tickets, Rule rule, int index) => tickets.Select(ticket => ticket.Values[index]).All(value => IsValid(rule, value));
    
    public record Rule(string Name, IReadOnlyList<(long Start, long End)> ValidRanges);
    public record Ticket(IReadOnlyList<long> Values);
    public record Spec(IReadOnlyList<Rule> Rules, Ticket Your, IReadOnlyList<Ticket> Nearby);
}
