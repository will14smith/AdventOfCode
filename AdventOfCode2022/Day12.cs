namespace AdventOfCode2022;

[Day]
public partial class Day12 : Day<Day12.Model, int, int>
{
    private const string Sample = "Sabqponm\nabcryxxl\naccszExk\nacctuvwj\nabdefghi";
    
    protected override Model Parse(string input) => Model.Build(GridParser.ParseChar(input, x => x - 'a'));

    [Sample(Sample, 31)]
    protected override int Part1(Model input)
    {
        // A*
        var start = input.Start;
        var end = input.End;

        var openSet = new PriorityQueue<Position, int>();
        openSet.Enqueue(start, H(start));
        
        var cameFrom = new Dictionary<Position, Position>();
        var gScore = new Dictionary<Position, int>
        {
            [start] = 0
        };

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();
            if (current == end)
            {
                return CountPath(cameFrom, end, start);
            }
            
            foreach (var neighbour in current.OrthogonalNeighbours())
            {
                if (!input.Map.IsValid(neighbour))
                {
                    continue;
                }
                if (input.Map[neighbour] - input.Map[current] > 1)
                {
                    continue;
                }

                var newGScore = gScore[current] + 1;
                var previousGScore = gScore.TryGetValue(neighbour, out var x) ? x : int.MaxValue;

                if (newGScore < previousGScore)
                {
                    cameFrom[neighbour] = current;
                    gScore[neighbour] = newGScore;
                    openSet.Enqueue(neighbour, newGScore + H(neighbour));
                }
            }
        }
        
        throw new Exception("no solution");
        
        int H(Position p) => (end - p).TaxiDistance();  
    }
    
    [Sample(Sample, 29)]
    protected override int Part2(Model input)
    {
        return OptimisedSearch.Solve((Current: input.End, Distance: 0), 0, IsGoal, Next, x => false, x => x.Distance).Distance;
        
        bool IsGoal((Position Current, int Distance) x) => input.Map[x.Current] == 0;
        IEnumerable<(Position Current, int Distance)> Next((Position Current, int Distance) x) => 
            x.Current.OrthogonalNeighbours().Where(n => input.Map.IsValid(n) && input.Map[x.Current] - input.Map[n] <= 1).Select(n => (n, x.Distance + 1));
    }

    private static int CountPath(IReadOnlyDictionary<Position, Position> cameFrom, Position end, Position start)
    {
        var position = end;
        var count = 0;
        
        while (position != start)
        {
            count++;
            position = cameFrom[position];
        }

        return count;
    }
    
    public record Model(Grid<int> Map, Position Start, Position End)
    {
        public static Model Build(Grid<int> map)
        {
            var start = map.Keys().First(x => map[x] == 'S' - 'a');
            var end = map.Keys().First(x => map[x] == 'E' - 'a');

            map[start] = 'a' - 'a';
            map[end] = 'z' - 'a';
            
            return new Model(map, start, end);
        }
    }
}
