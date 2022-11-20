namespace AdventOfCode2016;

[Day]
public partial class Day03 : LineDay<Day03.Model, int, int>
{
    protected override Model ParseLine(string input)
    {
        var parts = input.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        return new Model(parts[0], parts[1], parts[2]);
    }

    [Sample("5 10 25", 0)]
    [Sample("5 10 11", 1)]
    protected override int Part1(IEnumerable<Model> input) => CountValid(input);
    
    protected override int Part2(IEnumerable<Model> input) => CountValid(Remap(input));

    private static IEnumerable<Model> Remap(IEnumerable<Model> input)
    {
        var array = input.ToArray();
        var newArray = new Model[array.Length];
        
        for (var i = 0; i < array.Length; i += 3)
        {
            newArray[i] = new Model(array[i].A, array[i + 1].A, array[i + 2].A);
            newArray[i+1] = new Model(array[i].B, array[i + 1].B, array[i + 2].B);
            newArray[i+2] = new Model(array[i].C, array[i + 1].C, array[i + 2].C);
        }

        return newArray;
    }

    private static int CountValid(IEnumerable<Model> input)
    {
        return input.Select(x => x.Ordered).Count(x => x.X + x.Y > x.Z);
    }


    public record Model(int A, int B, int C)
    {
        public (int X, int Y, int Z) Ordered
        {
            get
            {
                var x = A;
                var y = B;
                var z = C;

                if (x > y) (x, y) = (y, x);
                if (y > z) (y, z) = (z, y);
                if (x > y) (x, y) = (y, x);
                
                return (x, y, z);
            }
        }
            
    }
}
