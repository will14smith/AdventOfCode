namespace AdventOfCode2024;

[Day]
public partial class Day18 : LineDay<Day18.Model, int, string>
{
    public record Model(Position Position);
    protected override Model ParseLine(string input) => new (Position.Parse(input));

    protected override int Part1(IEnumerable<Model> input)
    {
        return Solve(input, 1024);
    }
    
    protected override string Part2(IEnumerable<Model> input)
    {
        var inputs = input.ToArray();

        for (var i = 1024; i < inputs.Length; i++)
        {
            try
            {
                Solve(inputs, i);
            }
            catch
            {
                var failing = inputs[i - 1].Position;
                return $"{failing.X},{failing.Y}";
            }
        }
        
        throw new NotImplementedException();
    }
    
    private static int Solve(IEnumerable<Model> input, int count)
    {
        var start = new Position(0, 0);
        var end = new Position(70, 70);
        
        var map = Grid.Empty<bool>(end.X + 1, end.Y + 1);
        foreach (var cell in input.Take(count))
        {
            map[cell.Position] = true;
        }

        var openSet = new PriorityQueue<Position, int>();
        openSet.Enqueue(start, H(start));
        
        var cameFrom = new Dictionary<Position, Position>();
        var gScore = new Dictionary<Position, int>
        {
            [start] = 0
        };

        while (openSet.Count > 0)
        {
            var currentState = openSet.Dequeue();
            if (currentState == end)
            {
                return gScore[currentState];
            }
            
            foreach (var next in currentState.OrthogonalNeighbours())
            {
                if (!map.IsValid(next))
                {
                    continue;
                }

                if (map[next])
                {
                    continue;
                }
                
                var newGScore = gScore[currentState] + 1;
                var previousGScore = gScore.GetValueOrDefault(next, int.MaxValue);

                if (newGScore < previousGScore)
                {
                    cameFrom[next] = currentState;
                    gScore[next] = newGScore;
                    openSet.Enqueue(next, newGScore + H(next));
                }

            }
        }
        
        throw new Exception("no solution");
        
        int H(Position p) => (end - p).TaxiDistance();
    }
}