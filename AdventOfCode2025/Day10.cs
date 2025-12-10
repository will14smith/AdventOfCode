using Microsoft.Z3;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2025;

[Day]
public partial class Day10 : ParseDay<Day10.Model, int, int>
{
    public record Machine(IReadOnlyList<bool> Lights, IReadOnlyList<IReadOnlyList<int>> Buttons, IReadOnlyList<int> Joltages);
    public record Model(IReadOnlyList<Machine> Machines);

    private static readonly TextParser<IReadOnlyList<bool>> LightsParser =
        from open in Character.EqualTo('[')
        from lights in Character.EqualTo('.').Or(Character.EqualTo('#')).Many()
        from close in Character.EqualTo(']')
        select (IReadOnlyList<bool>) lights.Select(c => c == '#').ToArray();
    private static readonly TextParser<IReadOnlyList<int>> ButtonParser =
        from open in Character.EqualTo('(')
        from indices in Numerics.IntegerInt32.ManyDelimitedBy(Character.EqualTo(','))
        from close in Character.EqualTo(')')
        select (IReadOnlyList<int>) indices;
    private static readonly TextParser<IReadOnlyList<int>> JoltageParser =
        from open in Character.EqualTo('{')
        from joltages in Numerics.IntegerInt32.ManyDelimitedBy(Character.EqualTo(','))
        from close in Character.EqualTo('}')
        select (IReadOnlyList<int>) joltages;
    
    private static readonly TextParser<Machine> MachineParser =
        from lights in LightsParser.ThenIgnore(Character.EqualTo(' '))
        from buttons in ButtonParser.ThenIgnore(Character.EqualTo(' ')).Many()
        from joltages in JoltageParser
        select new Machine(lights, buttons, joltages);
    
    protected override TextParser<Model> Parser { get; } = 
        from machines in MachineParser.ThenIgnoreOptional(Character.EqualTo('\n')).Many()
        select new Model(machines);

    [Sample("[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}\n[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}\n[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}\n", 7)]
    protected override int Part1(Model input) => input.Machines.Sum(Solve1);

    private int Solve1(Machine machine)
    {
        var initial = 0;
        var target = 0;
        for (var i = 0; i < machine.Lights.Count; i++)
        {
            if (machine.Lights[i])
            {
                target |= 1 << i;
            }
        }
        
        var buttons = machine.Buttons.Select(button => button.Aggregate(0, (current, lightIndex) => current | (1 << lightIndex))).ToArray();
        
        var search = new Queue<(int Lights, int Presses)>();
        search.Enqueue((initial, 0));
        
        var visited = new HashSet<int>();
        
        while(search.Count > 0)
        {
            var (current, presses) = search.Dequeue();
            
            if (current == target)
            {
                return presses;
            }
            
            foreach (var button in buttons)
            {
                var next = current ^ button;
                if (visited.Add(next))
                {
                    search.Enqueue((next, presses + 1));
                }
            }
        }
        
        throw new InvalidOperationException("no solution found");
    }

    [Sample("[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}\n[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}\n[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}\n", 33)]
    protected override int Part2(Model input) => input.Machines.Sum(Solve2);

    private int Solve2(Machine machine)
    {
        var target = machine.Joltages;
        
        // target = linear combination of buttons
        // target = Xb0 + Yb1 + ... + Zbn
        // minimize X + Y + ... + Z
        
        // example:
        
        //   a    b      c    d      e      f
        // [(3), (1,3), (2), (2,3), (0,2), (0,1)]
        // a      = [0,0,0,1]
        // b      = [0,1,0,1]
        // c      = [0,0,1,0]
        // d      = [0,0,1,1]
        // e      = [1,0,1,0]
        // f      = [1,1,0,0]
        // target = [3,5,4,7]
        
        // target = Aa + Bb + Cc + Dd + Ee + Ff
        // 3 = E + F
        // 5 = B + F
        // 4 = C + D + E
        // 7 = A + B + D
        // find solution with min A + B + C + D + E + F
        
        var ctx = new Context();
        var optimize = ctx.MkOptimize();

        var buttonPressCount = new List<ArithExpr>();
        for (var i = 0; i < machine.Buttons.Count; i++)
        {
            var buttonVar = ctx.MkIntConst($"b{i}");
            optimize.Add(ctx.MkGe(buttonVar, ctx.MkInt(0)));
            buttonPressCount.Add(buttonVar);
        }
        
        for (var targetIndex = 0; targetIndex < target.Count; targetIndex++)
        {
            var terms = new List<ArithExpr>();
            for (var j = 0; j < machine.Buttons.Count; j++)
            {
                terms.AddRange(machine.Buttons[j].Where(index => index == targetIndex).Select(_ => buttonPressCount[j]));
            }
            
            var sum = ctx.MkAdd(terms.ToArray());
            optimize.Add(ctx.MkEq(sum, ctx.MkInt(target[targetIndex])));
        }
        
        var objective = ctx.MkAdd(buttonPressCount.ToArray());
        optimize.MkMinimize(objective);

        if (optimize.Check() != Status.SATISFIABLE)
        {
            throw new InvalidOperationException("no solution found");
        }

        return buttonPressCount
            .Select(buttonVar => optimize.Model.Evaluate(buttonVar))
            .Select(value => ((IntNum)value).Int).Sum();

    }
}