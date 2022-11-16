namespace AdventOfCode2015;

[Day]
public partial class Day20 : Day<int, int, int>
{
    protected override int Parse(string input) => int.Parse(input);

    protected override int Part1(int input)
    {
        return IterateBruteForce.SolveMin(i => SumDivisors(i+1) * 10 >= input) + 1;
    }

    protected override int Part2(int input)
    {
        return IterateBruteForce.SolveMin(i => SumDivisors50(i+1) * 10 >= input) + 1;
    }

    protected static int SumDivisors(int n) => Divisors(n).Sum();
    protected static int SumDivisors50(int n) => Divisors(n).Where(x => n / x <= 50).Sum();
    protected static IEnumerable<int> Divisors(int n)
    {
        var divisors = new List<int>();
        
        var mid = (int)Math.Sqrt(n);
        for (var i = 1; i <= mid; i++)
        {
            if (n % i != 0)
            {
                continue;
            }
            
            divisors.Add(i);
            var pair = n / i;
            if (pair != i)
            {
                divisors.Add(pair);
            }
        }

        return divisors;
    }
}