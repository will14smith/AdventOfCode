namespace AdventOfCode.Generation;

public class Day
{
    public Day(string ns, string cls, int number)
    {
        Namespace = ns;
        Class = cls;
        Number = number;
    }

    public string Namespace { get; }
    public string Class { get; }
    public int Number { get; }
}