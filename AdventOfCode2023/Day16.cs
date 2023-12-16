namespace AdventOfCode2023;

using State = (Position Position, Position Heading);

[Day]
public partial class Day16 : Day<Day16.Model, int, int>
{
    private static readonly Position Up = new(0, -1);
    private static readonly Position Down = new(0, 1);
    private static readonly Position Left = new(-1, 0);
    private static readonly Position Right = new(1, 0);
    
    protected override Model Parse(string input) => new (GridParser.ParseChar(input, Parse));
    private static Cell Parse(char input) => input switch
    {
        '.' => Cell.Empty,
        '/' => Cell.MirrorForward,
        '\\' => Cell.MirrorBackward,
        '|' => Cell.SplitterVertical,
        '-' => Cell.SplitterHorizontal,
    };

    [Sample(".|...\\....\n|.-.\\.....\n.....|-...\n........|.\n..........\n.........\\\n..../.\\\\..\n.-.-/..|..\n.|....-|.\\\n..//.|....", 46)]
    protected override int Part1(Model input) => Solve(input, new Position(0, 0), Right);

    [Sample(".|...\\....\n|.-.\\.....\n.....|-...\n........|.\n..........\n.........\\\n..../.\\\\..\n.-.-/..|..\n.|....-|.\\\n..//.|....", 51)]
    protected override int Part2(Model input)
    {
        var starts = new List<State>();

        for (var x = 0; x < input.Map.Width; x++)
        {
            starts.Add((new Position(x, 0), Down));
            starts.Add((new Position(x, input.Map.Height - 1), Up));
        }
        for (var y = 0; y < input.Map.Height; y++)
        {
            starts.Add((new Position(0, y), Right));
            starts.Add((new Position(input.Map.Width - 1, y), Left));
        }
        
        return starts.AsParallel().Max(x => Solve(input, x.Position, x.Heading));
    }

    private static int Solve(Model input, Position initialPosition, Position initialHeading)
    {
        var seen = new HashSet<State>();
        var search = new Queue<State>();
        
        search.Enqueue((initialPosition, initialHeading));

        while (search.Count > 0)
        {
            var (position, heading) = search.Dequeue();
            if (!seen.Add((position, heading)))
            {
                continue;
            }

            if (!input.Map.IsValid(position))
            {
                continue;
            }
            
            switch (input.Map[position])
            {
                case Cell.Empty:
                    search.Enqueue((position + heading, heading));
                    break;
                
                case Cell.MirrorForward when heading == Up:
                    search.Enqueue((position + Right, Right));
                    break;
                case Cell.MirrorForward when heading == Down:
                    search.Enqueue((position + Left, Left));
                    break;
                case Cell.MirrorForward when heading == Left:
                    search.Enqueue((position + Down, Down));
                    break;
                case Cell.MirrorForward when heading == Right:
                    search.Enqueue((position + Up, Up));
                    break;
                
                case Cell.MirrorBackward when heading == Up:
                    search.Enqueue((position + Left, Left));
                    break;
                case Cell.MirrorBackward when heading == Down:
                    search.Enqueue((position + Right, Right));
                    break;
                case Cell.MirrorBackward when heading == Left:
                    search.Enqueue((position + Up, Up));
                    break;
                case Cell.MirrorBackward when heading == Right:
                    search.Enqueue((position + Down, Down));
                    break;    
                
                case Cell.SplitterVertical when heading.X == 0:
                    search.Enqueue((position + heading, heading));
                    break;
                case Cell.SplitterVertical:
                    search.Enqueue((position + Up, Up));
                    search.Enqueue((position + Down, Down));
                    break;

                case Cell.SplitterHorizontal when heading.Y == 0: 
                    search.Enqueue((position + heading, heading));
                    break;
                case Cell.SplitterHorizontal: 
                    search.Enqueue((position + Left, Left));
                    search.Enqueue((position + Right, Right));
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        return seen.Select(x => x.Position).Where(p => input.Map.IsValid(p)).Distinct().Count();
    }
    
    public record Model(Grid<Cell> Map);
    public enum Cell
    {
        Empty,
        MirrorForward,
        MirrorBackward,
        SplitterVertical,
        SplitterHorizontal,
    }
}