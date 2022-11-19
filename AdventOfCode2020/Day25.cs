namespace AdventOfCode2020;

[Day]
public partial class Day25 : Day<Day25.Model, long, long>
{
    private const string Sample = "5764801\n17807724\n";
    
    protected override Model Parse(string input)
    {
        var parts = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        return new Model(long.Parse(parts[0]), long.Parse(parts[1]));
    }

    [Sample(Sample, 14897079L)]
    protected override long Part1(Model input)
    {
        var (cardPublicKey, doorPublicKey) = input;

        var cardLoopSize = 0;
        var cardValue = 1;
        while (true)
        {
            if (cardPublicKey == cardValue) { break; }
            cardValue = cardValue * 7 % 20201227;
            cardLoopSize++;
        }

        var doorLoopSize = 0;
        var doorValue = 1;
        while (true)
        {
            if (doorPublicKey == doorValue) { break; }
            doorValue = doorValue * 7 % 20201227;
            doorLoopSize++;
        }

        var key1 = Transform(doorLoopSize, cardPublicKey);
        var key2 = Transform(cardLoopSize, doorPublicKey);

        if (key1 != key2)
        {
            throw new InvalidOperationException("keys don't match");
        }

        return key1;
    }

    protected override long Part2(Model input) => 0;
    
    private static long Transform(long loopSize, long subjectNumber)
    {
        var value = 1L;

        for (var i = 0; i < loopSize; i++)
        {
            value = value * subjectNumber % 20201227;
        }

        return value;
    }

    public record Model(long CardPublicKey, long DoorPublicKey);
}
