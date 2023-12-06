namespace AdventOfCode2023;

[Day]
public partial class Day06 : Day<Day06.Model, long, long>
{
    protected override Model Parse(string input)
    {
        var lines = input
            .Split('\n')
            .Select(line => line
                .Split(':')[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(long.Parse))
            .ToArray();

        return new Model(lines[0].Zip(lines[1], (a, b) => new Race(a, b)).ToArray());
    }

    [Sample("Time:      7  15   30\nDistance:  9  40  200", 288L)]
    protected override long Part1(Model input) => input.Races.Aggregate(1, (current, race) => current * CalculateWins(race));
    
    [Sample("Time:      7  15   30\nDistance:  9  40  200", 71503L)]
    protected override long Part2(Model input) =>
        CalculateWins(new Race(
            long.Parse(string.Join("", input.Races.Select(x => x.TimeAllow))),
            long.Parse(string.Join("", input.Races.Select(x => x.DistanceRecord)))
        ));
    
    private static int CalculateWins(Race race)
    {
        var wins = 0;

        for (var chargeTime = 0L; chargeTime < race.TimeAllow; chargeTime++)
        {
            var distanceTravelled = (race.TimeAllow - chargeTime) * chargeTime;
            if (distanceTravelled > race.DistanceRecord)
            {
                wins++;
            }
        }

        return wins;
    }

    public record Model(IReadOnlyList<Race> Races);
    public record Race(long TimeAllow, long DistanceRecord);
}