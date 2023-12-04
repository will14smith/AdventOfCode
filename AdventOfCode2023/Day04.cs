namespace AdventOfCode2023;

[Day]
public partial class Day04 : ParseDay<Day04.Model, Day04.TokenType, int, int>
{
    private const string Sample = "Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53\nCard 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19\nCard 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1\nCard 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83\nCard 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36\nCard 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11";

    [Sample(Sample, 13)]
    protected override int Part1(Model input)
    {
        var points = 0;

        foreach (var card in input.Cards)
        {
            var count = CountMatches(card);
            points += count == 0 ? 0 : (int)Math.Pow(2, count - 1);
        }
        
        return points;
    }

    [Sample(Sample, 30)]
    protected override int Part2(Model input)
    {
        var cardCounts = input.Cards.ToDictionary(x => x.Id, _ => 1);

        var count = 0;
        foreach (var card in input.Cards)
        {
            var cardCount = cardCounts[card.Id];
            count += cardCount;
            
            var matches = CountMatches(card);
            for (var i = 0; i < matches; i++)
            {
                var nextId = card.Id + i + 1;
                if (cardCounts.TryGetValue(nextId, out var nextCardCount))
                {
                    cardCounts[nextId] = nextCardCount + cardCount;
                }
            }
        }

        return count;
    }
    
    private static int CountMatches(Card card) => card.Chosen.Count(n => card.Winners.Contains(n));

    public record Model(IReadOnlyList<Card> Cards);
    public record Card(int Id, IReadOnlyList<int> Winners, IReadOnlyList<int> Chosen);
}