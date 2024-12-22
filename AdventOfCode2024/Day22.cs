using System.Collections.Concurrent;

namespace AdventOfCode2024;

[Day]
public partial class Day22 : Day<Day22.Model, long, long>
{
    public record Model(IReadOnlyList<long> Buyers);
    
    protected override Model Parse(string input) => new(input.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray());
    
    [Sample("1\n10\n100\n2024\n", 37327623L)]
    protected override long Part1(Model input)
    {
        return input.Buyers.Sum(x => NextN(x, 2000));
    }

    private long NextN(long secret, int n)
    {
        for (var i = 0; i < n; i++)
        {
            secret = (long)Next((ulong)secret);
        }

        return secret;
    }
    
    [Sample("1\n2\n3\n2024\n", 23L)]
    protected override long Part2(Model input)
    {
        var ngrams = new ConcurrentDictionary<uint, ulong>();

        Parallel.ForEach(input.Buyers, buyer =>
        {
            var seenForBuyer = new HashSet<uint>();

            var a = (ulong)buyer;
            var b = Next(a);
            var c = Next(b);
            var d = Next(c);



            for (var i = 0; i < 2000 - 2; i++)
            {
                var e = Next(d);

                var price = e % 10;

                var diff1 = b % 10 - a % 10;
                var diff2 = c % 10 - b % 10;
                var diff3 = d % 10 - c % 10;
                var diff4 = e % 10 - d % 10;

                a = b;
                b = c;
                c = d;
                d = e;

                var ngram = (uint)(((diff1 + 10) << 24) | ((diff2 + 10) << 16) | ((diff3 + 10) << 8) |
                                   ((diff4 + 10) << 0));
                if (!seenForBuyer.Add(ngram))
                {
                    continue;
                }

                ngrams.AddOrUpdate(ngram, static (_, a) => a, static (_, a, b) => a + b, price);
            }
        });
        
        return (long) ngrams.Max(x => x.Value);
    }
    
    private static ulong Next(ulong secret)
    {
        var result = secret << 6;
        secret ^= result;
        secret &= 0xFFFFFF;

        result = secret >> 5;
        secret ^= result;
        secret &= 0xFFFFFF;

        result = secret << 11;
        secret ^= result;
        secret &= 0xFFFFFF;

        return secret;
    }
}