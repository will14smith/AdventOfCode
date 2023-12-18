using Superpower.Parsers;

namespace AdventOfCode2023;

[Day]
public partial class Day18 : LineDay<Day18.Model, long, long>
{
    protected override Model ParseLine(string input)
    {
        var parts = input.Split(' ');

        var direction = parts[0][0] switch
        {
            'U' => Direction.Up,
            'D' => Direction.Down,
            'L' => Direction.Left,
            'R' => Direction.Right,
        };
        
        return new Model(direction, int.Parse(parts[1]), parts[2][2..^1]);
    }

    [Sample("R 6 (#70c710)\nD 5 (#0dc571)\nL 2 (#5713f0)\nD 2 (#d2c081)\nR 2 (#59c680)\nD 2 (#411b91)\nL 5 (#8ceee2)\nU 2 (#caa173)\nL 1 (#1b58a2)\nU 2 (#caa171)\nR 2 (#7807d2)\nU 3 (#a77fa3)\nL 2 (#015232)\nU 2 (#7a21e3)", 62L)]
    protected override long Part1(IEnumerable<Model> input) => Solve(input.ToList());

    [Sample("R 6 (#70c710)\nD 5 (#0dc571)\nL 2 (#5713f0)\nD 2 (#d2c081)\nR 2 (#59c680)\nD 2 (#411b91)\nL 5 (#8ceee2)\nU 2 (#caa173)\nL 1 (#1b58a2)\nU 2 (#caa171)\nR 2 (#7807d2)\nU 3 (#a77fa3)\nL 2 (#015232)\nU 2 (#7a21e3)", 952408144115L)]
    protected override long Part2(IEnumerable<Model> input) =>
        Solve(input.Select(x => new Model(x.Colour[^1] switch
        {
            '0' => Direction.Right,
            '1' => Direction.Down,
            '2' => Direction.Left,
            '3' => Direction.Up,
        }, HexToInt(x.Colour[..^1]), x.Colour)).ToList());

    private static long Solve(IReadOnlyList<Model> input)
    {
        var points = new List<LongPosition>();
        var position = LongPosition.Identity;

        foreach (var model in input)
        {
            position += model.Direction switch
                {
                    Direction.Up => new LongPosition(0, -1),
                    Direction.Down => new LongPosition(0, 1),
                    Direction.Left => new LongPosition(-1, 0),
                    Direction.Right => new LongPosition(1, 0),
                    _ => throw new ArgumentOutOfRangeException()
                } * model.Amount;
            points.Add(position);
        }
        
        var area = 0L;
        
        for (var i = 0; i < points.Count; i++)
        {
            var p1 = points[i];
            var p2 = points[(i + 1) % points.Count];
            
            area += p1.X * p2.Y - p2.X * p1.Y;
        }

        // area is the "internal" area and doesn't account for the border thickness 
        return Math.Abs(area) / 2L + input.Sum(x => (long) x.Amount) / 2L + 1L;
    }
    
    private static int HexToInt(string str) =>
        str.Select(c => c switch
        {
            >= '0' and <= '9' => c - '0',
            >= 'A' and <= 'F' => c - 'A' + 10,
            >= 'a' and <= 'f' => c - 'a' + 10,
            _ => throw new Exception("no")
        }).Aggregate(0, (current, digit) => current * 16 + digit);

    public record Model(Direction Direction, int Amount, string Colour);
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }
}