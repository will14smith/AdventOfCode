namespace AdventOfCode2022;

[Day]
public partial class Day22 : ParseDay<Day22.Model, Day22.TokenType, int, int>
{
    private const string Sample = "        ...#\n        .#..\n        #...\n        ....\n...#.......#\n........#...\n..#....#....\n..........#.\n        ...#....\n        .....#..\n        .#......\n        ......#.\n\n10R5L5R10L4R5L5";

    private const int Face0 = 0;
    private const int Face1 = 1;
    private const int Face2 = 2;
    private const int Face3 = 3;
    private const int Face4 = 4;
    private const int Face5 = 5;

    private const int Up = 3;
    private const int Down = 1;
    private const int Left = 2;
    private const int Right = 0;
    
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

        var state = new State1(FindInitialPosition(map), Right);
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

    // [Sample(Sample, 5031)]
    protected override int Part2(Model input)
    {
        var facePositions = new[]
        {
            new Position(50, 0),
            new Position(100, 0),
            new Position(50, 50),
            new Position(0, 100),
            new Position(50, 100),
            new Position(0, 150),
        };

        var faces = facePositions.Select(x => ExtractFace(input, x)).ToArray();

        var state = new State2(new Position(0, 0), Face0, Right);
        
        foreach (var instruction in input.Instructions)
        {
            switch (instruction)
            {
                case Instruction.Forward forward:
                    var delta = Headings[state.Heading];
                    var face = faces[state.Face];
                    for (int i = 0; i < forward.Distance; i++)
                    {
                        var nextPosition = state.FacePosition + delta;
                        if (!face.IsValid(nextPosition))
                        {
                            var nextState = WrappedMoveCube(state);
                            var nextFace = faces[nextState.Face];
                            if (nextFace[nextState.FacePosition] == Cell.Wall)
                            {
                                break;
                            }

                            state = nextState;
                            delta = Headings[state.Heading];
                            face = nextFace;
                        }
                        else if(face[nextPosition] == Cell.Wall)
                        {
                            break;
                        }
                        else
                        {
                            state = state with { FacePosition = nextPosition };
                        }
                    }
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

        var position = state.FacePosition + facePositions[state.Face];
        return (position.Y + 1) * 1000 + (position.X + 1) * 4 + state.Heading;
    }

    private static State2 WrappedMoveCube(State2 state)
    {
        // Cube Layout
        //  01
        //  2
        // 34
        // 5
        
        // Headings
        //  3
        // 2 0
        //  1

        var x = state.FacePosition.X;
        var y = state.FacePosition.Y;

        return (state.Face, state.Heading) switch
        {
            (Face0, Right) => new State2(new Position(0, y), Face1, Right),
            (Face0, Down) => new State2(new Position(x, 0), Face2, Down),
            (Face0, Left) => new State2(new Position(0, 49 - y), Face3, Right),
            (Face0, Up) => new State2(new Position(0, x), Face5, Right),
            
            (Face1, Right) => new State2(new Position(49, 49 - y), Face4, Left),
            (Face1, Down) => new State2(new Position(49, x), Face2, Left),
            (Face1, Left) => new State2(new Position(49, y), Face0, Left),
            (Face1, Up) => new State2(new Position(x, 49), Face5, Up),
            
            (Face2, Right) => new State2(new Position(y, 49), Face1, Up),
            (Face2, Down) => new State2(new Position(x, 0), Face4, Down),
            (Face2, Left) => new State2(new Position(y, 0), Face3, Down),
            (Face2, Up) => new State2(new Position(x, 49), Face0, Up),
            
            (Face3, Right) => new State2(new Position(0, y), Face4, Right),
            (Face3, Down) => new State2(new Position(x, 0), Face5, Down),
            (Face3, Left) => new State2(new Position(0, 49 - y), Face0, Right),
            (Face3, Up) => new State2(new Position(0, x), Face2, Right),
            
            (Face4, Right) => new State2(new Position(49, 49 - y), Face1, Left),
            (Face4, Down) => new State2(new Position(49, x), Face5, Left),
            (Face4, Left) => new State2(new Position(49, y), Face3, Left),
            (Face4, Up) => new State2(new Position(x, 49), Face2, Up),
            
            (Face5, Right) => new State2(new Position(y, 49), Face4, Up),
            (Face5, Down) => new State2(new Position(x, 0), Face1, Down),
            (Face5, Left) => new State2(new Position(y, 0), Face0, Down),
            (Face5, Up) => new State2(new Position(x, 49), Face3, Up),
        };
    }

    private static Grid<Cell> ExtractFace(Model input, Position topLeft)
    {
        var face = Grid.Empty<Cell>(50, 50);
        
        for (var y = 0; y < face.Height; y++)
        for (var x = 0; x < face.Width; x++)
        {
            face[x, y] = input.Map[x + topLeft.X, y + topLeft.Y];
        }
        
        return face;
    }


    public record State1(Position Position, int Heading);
    public record State2(Position FacePosition, int Face, int Heading);
    
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
