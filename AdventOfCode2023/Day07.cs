namespace AdventOfCode2023;

[Day]
public partial class Day07 : LineDay<Day07.Model, long, long>
{
    protected override Model ParseLine(string input)
    {
        var parts = input.Split(' ');
        return new Model(parts[0], long.Parse(parts[1]));
    }
    
    [Sample("32T3K 765\nT55J5 684\nKK677 28\nKTJJT 220\nQQQJA 483", 6440L)]
    protected override long Part1(IEnumerable<Model> input) => input.OrderBy(x => x, RankingComparerPart1.Instance).Select((x, i) => x.Score * (i + 1)).Sum();

    [Sample("32T3K 765\nT55J5 684\nKK677 28\nKTJJT 220\nQQQJA 483", 5905L)]
    protected override long Part2(IEnumerable<Model> input) => input.OrderBy(x => x, RankingComparerPart2.Instance).Select((x, i) => x.Score * (i + 1)).Sum();

    public record Model(string Hand, long Score);
    
    private sealed class RankingComparerPart1 : IComparer<Model>
    {
        public static readonly RankingComparerPart1 Instance = new();

        private static readonly string Ranking = "23456789TJQKA";
        
        public int Compare(Model? x, Model? y)
        {
            if (x is null) throw new ArgumentNullException(nameof(x));
            if (y is null) throw new ArgumentNullException(nameof(y));

            return Compare(x.Hand, y.Hand);
        }

        private int Compare(string x, string y)
        {
            var xType = Type(x);
            var yType = Type(y);

            if (xType != yType)
            {
                return xType.CompareTo(yType);
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

        private int Type(string hand)
        {
            var counts = hand.GroupBy(x => x, x => x).Select(x => x.Count()).OrderByDescending(x => x).ToList();

            if (counts[0] == 5) return 6;
            if (counts[0] == 4) return 5;
            if (counts[0] == 3 && counts[1] == 2) return 4;
            if (counts[0] == 3) return 3;
            if (counts[0] == 2 && counts[1] == 2) return 2;
            if (counts[0] == 2) return 1;
            return 0;
        }
    }
    
    private sealed class RankingComparerPart2 : IComparer<Model>
    {
        public static readonly RankingComparerPart2 Instance = new();

        private static readonly string Ranking = "J23456789TQKA";
        
        public int Compare(Model? x, Model? y)
        {
            if (x is null) throw new ArgumentNullException(nameof(x));
            if (y is null) throw new ArgumentNullException(nameof(y));

            return Compare(x.Hand, y.Hand);
        }

        private int Compare(string x, string y)
        {
            var xType = Type(x);
            var yType = Type(y);

            if (xType != yType)
            {
                return xType.CompareTo(yType);
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

        private int Type(string hand)
        {
            if (!hand.Contains('J'))
            {
                return Calculate(hand);
            }

            var letters = hand.Distinct();

            return letters.Select(x => hand.Replace('J', x)).Select(Calculate).Max();
            
            static int Calculate(string hand)
            {
                var counts = hand.GroupBy(x => x, x => x).Select(x => x.Count()).OrderByDescending(x => x).ToList();

                if (counts[0] == 5) return 6;
                if (counts[0] == 4) return 5;
                if (counts[0] == 3 && counts[1] == 2) return 4;
                if (counts[0] == 3) return 3;
                if (counts[0] == 2 && counts[1] == 2) return 2;
                if (counts[0] == 2) return 1;
                return 0;
            }
        }
    }

}