namespace AdventOfCode2022;

[Day]
public partial class Day22 : ParseDay<Day22.Model, Day22.TokenType, int, int>
{
    private const string Sample = "        ...#\n        .#..\n        #...\n        ....\n...#.......#\n........#...\n..#....#....\n..........#.\n        ...#....\n        .....#..\n        .#......\n        ......#.\n\n10R5L5R10L4R5L5"; 
    
    private static readonly IReadOnlyList<Position> Headings = new[]
    {
        new Position(1, 0),
        new Position(0, 1),
        new Position(-1, 0),
        new Position(0, -1),
    };

    [Sample(Sample, 6032)]
    protected override int Part1(Model input)
    {
        var map = input.Map;

        var state = new State(FindInitialPosition(map), 0);
        var visited = new HashSet<Position>();
        
        foreach (var instruction in input.Instructions)
        {
            // Output.WriteLine(input.Map.Print((grid, p) => visited.Contains(p) ? 'X' : grid[p] switch
            // {
            //     Cell.Void => ' ',
            //     Cell.Open => '.',
            //     Cell.Wall => '#',
            //     
            //     _ => throw new ArgumentOutOfRangeException()
            // }));
            // Output.WriteLine(instruction.ToString());

            switch (instruction)
            {
                case Instruction.Forward forward:
                    var position = state.Position;
                    for (var i = 0; i < forward.Distance; i++)
                    {
                        visited.Add(position);
                        var nextPosition = WrappedMove(map, position, Headings[state.Heading]);
                        
                        var nextPositionCell = map[nextPosition];
                        while (nextPositionCell == Cell.Void)
                        {
                            nextPosition = WrappedMove(map, nextPosition, Headings[state.Heading]);
                            nextPositionCell = map[nextPosition];
                        }
                        
                        switch (nextPositionCell)
                        {
                            case Cell.Open: position = nextPosition; break;
                            case Cell.Wall: i = forward.Distance; break;
                            
                            default: throw new ArgumentOutOfRangeException();
                        }
                    }
                    visited.Add(position);
                    state = state with { Position = position };
                    break;                    
                    
                case Instruction.Left:
                    state = state with { Heading = (state.Heading - 1 + Headings.Count) % Headings.Count };
                    break;

                case Instruction.Right:
                    state = state with { Heading = (state.Heading + 1) % Headings.Count };
                    break;
                
                default: throw new ArgumentOutOfRangeException(nameof(instruction));
            }
        }

        return (state.Position.Y + 1) * 1000 + (state.Position.X + 1) * 4 + state.Heading;
    }
    
    private static Position FindInitialPosition(Grid<Cell> map)
    {
        var initialPosition = Position.Identity;
        for (var x = 0; x < map.Width; x++)
        {
            if (map[x, 0] != Cell.Open)
            {
                continue;
            }

            initialPosition = new Position(x, 0);
            break;
        }

        return initialPosition;
    }

    private static Position WrappedMove(Grid<Cell> map, Position current, Position delta)
    {
        var x = (current.X + delta.X + map.Width) % map.Width;
        var y = (current.Y + delta.Y + map.Height) % map.Height;
        
        return new Position(x, y);
    }

    protected override int Part2(Model input) => throw new NotImplementedException();


    public record State(Position Position, int Heading);
    
    public record Model(Grid<Cell> Map, IReadOnlyList<Instruction> Instructions)
    {
        public static Model Create(Cell[][] cells, IReadOnlyList<Instruction> instructions)
        {
            var h = cells.Length;
            var w = cells.Max(x => x.Length);

            var map = Grid.Empty<Cell>(w, h);

            for (var y = 0; y < h; y++)
            {
                var line = cells[y];
                
                for (var x = 0; x < w; x++)
                {
                    map[x, y] = x < line.Length ? line[x] : Cell.Void;
                }
            }
            
            return new Model(map, instructions);
        }
    }
    
    public abstract record Instruction
    {
        public record Forward(int Distance) : Instruction;
        public record Left : Instruction;
        public record Right : Instruction;
    }
    
    public enum Cell
    {
        Void,
        Open,
        Wall,
    }
}
