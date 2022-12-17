using System.Text;

namespace AdventOfCode2022;

[Day]
public partial class Day17 : Day<Day17.Model, int, int>
{
    private const string Sample = @">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";

    private static readonly IReadOnlyList<RockShape> RockShapes = new[] { "####", ".#.\n###\n.#.", "..#\n..#\n###", "#\n#\n#\n#", "##\n##" }
        .Select(x => new RockShape(x.Split('\n').Reverse().Select(line => line.Select(c => c == '#').ToArray()).ToArray())).ToArray();

    protected override Model Parse(string input) => new(input.Select(x => x == '<' ? Direction.Left : Direction.Right).ToArray());

    [Sample(Sample, 3068)]
    protected override int Part1(Model input)
    {
        const int rockCount = 2022;
        
        var map = Map.Initial;
        var windIndex = 0;

        for (var rockIndex = 0; rockIndex < rockCount; rockIndex++)
        {
            // drop rock
            var rockShape = RockShapes[rockIndex % RockShapes.Count];
            var rock = MaterialiseRock(map, rockShape);

            // Print(map, rock);

            while (true)
            {
                var direction = input.Wind[windIndex++ % input.Wind.Count];
                var pushedRock = ApplyForce(rock, new Position(direction == Direction.Left ? -1 : 1, 0));
                if (!map.Intersects(pushedRock))
                {
                    rock = pushedRock;
                }
                
                var droppedRock = ApplyForce(rock, new Position(0, -1));

                if (!map.Intersects(droppedRock))
                {
                    rock = droppedRock;
                    continue;
                }
                
                map.Add(rock);
                break;
            }
        }

        return map.HighestRock;
    }

    private void Print(Map map, Position[] rock)
    {
        for (var y = rock.Max(x => x.Y); y >= 0; y--)
        {
            var line = new StringBuilder(9);

            line.Append('|');
            
            for (var x = 0; x < 7; x++)
            {
                if (map.Rocks.Contains(new Position(x, y)))
                {
                    line.Append('#');
                } 
                else if (rock.Contains(new Position(x, y)))
                {
                    line.Append('@');
                }
                else
                {
                    line.Append('.');
                }
            }

            line.Append('|');
            
            Output.WriteLine(line.ToString());
        }
        
        Output.WriteLine("+-------+");
        Output.WriteLine("\n\n");
    }

    [Sample(Sample, -1)]
    protected override int Part2(Model input) => throw new NotImplementedException();
    
    private Position[] MaterialiseRock(Map map, RockShape rockShape)
    {
        var baseY = map.HighestRock + 1 + 3;

        var rock = new List<Position>();

        for (var y = 0; y < rockShape.Cells.Length; y++)
        {
            var line = rockShape.Cells[y];
            for (var x = 0; x < line.Length; x++)
            {
                var cell = line[x];

                if (cell)
                {
                    rock.Add(new Position(x + 2, y + baseY));
                }
            }
        }
        
        return rock.ToArray();
    }
    
    private Position[] ApplyForce(Position[] rock, Position force)
    {
        var newRock = new Position[rock.Length];

        for (var i = 0; i < rock.Length; i++)
        {
            newRock[i] = rock[i] + force;
        }
        
        return newRock;
    }
    
    public record Model(IReadOnlyList<Direction> Wind);
    public enum Direction
    {
        Left,
        Right
    }

    public record RockShape(bool[][] Cells);

    public record Map(HashSet<Position> Rocks)
    {
        public static Map Initial => new(new HashSet<Position>());

        public int HighestRock { get; private set; }

        public bool Intersects(Position[] rock)
        {
            return rock.Any(p => p.Y <= 0 || p.X is < 0 or >= 7 || Rocks.Contains(p));
        }

        public void Add(Position[] rock)
        {
            Rocks.UnionWith(rock);
            HighestRock = Math.Max(HighestRock, rock.Max(x => x.Y));
        }
    }
}
