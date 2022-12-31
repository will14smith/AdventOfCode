namespace AdventOfCode2016;

[Day]
public partial class Day19 : Day<Day19.Model, int, int>
{
    protected override Model Parse(string input) => new Model(int.Parse(input));

    [Sample("5", 3)]
    protected override int Part1(Model input)
    {
        var elves = Enumerable.Range(1, input.Count).Select(x => new Elf(x)).ToList();

        for (var j = 0; j < elves.Count; j++)
        {
            elves[j].Next = elves[(j + 1) % elves.Count];
        }
        
        var elf = elves[0];
        while (elf.Next != elf)
        {
            elf.Next = elf.Next.Next;
            elf = elf.Next;
        }

        return elf.Id;
    }

    [Sample("5", 2)]
    [Sample("9", 9)]
    protected override int Part2(Model input)
    {
        var elves = Enumerable.Range(1, input.Count).Select(x => new Elf(x)).ToList();
        var count = input.Count;
        
        for (var j = 0; j < elves.Count; j++)
        {
            elves[j].Next = elves[(j + 1) % elves.Count];
        }
        
        var elf = elves[0];
        var cursor = elf;
        var cursorDistance = 0;
        
        while (elf.Next != elf)
        {
            var steps = count-- / 2;
            while (++cursorDistance < steps)
            {
                cursor = cursor.Next;
            }
            cursor.Next = cursor.Next.Next;
            
            elf = elf.Next;
            cursorDistance -= 2;
        }

        return elf.Id;
    }

    public record Model(int Count);

    public class Elf
    {
        public int Id { get; }
        public Elf Next { get; set; }

        public Elf(int id) => Id = id;

        public override string ToString() => Id.ToString();
    }
}
