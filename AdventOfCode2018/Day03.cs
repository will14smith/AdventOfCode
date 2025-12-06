using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2018;

[Day]
public partial class Day03 : ParseLineDay<Day03.Model, int, int>
{
    public record Model(int Id, int X, int Y, int Width, int Height);
    
    protected override TextParser<Model> LineParser { get; } =
        from id in Character.EqualTo('#').IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" @ "))
        from x in Numerics.IntegerInt32.ThenIgnore(Character.EqualTo(','))
        from y in Numerics.IntegerInt32.ThenIgnore(Span.EqualTo(": "))
        from width in Numerics.IntegerInt32.ThenIgnore(Character.EqualTo('x'))
        from height in Numerics.IntegerInt32
        select new Model(id, x, y, width, height);

    [Sample("#1 @ 1,3: 4x4\n#2 @ 3,1: 4x4\n#3 @ 5,5: 2x2\n", 4)]
    protected override int Part1(IEnumerable<Model> input)
    {
        var cells = new Dictionary<Position, int>();
        
        foreach (var model in input)
        {
            for (var x = 0; x < model.Width; x++)
            for (var y = 0; y < model.Height; y++)
            {
                var pos = new Position(model.X + x, model.Y + y);
                
                cells[pos] = cells.GetValueOrDefault(pos) + 1;
            }
        }
        
        return cells.Count(x => x.Value > 1);
    }

    [Sample("#1 @ 1,3: 4x4\n#2 @ 3,1: 4x4\n#3 @ 5,5: 2x2\n", 3)]
    protected override int Part2(IEnumerable<Model> input)
    {
        var cells = new Dictionary<Position, int>();
        
        foreach (var model in input)
        {
            for (var x = 0; x < model.Width; x++)
            for (var y = 0; y < model.Height; y++)
            {
                var pos = new Position(model.X + x, model.Y + y);
                
                cells[pos] = cells.GetValueOrDefault(pos) + 1;
            }
        }

        foreach (var model in input)
        {
            var overlaps = false;
            
            for (var x = 0; x < model.Width; x++)
            for (var y = 0; y < model.Height; y++)
            {
                var pos = new Position(model.X + x, model.Y + y);

                if (cells[pos] > 1)
                {
                    overlaps = true;
                }
            }

            if (!overlaps)
            {
                return model.Id;
            }
        }

        throw new NotImplementedException();
    }
}
