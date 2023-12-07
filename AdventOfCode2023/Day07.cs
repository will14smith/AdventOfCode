namespace AdventOfCode2023;

[Day]
public partial class Day07 : LineDay<Day07.Model, int, int>
{
    protected override Model ParseLine(string input)
    {
        var parts = input.Split(' ');
        return new Model(parts[0], int.Parse(parts[1]));
    }
    
    [Sample("32T3K 765\nT55J5 684\nKK677 28\nKTJJT 220\nQQQJA 483", 6440)]
    protected override int Part1(IEnumerable<Model> input) => Solve(input, RankingComparerPart1.Instance);
    [Sample("32T3K 765\nT55J5 684\nKK677 28\nKTJJT 220\nQQQJA 483", 5905)]
    protected override int Part2(IEnumerable<Model> input) => Solve(input, RankingComparerPart2.Instance);

    private static int Solve(IEnumerable<Model> input, RankingComparer ranker) => input.OrderBy(x => x, ranker).Select((x, i) => x.Score * (i + 1)).Sum();
    
    public record Model(string Hand, int Score);
    
    private abstract class RankingComparer : IComparer<Model>
    {
        private string Ranking { get; } 
        
        protected RankingComparer(string ranking)
        {
            Ranking = ranking;
        }
        
        public int Compare(Model? x, Model? y)
        {
            if (x is null) throw new ArgumentNullException(nameof(x));
            if (y is null) throw new ArgumentNullException(nameof(y));

            return Compare(x.Hand, y.Hand);
        }

        private int Compare(string x, string y)
        {
            var xTypeScore = TypeScore(x);
            var yTypeScore = TypeScore(y);

            if (xTypeScore != yTypeScore)
            {
                return xTypeScore.CompareTo(yTypeScore);
            }
            
            for (var i = 0; i < 5; i++)
            {                
                if(x[i] == y[i]) continue;

                var xScore = Ranking.IndexOf(x[i]);
                var yScore = Ranking.IndexOf(y[i]);
                
                return xScore.CompareTo(yScore);
            }

            return 0;
        }

        protected virtual int TypeScore(string hand)
        {
            var counts = hand.GroupBy(x => x, x => x).Select(x => x.Count()).OrderByDescending(x => x).ToList();

            return counts[0] switch
            {
                5 => 6,                     // 5-of-a-kind
                4 => 5,                     // 4-of-a-kind
                3 when counts[1] == 2 => 4, // full house
                3 => 3,                     // 3-of-a-kind
                2 when counts[1] == 2 => 2, // 2 pairs
                2 => 1,                     // 1 pair
                _ => 0                      // high card
            };
        }
    }
    
    private sealed class RankingComparerPart1 : RankingComparer
    {
        public static readonly RankingComparerPart1 Instance = new();
        private RankingComparerPart1() : base("23456789TJQKA") { }
    }
    
    private sealed class RankingComparerPart2 : RankingComparer
    {
        public static readonly RankingComparerPart2 Instance = new();
        public RankingComparerPart2() : base("J23456789TQKA") { }

        protected override int TypeScore(string hand)
        {
            if (!hand.Contains('J'))
            {
                return base.TypeScore(hand);
            }

            var letters = hand.Distinct();
            return letters.Select(x => hand.Replace('J', x)).Select(base.TypeScore).Max();
        }
    }
}