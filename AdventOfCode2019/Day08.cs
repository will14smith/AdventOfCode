namespace AdventOfCode2019;

[Day]
public partial class Day08 : Day<Day08.Model, int, string>
{
    private const int Width = 25;
    private const int Height = 6;

    protected override Model Parse(string input) => new(input.BatchesOfN(Width * Height).Select(layer => new Grid<int>(layer.Select(x => x - '0').ToArray(), Width)).ToList());

    protected override int Part1(Model input)
    {
        var minZeroLayer = input.Layers.MinBy(x => x.Count(y => y == 0));

        return minZeroLayer.Count(x => x == 1) * minZeroLayer.Count(x => x == 2);
    }

    protected override string Part2(Model input)
    {
        var finalGrid = Grid.Empty<bool>(Width, Height);
        
        for (var i = input.Layers.Count - 1; i >= 0; i--)
        {
            var layer = input.Layers[i];

            foreach (var position in layer.Keys())
            {
                var cell = layer[position];

                if (cell != 2) finalGrid[position] = cell == 0;
            }
        }

        return "\n" + finalGrid.Print(x => x ? ' ' : '#');
    }

    public record Model(IReadOnlyList<Grid<int>> Layers);
}
