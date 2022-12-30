using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2016;

[Day]
public partial class Day10 : ParseLineDay<Day10.Model, int, int>
{
    private const string Sample = "value 61 goes to bot 2\nbot 2 gives low to bot 1 and high to bot 0\nvalue 3 goes to bot 1\nbot 1 gives low to output 1 and high to bot 0\nbot 0 gives low to output 2 and high to output 0\nvalue 17 goes to bot 2";
    
    private static readonly TextParser<Target> BotTargetParser = Span.EqualTo("bot ").IgnoreThen(Numerics.IntegerInt32).Select(x => (Target)new Target.Bot(x));
    private static readonly TextParser<Target> OutputTargetParser = Span.EqualTo("output ").IgnoreThen(Numerics.IntegerInt32).Select(x => (Target)new Target.Output(x));
    private static readonly TextParser<Target> TargetParser = BotTargetParser.Or(OutputTargetParser);

    private static readonly TextParser<Model> BotParser = Span.EqualTo("bot ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" gives low to ")).Then(TargetParser).ThenIgnore(Span.EqualTo(" and high to ")).Then(TargetParser)
        .Select(x => (Model)new Model.Bot(x.Item1.Item1, x.Item1.Item2, x.Item2));
    private static readonly TextParser<Model> ValueParser = Span.EqualTo("value ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" goes to bot ")).Then(Numerics.IntegerInt32)
        .Select(x => (Model)new Model.Value(x.Item1, x.Item2));

    protected override TextParser<Model> LineParser => BotParser.Or(ValueParser);

    [Sample(Sample, 2)]
    protected override int Part1(IEnumerable<Model> input)
    {
        var inventories = new Inventories();

        var inputList = input.ToList();
        while (true)
        {
            foreach (var model in inputList)
            {
                switch (model)
                {
                    case Model.Bot bot:
                        var inventory = inventories.Get(new Target.Bot(bot.Source));
                        if (inventory.Count == 2)
                        {
                            var list = inventory.ToList();
                            var (low, high) = list[0] < list[1] ? (list[0], list[1]) : (list[1], list[0]);

                            if (low == 17 && high == 61)
                            {
                                return bot.Source;
                            }
                            
                            inventories.Add(bot.Low, low);
                            inventories.Add(bot.High, high);
                        }
                        break;
                    
                    case Model.Value value:
                        inventories.Add(new Target.Bot(value.Target), value.Data);
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException(nameof(model));
                }
            }
        }
    }

    protected override int Part2(IEnumerable<Model> input)
    {
        var inventories = new Inventories();

        var inputList = input.ToList();
        while (true)
        {
            foreach (var model in inputList)
            {
                switch (model)
                {
                    case Model.Bot bot:
                        var inventory = inventories.Get(new Target.Bot(bot.Source));
                        if (inventory.Count == 2)
                        {
                            var list = inventory.ToList();
                            var (low, high) = list[0] < list[1] ? (list[0], list[1]) : (list[1], list[0]);

                            inventories.Add(bot.Low, low);
                            inventories.Add(bot.High, high);
                        }
                        break;
                    
                    case Model.Value value:
                        inventories.Add(new Target.Bot(value.Target), value.Data);
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException(nameof(model));
                }
            }

            var output0 = inventories.Get(new Target.Output(0));
            var output1 = inventories.Get(new Target.Output(1));
            var output2 = inventories.Get(new Target.Output(2));

            if (output0.Count == 1 && output1.Count == 1 && output2.Count == 1)
            {
                return output0.First() * output1.First() * output2.First();
            }
        }
    }

    public abstract record Model
    {
        public record Bot(int Source, Target Low, Target High) : Model;
        public record Value(int Data, int Target) : Model;
    }

    public abstract record Target
    {
        public record Bot(int Id) : Target;
        public record Output(int Id) : Target;
    }

    public class Inventories
    {
        private readonly Dictionary<Target, HashSet<int>> _inventories = new();

        public void Add(Target target, int value)
        {
            if (!_inventories.TryGetValue(target, out var targetInventory))
            {
                targetInventory = new HashSet<int>();
                _inventories.Add(target, targetInventory);
            }

            targetInventory.Add(value);
        }
        
        public IReadOnlyCollection<int> Get(Target target)
        {
            if (_inventories.TryGetValue(target, out var targetInventory))
            {
                return targetInventory;
            }
            
            targetInventory = new HashSet<int>();
            _inventories.Add(target, targetInventory);

            return targetInventory;
        }

    }
}
