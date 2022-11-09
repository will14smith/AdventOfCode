using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2015;

[Day]
public partial class Day03 : ParseDay<Day03.Direction[], int, int>
{
    protected override IEnumerable<(string, int)> Tests1 { get; } = new[]
    {
        (">", 2),
        ("^>v<", 4),
        ("^v^v^v^v^v", 2),
    };
    protected override IEnumerable<(string, int)> Tests2 { get; } = new[]
    {
        (">v", 3),
        ("^>v<", 3),
        ("^v^v^v^v^v", 11),
    };
    
    protected override int Part1(Direction[] input)
    {
        var location = (X: 0, Y: 0);
        var visited = new HashSet<(int, int)> { location };

        foreach (var direction in input)
        {
            location = direction switch
            {
                Direction.North => (location.X, location.Y + 1),
                Direction.East => (location.X + 1, location.Y),
                Direction.South => (location.X, location.Y - 1),
                Direction.West => (location.X - 1, location.Y),
            };
            
            visited.Add(location);
        }

        return visited.Count;
    }

    protected override int Part2(Direction[] input)
    {
        var location1 = (X: 0, Y: 0);
        var location2 = (X: 0, Y: 0);
        
        var visited = new HashSet<(int, int)> { location1, location2 };

        foreach (var direction in input)
        {
            location1 = direction switch
            {
                Direction.North => (location1.X, location1.Y + 1),
                Direction.East => (location1.X + 1, location1.Y),
                Direction.South => (location1.X, location1.Y - 1),
                Direction.West => (location1.X - 1, location1.Y),
            };
            
            visited.Add(location1);
            (location1, location2) = (location2, location1);
        }

        return visited.Count;    
    }
    
    protected override TextParser<Direction[]> Parser { get; } = Character.In('^', '>', 'v', '<').Select(c => c switch
    {
        '^' => Direction.North,
        '>' => Direction.East,
        'v' => Direction.South,
        '<' => Direction.West,
    }).Many();
    
    public enum Direction
    {
        North,
        East,
        South,
        West
    }
}