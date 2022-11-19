namespace AdventOfCode2020;

[Day]
public partial class Day22 : ParseDay<Day22.Model, Day22.TokenType, int, int>
{
    private const string Sample1 = "Player 1:\n9\n2\n6\n3\n1\n\nPlayer 2:\n5\n8\n4\n7\n10\n";
    
    [Sample(Sample1, 306)]
    protected override int Part1(Model input)
    {
        var player1 = new Queue<byte>(input.Player1);
        var player2 = new Queue<byte>(input.Player2);

        while (player1.Count > 0 && player2.Count > 0)
        {
            var top1 = player1.Dequeue();
            var top2 = player2.Dequeue();

            if (top1 > top2)
            {
                player1.Enqueue(top1);
                player1.Enqueue(top2);
            }
            else
            {
                player2.Enqueue(top2);
                player2.Enqueue(top1);
            }
        }

        return SumDeck(player1.Count > 0 ? player1 : player2);
    }

    private static int SumDeck(IReadOnlyCollection<byte> deck) => deck.Select((x, i) => x * (deck.Count - i)).Sum();
    
    public record Model(IReadOnlyList<byte> Player1, IReadOnlyList<byte> Player2);
}
