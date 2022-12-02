namespace AdventOfCode2022;

[Day]
public partial class Day02 : LineDay<Day02.Model, int, int>
{
    private const string Sample = "A Y\nB X\nC Z";
    
    protected override Model ParseLine(string input)
    {
        return new Model(input[0], input[2]);
    }

    [Sample(Sample, 15)]
    protected override int Part1(IEnumerable<Model> input)
    {
        var decode = new Dictionary<char, char>
        {
            { 'X', 'A' },
            { 'Y', 'B' },
            { 'Z', 'C' },
        };
        
        return input.Select(x => x with { Right = decode[x.Right] }).Sum(ScoreGame);
    }

    [Sample(Sample, 12)]
    protected override int Part2(IEnumerable<Model> input)
    {
        var decode = new Dictionary<char, Result>
        {
            { 'X', Result.Lose },
            { 'Y', Result.Draw },
            { 'Z', Result.Win },
        };
        
        return input.Select(x => x with { Right = FindShapeForResult(x.Left, decode[x.Right]) }).Sum(ScoreGame);
    }
    
    private char FindShapeForResult(char l, Result r)
    {
        return (l, r) switch
        {
            ('A', Result.Lose) => 'C',
            ('A', Result.Draw) => 'A',
            ('A', Result.Win) => 'B',

            ('B', Result.Lose) => 'A',
            ('B', Result.Draw) => 'B',
            ('B', Result.Win) => 'C',

            ('C', Result.Lose) => 'B',
            ('C', Result.Draw) => 'C',
            ('C', Result.Win) => 'A',
        };
    }
    
    private static Result FindResult(char l, char r) =>
        (l, r) switch
        {
            ('A', 'A') => Result.Draw,
            ('A', 'B') => Result.Win,
            ('A', 'C') => Result.Lose,

            ('B', 'A') => Result.Lose,
            ('B', 'B') => Result.Draw,
            ('B', 'C') => Result.Win,

            ('C', 'A') => Result.Win,
            ('C', 'B') => Result.Lose,
            ('C', 'C') => Result.Draw,
        };

    private static int ScoreGame(Model game) => (game.Right - 'A' + 1) + (int)FindResult(game.Left, game.Right);

    public record Model(char Left, char Right);

    public enum Result
    {
        Lose = 0,
        Draw = 3,
        Win = 6,
    }
}
