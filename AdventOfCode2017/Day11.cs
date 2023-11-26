namespace AdventOfCode2017;

[Day]
public partial class Day11 : Day<Day11.Model, int, int>
{
    // using flat axial coordinates

    protected override Model Parse(string input) => new (input.Split(',').Select(ParseDirection).ToList());
    private static Direction ParseDirection(string input) =>
        input switch
        {
            "n" => Direction.North,
            "ne" => Direction.NorthEast,
            "se" => Direction.SouthEast,
            "s" => Direction.South,
            "sw" => Direction.SouthWest,
            "nw" => Direction.NorthWest,
        };

    [Sample("ne,ne,ne", 3)]
    [Sample("ne,ne,sw,sw", 0)]
    [Sample("ne,ne,s,s", 2)]
    [Sample("se,sw,se,sw,sw", 3)]
    protected override int Part1(Model input) => GetDistance(input.Directions.Aggregate(new Position(0, 0), (current, direction) => current + GetVector(direction)));

    protected override int Part2(Model input)
    {
        var position = new Position(0, 0);
        var maxDistance = 0;
        
        foreach (var direction in input.Directions)
        {
            position += GetVector(direction);
            maxDistance = Math.Max(maxDistance, GetDistance(position));
        }
        
        return maxDistance;
    }

    private static int GetDistance(Position position)
    {
        return (Math.Abs(position.X) + Math.Abs(position.X + position.Y) + Math.Abs(position.Y)) / 2;
    }

    private static Position GetVector(Direction direction) =>
        direction switch
        {
            Direction.North => new Position(0, -1),
            Direction.NorthEast => new Position(1, -1),
            Direction.SouthEast => new Position(1, 0),
            Direction.South => new Position(0, 1),
            Direction.SouthWest => new Position(-1, 1),
            Direction.NorthWest => new Position(-1, 0),
            _ => throw new ArgumentOutOfRangeException()
        };

    public record Model(IReadOnlyList<Direction> Directions);

    public enum Direction
    {
        North,
        NorthEast,
        SouthEast,
        South,
        SouthWest,
        NorthWest,
    }
}
