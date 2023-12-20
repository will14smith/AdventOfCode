using System.Collections.Immutable;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2023;

[Day]
public partial class Day20 : ParseDay<Day20.Model, int, long>
{
    private static readonly TextParser<string> ModuleNameParser = Span.Regex("[a-z]+").Select(x => x.ToStringValue());
    private static readonly TextParser<(string, ModuleType)> ModuleNameTypeParser = 
        Span.EqualTo("broadcaster").Select(_ => ("broadcaster", ModuleType.Broadcaster))
            .Or(Span.EqualTo('%').IgnoreThen(ModuleNameParser).Select(x => (x, ModuleType.FlipFlop)))
            .Or(Span.EqualTo('&').IgnoreThen(ModuleNameParser).Select(x => (x, ModuleType.Conjunction)));
    private static readonly TextParser<Module> ModuleParser = ModuleNameTypeParser.ThenIgnore(Span.EqualTo(" -> ")).Then(ModuleNameParser.ManyDelimitedBy(Span.EqualTo(", "))).Select(x => new Module(x.Item1.Item1, x.Item1.Item2, x.Item2));

    protected override TextParser<Model> Parser { get; } = ModuleParser.ManyDelimitedBy(Span.EqualTo('\n')).Select(x => new Model(x));

    [Sample("broadcaster -> a, b, c\n%a -> b\n%b -> c\n%c -> inv\n&inv -> a", 32000000)]
    [Sample("broadcaster -> a\n%a -> inv, con\n&inv -> b\n%b -> con\n&con -> output", 11687500)]
    protected override int Part1(Model input)
    {
        var state = State.From(input);
        
        var highTotal = 0;
        var lowTotal = 0;
        
        for (var i = 0; i < 1000; i++)
        {
            (state, var high, var low) = state.PressButton();
            highTotal += high;
            lowTotal += low;
        }
        
        return highTotal * lowTotal;
    }

    protected override long Part2(Model input)
    {
        // very input specific...
        // rx is only fed by vd
        // vd is fed by 4 "binary counters"
        // vd is triggered every LCM of all the counters cycle lengths

        var counters = new[]
        {
            "broadcaster -> gn, gb, rb, df\n%df -> hc, lq\n%lq -> gq, hc\n%gq -> hc, ch\n%ch -> qr\n%qr -> dj\n%dj -> nl\n%nl -> cc, hc\n%cc -> vg\n%vg -> fr, hc\n%fr -> ks, hc\n%ks -> lc, hc\n%lc -> hc\n&hc -> qr, ch, df, dj, cc, rd, output",
            "broadcaster -> gn, gb, rb, df\n%rb -> ck, mt\n%mt -> sx\n%sx -> jl\n%jl -> ck, jn\n%jn -> ck, nz\n%nz -> hh\n%hh -> ck, ms\n%ms -> xz\n%xz -> ck, bg\n%bg -> ck, rq\n%rq -> ck, ts\n%ts -> ck\n&ck -> nz, fv, rb, sx, ms, mt, output",
            "broadcaster -> gn, gb, rb, df\n%gb -> vx, qt\n%vx -> fp\n%fp -> rp, qt\n%rp -> qt, gh\n%gh -> td\n%td -> kz\n%kz -> jb, qt\n%jb -> fz\n%fz -> qt, zq\n%zq -> qt, xm\n%xm -> qt, cs\n%cs -> qt\n&qt -> jb, vx, bt, gh, td, gb, output",
            "broadcaster -> gn, gb, rb, df\n%gn -> kb, tn\n%tn -> pf\n%pf -> gd\n%gd -> gc\n%gc -> kb, pv\n%pv -> ps\n%ps -> kb, rf\n%rf -> kb, nm\n%nm -> gt\n%gt -> pp, kb\n%pp -> gv, kb\n%gv -> kb\n&kb -> pv, pr, tn, nm, pf, gn, gd, output",
        };

        var models = counters.Select(Parse);
        var cycles = models.Select(model => CycleDetection.Detect(() => State.From(model), x => x.PressButton().NextState, State.Equal));
        return cycles.Select(x => (long)x.Length).Aggregate(NumberExtensions.LowestCommonMultiple);
    }

    public record State(IReadOnlyDictionary<string, Module> Modules, ImmutableDictionary<string, bool> FlipFlops, ImmutableDictionary<string, ImmutableDictionary<string, bool>> Conjunctions)
    {
        public static State From(Model model) =>
            new(
                model.Modules.ToDictionary(x => x.Name),
                model.Modules.Where(x => x.Type == ModuleType.FlipFlop).ToImmutableDictionary(x => x.Name, _ => false),
                model.Modules.Where(x => x.Type == ModuleType.Conjunction).ToImmutableDictionary(x => x.Name, x =>
                {
                    var sources = model.Modules.Where(y => y.Destinations.Contains(x.Name));
                    return sources.ToImmutableDictionary(y => y.Name, _ => false);
                })
            );
        
        public (State NextState, int HighCount, int LowCount) PressButton()
        {
            var state = this;
            var highCount = 0;
            var lowCount = 0;
            
            var pulses = new Queue<(string Source, string Target, bool Signal)>();
            pulses.Enqueue(("broadcaster", "broadcaster", false));

            while (pulses.Count > 0)
            {
                var (source, target, signal) = pulses.Dequeue();
                if (signal) highCount++; else lowCount++;
                
                if (Modules.TryGetValue(target, out var targetModule))
                {
                    state = SendSignal(state, pulses, Modules[source], targetModule, signal);
                }
            }

            return (state, highCount, lowCount);
        }

        private static State SendSignal(State state, Queue<(string Source, string Target, bool Signal)> pulses, Module source, Module target, bool signal)
        {
            switch (target.Type)
            {
                case ModuleType.Broadcaster:
                    foreach (var destination in target.Destinations)
                    {
                        pulses.Enqueue((target.Name, destination, signal));
                    }

                    return state;

                case ModuleType.FlipFlop:
                {
                    if (signal)
                    {
                        return state;
                    }

                    var current = state.FlipFlops.GetValueOrDefault(target.Name, false);
                    var output = !current;

                    var newFlipFlops = state.FlipFlops.SetItem(target.Name, output);
                    foreach (var destination in target.Destinations)
                    {
                        pulses.Enqueue((target.Name, destination, output));
                    }

                    return state with { FlipFlops = newFlipFlops };
                }

                case ModuleType.Conjunction:
                {
                    var memory = state.Conjunctions[target.Name];
                    var newMemory = memory.SetItem(source.Name, signal);
                    var newConjunctions = state.Conjunctions.SetItem(target.Name, newMemory);
                    
                    var output = !newMemory.Values.All(x => x);

                    foreach (var destination in target.Destinations)
                    {
                        pulses.Enqueue((target.Name, destination, output));
                    }

                    return state with { Conjunctions = newConjunctions };
                }

                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        public static bool Equal(State s1, State s2)
        {
            foreach (var (key, s1Value) in s1.FlipFlops)
            {
                if (s2.FlipFlops[key] != s1Value) return false;
            }
            
            foreach (var (key, s1Value) in s1.Conjunctions)
            {
                var s2Value = s2.Conjunctions[key];
                
                foreach (var (keyInner, s1InnerValue) in s1Value)
                {
                    if (s2Value[keyInner] != s1InnerValue) return false;
                }
            }

            return true;
        }
    }

    public record Model(IReadOnlyList<Module> Modules);
    public record Module(string Name, ModuleType Type, IReadOnlyList<string> Destinations);
    public enum ModuleType
    {
        Broadcaster,
        FlipFlop,
        Conjunction,
    }
}