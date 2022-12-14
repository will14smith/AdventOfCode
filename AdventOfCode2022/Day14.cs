using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2022;

[Day]
public partial class Day14 : ParseDay<Day14.Input, int, int>
{
    private const string Sample = "498,4 -> 498,6 -> 496,6\n503,4 -> 502,4 -> 502,9 -> 494,9";

    public static readonly TextParser<Position> PositionParser = Numerics.IntegerInt32.ThenIgnore(Character.EqualTo(',')).Then(Numerics.IntegerInt32).Select(x => new Position(x.Item1, x.Item2));
    public static readonly TextParser<Line> LineParser = PositionParser.ManyDelimitedBy(Span.EqualTo(" -> ")).Select(x => new Line(x));
    public static readonly TextParser<Input> InputParser = LineParser.ManyDelimitedBy(Character.EqualTo('\n')).Select(x => new Input(x));

    protected override TextParser<Input> Parser => InputParser;

    [Sample(Sample, 24)]
    protected override int Part1(Input input)
    {
        var (normalisedInput, sandSource, size) = NormalizeInput(input, new Position(500, 0)); 
        var grid = InputToGrid(normalisedInput, size);

        var restCount = 0;
        while (true)
        {
            var (nextGrid, result) = DropSand(grid, sandSource);

            if (result == DropSandResult.SacrificedToTheInfiniteVoid)
            {
                return restCount;
            }

            restCount++;
            grid = nextGrid;
        }
    }
    
    [Sample(Sample, 93)]
    protected override int Part2(Input input)
    {
        var (normalisedInput, sandSource, size) = NormalizeInput(input, new Position(500, 0));
        normalisedInput = new Input(normalisedInput.Lines.Append(new Line(new[]
        {
            new Position(-500, size.Y + 1), 
            new Position(500, size.Y + 1), 
        })).ToList());
        (normalisedInput, sandSource, size) = NormalizeInput(normalisedInput, sandSource);

        var grid = InputToGrid(normalisedInput, size);

        var restCount = 0;
        while (true)
        {
            var (nextGrid, result) = DropSand(grid, sandSource);

            if (result == DropSandResult.SacrificedToTheInfiniteVoid)
            {
                throw new Exception("no solution");
            }
            
            if (result == DropSandResult.ReachedTheSourceWhichIsGoodAndIDontDie)
            {
                return ++restCount;
            }

            restCount++;
            grid = nextGrid;
        }
    }

    private (Input NormalisedInput, Position SandSource, Position Size) NormalizeInput(Input input, Position sandSource)
    {
        var minX = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;

        foreach (var line in input.Lines)
        {
            foreach (var point in line.Points)
            {
                if (point.X < minX) minX = point.X;
                if (point.X > maxX) maxX = point.X;
                if (point.Y > maxY) maxY = point.Y;
            }
        }

        var delta = new Position(minX, 0);
        var size = new Position(maxX - minX + 1, maxY + 1);
        
        var newLines = new List<Line>();

        foreach (var line in input.Lines)
        {
            var newPoints = new List<Position>();
            foreach (var point in line.Points)
            {
                newPoints.Add(point - delta);
            }
            newLines.Add(new Line(newPoints));
        }
        
        var newInput = new Input(newLines);
        return (newInput, sandSource - delta, size);
    }

    private static Grid<State> InputToGrid(Input input, Position size)
    {
        var grid = Grid<State>.Empty(size.X, size.Y);
        
        foreach (var line in input.Lines)
        {
            var points = line.Points;
            
            for (var i = 1; i < points.Count; i++)
            {
                var point = points[i - 1];
                var nextPoint = points[i];

                if (point.X == nextPoint.X)
                {
                    var x = point.X;
                    var (startY, endY) = point.Y < nextPoint.Y ? (point.Y, nextPoint.Y) : (nextPoint.Y, point.Y);
                    
                    for (var y = startY; y <= endY; y++)
                    {
                        grid[x, y] = State.Rock;
                    }
                }
                else if(point.Y == nextPoint.Y)
                {
                    var y = point.Y;
                    var (startX, endX) = point.X < nextPoint.X ? (point.X, nextPoint.X) : (nextPoint.X, point.X);
                    
                    for (var x = startX; x <= endX; x++)
                    {
                        grid[x, y] = State.Rock;
                    }
                }
                else
                {
                    throw new Exception("diagonals! are you crazy?");
                }
            }
        }

        return grid;
    }

    private (Grid<State>, DropSandResult) DropSand(Grid<State> grid, Position source)
    {
        var sand = source;

        var down = new Position(0, 1);
        var downLeft = new Position(-1, 1);
        var downRight = new Position(1, 1);

        while (true)
        {
            var sandDown = sand + down;
            if (!grid.IsValid(sandDown))
            {
                return (grid, DropSandResult.SacrificedToTheInfiniteVoid);
            }

            if (grid[sandDown] == State.Air)
            {
                sand = sandDown;
                continue;
            }

            var sandDownLeft = sand + downLeft;
            if (!grid.IsValid(sandDownLeft))
            {
                return (grid, DropSandResult.SacrificedToTheInfiniteVoid);
            }

            if (grid[sandDownLeft] == State.Air)
            {
                sand = sandDownLeft;
                continue;
            }

            var sandDownRight = sand + downRight;
            if (!grid.IsValid(sandDownRight))
            {
                return (grid, DropSandResult.SacrificedToTheInfiniteVoid);
            }

            if (grid[sandDownRight] == State.Air)
            {
                sand = sandDownRight;
                continue;
            }

            if (sand == source)
            {
                return (grid, DropSandResult.ReachedTheSourceWhichIsGoodAndIDontDie);
            }

            grid[sand] = State.Sand;
            break;
        }

        return (grid, DropSandResult.Rest);
    }
    
    private enum State
    {
        Air = 0,
        Rock,
        Sand,
    }

    private enum DropSandResult
    {
        Rest,
        SacrificedToTheInfiniteVoid,
        ReachedTheSourceWhichIsGoodAndIDontDie,
    }
    
    public record Input(IReadOnlyList<Line> Lines);
    public record Line(IReadOnlyList<Position> Points);
}
