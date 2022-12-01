namespace AdventOfCode2022;

[Day]
public partial class Day01 : Day<Day01.Model, int, int>
{
    private const string Sample = "1000\n2000\n3000\n\n4000\n\n5000\n6000\n\n7000\n8000\n9000\n\n10000\n";
    
    protected override Model Parse(string input) => new(input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries).Select(ParseElf).ToList());
    private static Elf ParseElf(string input) => new(input.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).Select(calories => new Food(calories)).ToList());

    [Sample(Sample, 24000)]
    protected override int Part1(Model input) => input.Elves.Max(elf => elf.Food.Sum(food => food.Calories));
    [Sample(Sample, 45000)]
    protected override int Part2(Model input) => input.Elves.Select(elf => elf.Food.Sum(food => food.Calories)).OrderByDescending(x => x).Take(3).Sum();

    public record Model(List<Elf> Elves);
    public record Elf(List<Food> Food);
    public record Food(int Calories);
}
