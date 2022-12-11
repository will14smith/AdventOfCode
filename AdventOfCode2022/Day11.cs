using System.Collections.Immutable;

namespace AdventOfCode2022;

[Day]
public partial class Day11 : ParseDay<Day11.Model, Day11.TokenType, long, long>
{ 
    private const string Sample = "Monkey 0:\n  Starting items: 79, 98\n  Operation: new = old * 19\n  Test: divisible by 23\n    If true: throw to monkey 2\n    If false: throw to monkey 3\n\nMonkey 1:\n  Starting items: 54, 65, 75, 74\n  Operation: new = old + 6\n  Test: divisible by 19\n    If true: throw to monkey 2\n    If false: throw to monkey 0\n\nMonkey 2:\n  Starting items: 79, 60, 97\n  Operation: new = old * old\n  Test: divisible by 13\n    If true: throw to monkey 1\n    If false: throw to monkey 3\n\nMonkey 3:\n  Starting items: 74\n  Operation: new = old + 3\n  Test: divisible by 17\n    If true: throw to monkey 0\n    If false: throw to monkey 1";
  
    [Sample(Sample, 10605L)]
    protected override long Part1(Model model) => RunRounds(model, 20, true);

    [Sample(Sample, 2713310158L)]
    protected override long Part2(Model model) => RunRounds(model, 10_000, false);

    private static long RunRounds(Model model, int count, bool relaxed)
    {
        for (var round = 0; round < count; round++)
        {
            model = RunRound(model, relaxed);
        }

        return model.Monkeys.Select(x => x.ItemsInspected).OrderByDescending(x => x).Take(2).Aggregate(1L, (acc, x) => acc * x);
    }
    
    private static Model RunRound(Model model, bool relaxed)
    {
        for (var i = 0; i < model.Monkeys.Count; i++)
        {
            model = RunTurn(model, i, relaxed);
        }

        return model;
    }

    private static Model RunTurn(Model model, int id, bool relaxed)
    {
        // the monkey could be its own target, so make sure to look it up each time
        while (!model.Monkeys[id].Items.IsEmpty)
        {
            var monkey = model.Monkeys[id];
            model = model.Dequeue(id, out var item);
            
            var newItem = Evaluate(monkey.WorryExpression, item);
            if (relaxed) newItem /= 3;
            newItem %= model.Modulo;

            var test = newItem % monkey.Action.Modulo == 0;
            model = model.Enqueue(test ? monkey.Action.DivisibleTarget : monkey.Action.IndivisibleTarget, newItem);
        }
        
        return model;
    }

    private static long Evaluate(MonkeyExpression expr, long old) =>
        expr switch
        {
            MonkeyExpression.Constant constant => constant.Value,
            MonkeyExpression.Old => old,

            MonkeyExpression.Add add => Evaluate(add.Left, old) + Evaluate(add.Right, old),
            MonkeyExpression.Mult mult => Evaluate(mult.Left, old) * Evaluate(mult.Right, old),

            _ => throw new ArgumentOutOfRangeException(nameof(expr))
        };

    public record Model(ImmutableList<Monkey> Monkeys, long Modulo)
    {
        public Model Enqueue(int id, long item) => this with { Monkeys = Monkeys.SetItem(id, Monkeys[id].Enqueue(item)) };
        public Model Dequeue(int id, out long item) => this with { Monkeys = Monkeys.SetItem(id, Monkeys[id].Dequeue(out item)) };
    }

    public record Monkey(int Id, ImmutableQueue<long> Items, MonkeyExpression WorryExpression, MonkeyAction Action, long ItemsInspected = 0)
    {
        public Monkey Enqueue(long item) => this with { Items = Items.Enqueue(item) };
        public Monkey Dequeue(out long item) => this with { Items = Items.Dequeue(out item), ItemsInspected = ItemsInspected + 1 };
    }
    public abstract record MonkeyExpression
    {
        public record Old : MonkeyExpression;
        public record Constant(long Value) : MonkeyExpression;
        public record Add(MonkeyExpression Left, MonkeyExpression Right) : MonkeyExpression;
        public record Mult(MonkeyExpression Left, MonkeyExpression Right) : MonkeyExpression;
    }
    public record MonkeyAction(long Modulo, int DivisibleTarget, int IndivisibleTarget);

}
