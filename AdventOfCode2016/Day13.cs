using System.Numerics;

namespace AdventOfCode2016;

[Day]
public partial class Day13 : Day<Day13.Model, int, int>
{
    protected override Model Parse(string input) => new Model(int.Parse(input));

    [Sample("10", 11)]
    protected override int Part1(Model input)
    {
        var target = input.Magic == 10 ? new Position(7, 4) : new Position(31, 39);
        var start = new Position(1, 1);

        return OptimisedSearch.Solve((Position: start, Steps: 0), x => x.Position == target, x => Next(input, x), _ => false, x => x.Position, x => (target - x.Position).BlockDistance()).Steps;
    }

    protected override int Part2(Model input)
    {
        var initial = new Position(1, 1);
        var seen = new Dictionary<Position, int>();

        Search(input, seen, initial, 50);

        return seen.Count;
    }

    private void Search(Model input, Dictionary<Position, int> seen, Position current, int depth)
    {
        if (depth < 0) return;

        if (seen.TryGetValue(current, out var previous) && previous >= depth)
        {
            return;
        }
        seen[current] = depth;
        
        foreach (var (next, _) in Next(input, (current, 0)))
        {
            Search(input, seen, next, depth - 1);
        }
    }

    private static IEnumerable<(Position, int)> Next(Model input, (Position, int) current)
    {
        var (currentPosition, currentSteps) = current;

        foreach (var neighbour in currentPosition.OrthogonalNeighbours())
        {
            if (neighbour.X < 0 || neighbour.Y < 0)
            {
                continue;
            }
                
            if (!input.IsWall(neighbour))
            {
                yield return (neighbour, currentSteps + 1);
            }
        }
    }


    public record Model(int Magic)
    {
        public bool IsWall(Position position)
        {
            var number = position.X * position.X + 3 * position.X + 2 * position.X * position.Y + position.Y + position.Y * position.Y;
            var bits = BitOperations.PopCount((uint)(number + Magic));
            
            return bits % 2 == 1;
        }
    }
}
