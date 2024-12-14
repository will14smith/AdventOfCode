using System.Drawing;

namespace AdventOfCode2024;

[Day]
public partial class Day14 : LineDay<Day14.Model, int, int>
{
    public record Model(Position Position, Position Velocity);
    protected override Model ParseLine(string input)
    {
        var parts =input.Split(" ");
        return new Model(ParsePart(parts[0]), ParsePart(parts[1]));
        
        static Position ParsePart(string input)
        {
            var parts = input[2..].Split(',').Select(int.Parse).ToArray();
            return new Position(parts[0], parts[1]);
        }
    }
    
    // [Sample("p=0,4 v=3,-3\np=6,3 v=-1,-3\np=10,3 v=-1,2\np=2,0 v=2,-1\np=0,0 v=1,3\np=3,0 v=-2,-2\np=7,6 v=-1,-3\np=3,0 v=-1,-2\np=9,3 v=2,3\np=7,3 v=-1,2\np=2,4 v=2,-3\np=9,5 v=-3,-3", 12)]
    protected override int Part1(IEnumerable<Model> input)
    {
        var size = new Position(101, 103);
        // var size = new Position(11, 7);
        var ticks = 100;
        
        var finalPositions = input.Select(model => Wrap(model.Position + model.Velocity * ticks, size)).ToArray();
        var quadrantCounts = finalPositions.CountBy(position => Quadrant(position, size)).Where(x => x.Key != -1);

        return quadrantCounts.Aggregate(1, (a, b) => a * b.Value);
    }
    
    private static Position Wrap(Position position, Position size)
    {
        var x = position.X % size.X;
        if (x < 0) x += size.X;
        
        var y = position.Y % size.Y;
        if (y < 0) y += size.Y;
        
        return new Position(x, y);
    }
    
    private int Quadrant(Position position, Position size)
    {
        if (size.X % 2 == 1 && position.X == size.X / 2) return -1;
        if (size.Y % 2 == 1 && position.Y == size.Y / 2) return -1;

        var isLeft = position.X < size.X / 2;
        var isTop = position.Y < size.Y / 2;
        
        return isLeft ? (isTop ? 0 : 2) : (isTop ? 1 : 3);
    }

    protected override int Part2(IEnumerable<Model> input)
    {
        var size = new Position(101, 103);
        var inputs = input.ToList();

        return Enumerable.Range(0, size.X * size.Y).AsParallel().Select(ticks =>
        {
            var finalPositions = inputs.Select(model => Wrap(model.Position + model.Velocity * ticks, size)).ToHashSet();

            var longest = 0;
            // look for a long vertical line for the trunk
            foreach (var position in finalPositions)
            {
                // only test the top elements
                if (finalPositions.Contains(position with { Y = position.Y - 1 }))
                {
                    continue;
                }

                var currentLength = 0;
                var currentPosition = position;
                while (true)
                {
                    currentLength++;
                    currentPosition = currentPosition with { Y = currentPosition.Y + 1 };
                    if (!finalPositions.Contains(currentPosition))
                    {
                        break;
                    }
                }

                if (currentLength > longest)
                {
                    longest = currentLength;
                }
            }
            
            return (Tick: ticks, Length: longest);
        }).MaxBy(x => x.Length).Tick;
    }
}