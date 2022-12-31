using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2016;

[Day]
public partial class Day20 : ParseLineDay<Day20.Model, uint, long>
{
    protected override TextParser<Model> LineParser => Numerics.NaturalUInt32.ThenIgnore(Character.EqualTo('-')).Then(Numerics.NaturalUInt32).Select(x => new Model(x.Item1, x.Item2));

    [Sample("5-8\n0-2\n4-7", 3U)]
    protected override uint Part1(IEnumerable<Model> input) => ComputeAllowed(input).Allowed.Min(x => x.Lower);
    protected override long Part2(IEnumerable<Model> input) => ComputeAllowed(input).Allowed.Sum(x => (x.Upper - x.Lower) + 1);

    private static List ComputeAllowed(IEnumerable<Model> input) => input.Aggregate(List.Initial, (current, model) => current.Remove(model));

    public record List(List<Model> Allowed)
    {
        public static readonly List Initial = new(new List<Model>(new[] { new Model(0, uint.MaxValue) }));

        public List Remove(Model model) => new(Allowed.SelectMany(x => x.Remove(model)).ToList());
    }
    
    public record Model(uint Lower, uint Upper)
    {
        public IEnumerable<Model> Remove(Model other)
        {
            // other fully before 
            if (other.Upper < Lower)
            {
                return new[] { this };
            }
            
            // other fully after
            if (other.Lower > Upper)
            {
                return new[] { this };
            }

            // other fully contains
            if (other.Lower <= Lower && other.Upper >= Upper) return Array.Empty<Model>();

            // other removes lower end
            if (other.Lower <= Lower && other.Upper >= Lower) return new[] { this with { Lower = other.Upper + 1 } };
          
            // other removes upper end
            if (other.Lower <= Upper && other.Upper >= Upper) return new[] { this with { Upper = other.Lower - 1 } };

            // other is in middle
            return new[]
            {
                this with { Upper = other.Lower - 1 },
                this with { Lower = other.Upper + 1 },
            };
        }
    }
}
