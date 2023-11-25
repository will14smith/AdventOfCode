namespace AdventOfCode2017;

[Day]
public partial class Day02 : Day<Day02.Model, int, int>
{
    protected override Model Parse(string input) => new (input.Split('\n').Select(x => new Row(x.Split('\t').Select(int.Parse).ToList())).ToList());

    [Sample("5\t1\t9\t5\n7\t5\t3\n2\t4\t6\t8", 18)]
    protected override int Part1(Model input) => input.Rows.Sum(x => x.Values.Max() - x.Values.Min());
    [Sample("5\t9\t2\t8\n9\t4\t7\t3\n3\t8\t6\t5", 9)]
    protected override int Part2(Model input) => input.Rows.Sum(Part2);

    private static int Part2(Row input)
    {
        for (var i = 0; i < input.Values.Count; i++)
        {
            for (var j = i + 1; j < input.Values.Count; j++)
            {
                var a = input.Values[i];
                var b = input.Values[j];

                if (a > b)
                {
                    (a, b) = (b, a);
                }
                
                if (b % a == 0)
                {
                    return b / a;
                }
            }
        }

        throw new Exception("bad");
    }

    public record Model(IReadOnlyList<Row> Rows);
    public record Row(IReadOnlyList<int> Values);
}
