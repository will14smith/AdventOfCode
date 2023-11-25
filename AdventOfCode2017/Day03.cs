namespace AdventOfCode2017;

[Day]
public partial class Day03 : Day<Day03.Model, int, int>
{
    protected override Model Parse(string input) => new(int.Parse(input));

    [Sample("1", 0)]
    [Sample("12", 3)]
    [Sample("23", 2)]
    [Sample("1024", 31)]
    protected override int Part1(Model input) => GenerateSpiral().Skip(input.Value - 1).First().TaxiDistance();

    protected override int Part2(Model input)
    {
        var storage = new Dictionary<Position, int>
        {
            [new Position(0, 0)] = 1
        };

        foreach (var position in GenerateSpiral().Skip(1))
        {
            var sum = position.StrictNeighbours().Sum(x => storage.TryGetValue(x, out var s) ? s : 0);
            if (sum > input.Value)
            {
                return sum;
            }
            
            storage[position] = sum;
        }

        throw new Exception("bad");
    }

    private static IEnumerable<Position> GenerateSpiral()
    {
        var visited = new HashSet<Position>();

        var current = new Position(0, 0);
        visited.Add(current);

        var vector = new Position(0, -1);

        while (true)
        {
            yield return current;
            
            // try turning left
            var leftVector = vector.RotateCCW(90);
            var leftPosition = current + leftVector;
            if (visited.Add(leftPosition))
            {
                // new position!
                vector = leftVector;
                current = leftPosition;
            }
            else
            {
                // that was visit so continue on current vector
                current += vector;
                visited.Add(current);
            }
        }
    }
    
    public record Model(int Value);
}
