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
    protected override long Part1(Model input) => input.Races.Aggregate(1L, (current, race) => current * CalculateWins(race));
    
    [Sample("Time:      7  15   30\nDistance:  9  40  200", 71503L)]
    protected override long Part2(Model input) =>
        CalculateWins(new Race(
            long.Parse(string.Join("", input.Races.Select(x => x.TimeAllow))),
            long.Parse(string.Join("", input.Races.Select(x => x.DistanceRecord)))
        ));
    
    private static long CalculateWins(Race race)
    {
        var wins = 0;

        // distance = (race.TimeAllow - chargeTime) * chargeTime
        // distance = -(chargeTime^2) + (race.TimeAllow * chargeTime)
        // find distance > race.DistanceRecord
        // -(chargeTime^2) + (race.TimeAllow * chargeTime) - race.DistanceRecord > 0
        
        // ax^2 + bx + c = 0
        // roots = (-b +- sqrt(b^2 - 4ac)) / 2a

        // a = -1
        // b = race.TimeAllow
        // c = -race.DistanceRecord
        
        var sqrt = Math.Sqrt(race.TimeAllow * race.TimeAllow - 4 * race.DistanceRecord);
        var rootA = (-race.TimeAllow + sqrt) / -2;
        var rootB = (-race.TimeAllow - sqrt) / -2;

        // if the roots are exact values then they don't **beat** the record
        if (rootA - Math.Floor(rootA) == 0) rootA++;        
        if (rootB - Math.Ceiling(rootB) == 0) rootB--;        
        
        return (long)Math.Floor(rootB) - (long)Math.Ceiling(rootA) + 1;
    }

    public record Model(IReadOnlyList<Race> Races);
    public record Race(long TimeAllow, long DistanceRecord);
}