namespace AdventOfCode2024;

[Day]
public partial class Day15 : Day<Day15.Model, int, int>
{
    public record Model(Grid<Cell> Map, IReadOnlyList<Direction> Directions);
    public enum Cell
    {
        Empty,
        Wall,
        Box,
        Robot,
    }
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }

    protected override Model Parse(string input)
    {
        var parts = input.Split("\n\n");
        
        var map = GridParser.ParseChar(parts[0], x => x switch
        {
            '.' => Cell.Empty,
            '#' => Cell.Wall,
            'O' => Cell.Box,
            '@' => Cell.Robot,
        });
        
        var directions = parts[1].Where(x => x != '\n').Select(x => x switch
        {
            '^' => Direction.Up,
            'v' => Direction.Down,
            '<' => Direction.Left,
            '>' => Direction.Right,
        }).ToArray();
        
        return new Model(map, directions);
    }

    [Sample("########\n#..O.O.#\n##@.O..#\n#...O..#\n#.#.O..#\n#...O..#\n#......#\n########\n\n<^^>>>vv<v>>v<<", 2028)]
    [Sample("##########\n#..O..O.O#\n#......O.#\n#.OO..O.O#\n#..O@..O.#\n#O#..O...#\n#O..O..O.#\n#.OO.O.OO#\n#....O...#\n##########\n\n<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^\nvvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v\n><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<\n<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^\n^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><\n^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^\n>^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^\n<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>\n^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>\nv^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^", 10092)]
    protected override int Part1(Model input)
    {
        var map = input.Map;
        var directions = input.Directions;

        var robot = map.Keys().First(p => map[p] == Cell.Robot);
        foreach (var direction in directions)
        {
            var delta = direction switch
            {
                Direction.Up => new Position(0, -1),
                Direction.Down => new Position(0, 1),
                Direction.Left => new Position(-1, 0),
                Direction.Right => new Position(1, 0),
            };
            var next = robot + delta;

            switch (map[next])
            {
                case Cell.Empty:
                    map[robot] = Cell.Empty;
                    map[next] = Cell.Robot;
                    robot = next;
                    break;
                
                case Cell.Wall: 
                    break;
                
                case Cell.Box:
                    var endOfChain = next;
                    while (true)
                    {
                        endOfChain += delta;
                        var endOfChainCell = map[endOfChain];
                        if (endOfChainCell == Cell.Empty)
                        {
                            map[robot] = Cell.Empty;
                            map[next] = Cell.Robot;
                            map[endOfChain] = Cell.Box;
                            robot = next;
                            break;
                        } 
                        if (endOfChainCell == Cell.Wall)
                        {
                            break;
                        } 
                        
                        if (endOfChainCell == Cell.Box)
                        {
                            continue;
                        }

                        throw new InvalidOperationException();
                    }
                    break;
                
                default: throw new InvalidOperationException();
            }
        }

        return SumCoordinates(map);
    }
    
    [Sample("##########\n#..O..O.O#\n#......O.#\n#.OO..O.O#\n#..O@..O.#\n#O#..O...#\n#O..O..O.#\n#.OO.O.OO#\n#....O...#\n##########\n\n<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^\nvvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v\n><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<\n<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^\n^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><\n^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^\n>^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^\n<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>\n^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>\nv^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^", 9021)]
    protected override int Part2(Model input)
    {
        var map = ExpandMap(input.Map);
        var directions = input.Directions;

        var robot = map.Keys().First(p => map[p] == Cell2.Robot);
        foreach (var direction in directions)
        {
            var delta = direction switch
            {
                Direction.Up => new Position(0, -1),
                Direction.Down => new Position(0, 1),
                Direction.Left => new Position(-1, 0),
                Direction.Right => new Position(1, 0),
            };
            var next = robot + delta;

            switch (map[next])
            {
                case Cell2.Empty:
                    map[robot] = Cell2.Empty;
                    map[next] = Cell2.Robot;
                    robot = next;
                    break;
                
                case Cell2.Wall: 
                    break;
                
                case Cell2.BoxLeft when direction == Direction.Right:
                case Cell2.BoxRight when direction == Direction.Left:
                    var endOfChain = next;
                    while (true)
                    {
                        endOfChain += delta;
                        
                        var endOfChainCell = map[endOfChain];
                        if (endOfChainCell == Cell2.Empty)
                        {
                            map[robot] = Cell2.Empty;
                            map[next] = Cell2.Robot;
                            robot = next;

                            next += delta;
                            while (next != endOfChain + delta)
                            {
                                if (direction == Direction.Right)
                                {
                                    map[next] = map[next] == Cell2.BoxRight ? Cell2.BoxLeft : Cell2.BoxRight;
                                }
                                else
                                {
                                    map[next] = map[next] == Cell2.BoxLeft ? Cell2.BoxRight : Cell2.BoxLeft;
                                }

                                next += delta;
                            }
                            break;
                        } 
                        if (endOfChainCell == Cell2.Wall)
                        {
                            break;
                        } 
                        
                        if (endOfChainCell is Cell2.BoxLeft or Cell2.BoxRight)
                        {
                            continue;
                        }

                        throw new InvalidOperationException();
                    }
                    break;
                
                case Cell2.BoxLeft when direction is Direction.Up or Direction.Down:
                case Cell2.BoxRight when direction is Direction.Up or Direction.Down:
                    var move = PushUpDown(map, robot, delta)?.Distinct().ToDictionary(x => x, x => map[x]);
                    if (move is null)
                    {
                        break;
                    }
                    
                    foreach (var pair in move)
                    {
                        if (!move.ContainsKey(pair.Key - delta))
                        {
                            map[pair.Key] = Cell2.Empty;
                        }
                        map[pair.Key + delta] = pair.Value;
                    }
                    robot = next;
                    break;

                default: throw new InvalidOperationException();
            }
        }

        return SumCoordinates(map);
    }

    public static IEnumerable<Position>? PushUpDown(Grid<Cell2> map, Position position, Position delta)
    {
        if (map[position] == Cell2.Empty)
        {
            return Array.Empty<Position>();
        }
        
        if (map[position] == Cell2.Robot)
        {
            var next = PushUpDown(map, position + delta, delta);
            return next?.Append(position);
        }

        if (map[position] == Cell2.Wall)
        {
            return null;
        }
        
        if (map[position] == Cell2.BoxLeft)
        {
            var nextDelta = PushUpDown(map, position + delta, delta);
            if (nextDelta is null) return null;
            
            var nextRight = PushUpDown(map, position + new Position(1, 0) + delta, delta);
            if (nextRight is null) return null;
            
            return nextDelta.Concat(nextRight).Append(position).Append(position + new Position(1, 0));
        }
        
        if (map[position] == Cell2.BoxRight)
        {
            var nextDelta = PushUpDown(map, position + delta, delta);
            if (nextDelta is null) return null;
            
            var nextLeft = PushUpDown(map, position + new Position(-1, 0) + delta, delta);
            if (nextLeft is null) return null;
            
            return nextDelta.Concat(nextLeft).Append(position).Append(position + new Position(-1, 0));
        }

        throw new InvalidOperationException();
    }
    
    private static int SumCoordinates(Grid<Cell> map) => map.Keys().Where(p => map[p] == Cell.Box).Sum(p => p.X + 100 * p.Y);
    private static int SumCoordinates(Grid<Cell2> map) => map.Keys().Where(p => map[p] == Cell2.BoxLeft).Sum(p => p.X + 100 * p.Y);
    
    public enum Cell2
    {
        Empty,
        Wall,
        BoxLeft,
        BoxRight,
        Robot,
    }
    private Grid<Cell2> ExpandMap(Grid<Cell> map)
    {
        var newMap = Grid.Empty<Cell2>(map.Width * 2, map.Height);
        
        foreach (var position in map.Keys())
        {
            switch (map[position])
            {
                case Cell.Empty:
                    newMap[position.X*2, position.Y] = Cell2.Empty;
                    newMap[position.X*2+1, position.Y] = Cell2.Empty;
                    break;
                case Cell.Wall:
                    newMap[position.X*2, position.Y] = Cell2.Wall;
                    newMap[position.X*2+1, position.Y] = Cell2.Wall;
                    break;
                case Cell.Box:
                    newMap[position.X*2, position.Y] = Cell2.BoxLeft;
                    newMap[position.X*2+1, position.Y] = Cell2.BoxRight;
                    break;
                case Cell.Robot:
                    newMap[position.X*2, position.Y] = Cell2.Robot;
                    newMap[position.X*2+1, position.Y] = Cell2.Empty;
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }
        }

        return newMap;
    }
}