using System.Collections;

namespace AdventOfCode2024;

[Day]
public partial class Day24 : Day<Day24.Model, long, string>
{
    public record Model(IReadOnlyList<(string Wire, bool State)> Initial, IReadOnlyList<Gate> Gates);
    public record Gate(GateType Type, string WireA, string WireB, string WireOut);
    public enum GateType { AND, OR, XOR }
    
    protected override Model Parse(string input)
    {
        var sections = input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
        
        var initial = new List<(string Wire, bool State)>();
        var gates = new List<Gate>();
        
        foreach (var line in sections[0].Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = line.Split(": ");
            
            initial.Add((parts[0], int.Parse(parts[1]) != 0));
        }

        foreach (var line in sections[1].Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var type = parts[1] switch
            {
                "AND" => GateType.AND,
                "OR" => GateType.OR,
                "XOR" => GateType.XOR,
            };
            
            gates.Add(new Gate(type, parts[0], parts[2], parts[4]));
        }
        
        return new Model(initial, gates);
    }

    [Sample("x00: 1\nx01: 0\nx02: 1\nx03: 1\nx04: 0\ny00: 1\ny01: 1\ny02: 1\ny03: 1\ny04: 1\n\nntg XOR fgs -> mjb\ny02 OR x01 -> tnw\nkwq OR kpj -> z05\nx00 OR x03 -> fst\ntgd XOR rvg -> z01\nvdt OR tnw -> bfw\nbfw AND frj -> z10\nffh OR nrd -> bqk\ny00 AND y03 -> djm\ny03 OR y00 -> psh\nbqk OR frj -> z08\ntnw OR fst -> frj\ngnj AND tgd -> z11\nbfw XOR mjb -> z00\nx03 OR x00 -> vdt\ngnj AND wpb -> z02\nx04 AND y00 -> kjc\ndjm OR pbm -> qhw\nnrd AND vdt -> hwm\nkjc AND fst -> rvg\ny04 OR y02 -> fgs\ny01 AND x02 -> pbm\nntg OR kjc -> kwq\npsh XOR fgs -> tgd\nqhw XOR tgd -> z09\npbm OR djm -> kpj\nx03 XOR y03 -> ffh\nx00 XOR y04 -> ntg\nbfw OR bqk -> z06\nnrd XOR fgs -> wpb\nfrj XOR qhw -> z04\nbqk OR frj -> z07\ny03 OR x01 -> nrd\nhwm AND bqk -> z03\ntgd XOR rvg -> z12\ntnw OR pbm -> gnj\n", 2024L)]
    protected override long Part1(Model input)
    {
        var wires = input.Gates.ToDictionary(x => x.WireOut, Value (x) => new Value.Gate(x.Type, new Value.Wire(x.WireA), new Value.Wire(x.WireB)));
        foreach (var (wire, state) in input.Initial)
        {
            wires[wire] = new Value.Constant(state);
        }

        var results = wires.Keys.Where(x => x.StartsWith("z")).OrderByDescending(x => int.Parse(x[1..])).Select(x => wires[x]).Select(Evaluate);
        return results.Aggregate(0L, (i, b) => (i << 1) | (b ? 1L : 0L));
        
        bool Evaluate(Value value)
        {
            switch (value)
            {
                case Value.Constant constant: return constant.Value;
                case Value.Wire wire: return Evaluate(wires[wire.Name]);

                case Value.Gate gate:
                    var a = Evaluate(gate.A);
                    var b = Evaluate(gate.B);

                    switch (gate.Type)
                    {
                        case GateType.AND: return a && b;
                        case GateType.OR: return a || b;
                        case GateType.XOR: return a ^ b;
                        
                        default:  throw new ArgumentOutOfRangeException();
                    }
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        throw new NotImplementedException();
    }

    private abstract record Value
    {
        public record Wire(string Name) : Value;
        public record Constant(bool Value) : Value;
        public record Gate(GateType Type, Value A, Value B) : Value;
    }


    protected override string Part2(Model input)
    {
        Output.WriteLine("digraph {");

        var renames = new Dictionary<string, string>();

        IReadOnlyDictionary<string, string> switches = new Dictionary<string, string>()
        {
            { "shj", "z07" },
            { "z07", "shj" },
            { "wkb", "tpk" },
            { "tpk", "wkb" },
            { "pfn", "z23" },
            { "z23", "pfn" },
            { "kcd", "z27" },
            { "z27", "kcd" },
        };
        var gates = input.Gates.Select(x =>
        {
            return x with { WireOut = Switch(x.WireOut) };

            string Switch(string wire)
            {
                return switches.TryGetValue(wire, out var s) ? s : wire;
            }
        }).ToList();
        
        // xN ^ yN => sN
        // xN & yN => hN
        
        // c0 = h0
        
        // sN ^ c(N-1) => zN
        // sN & c(N-1) => rN
        // hN | rN => cN

        foreach (var gate in gates)
        {
            var a = ParseWire(gate.WireA);
            var b = ParseWire(gate.WireB);
            
            var x = ChooseWire('x', a, b);
            var y = ChooseWire('y', a, b);
            
            if (gate.Type == GateType.AND && x != null && x == y)
            {
                if (x == 0)
                {
                    renames.Add(gate.WireOut, $"c{x}");
                }
                else
                {
                    renames.Add(gate.WireOut, $"h{x}");
                }
            }
            else if(gate.Type == GateType.XOR && x != null && x == y)
            {
                renames.Add(gate.WireOut, $"s{x}");
            }
        }

        for (var i = 1; i < 45; i++)
        {
            // sN ^ c(N-1) => zN
            // sN & c(N-1) => rN
            foreach (var gate in gates)
            {
                var a = ParseWire(gate.WireA);
                var b = ParseWire(gate.WireB);
            
                var s = ChooseWire('s', a, b);
                var c = ChooseWire('c', a, b);

                if (s != i)
                {
                    continue;
                }
                
                if (s.HasValue && c.HasValue && s.Value == c.Value + 1)
                {
                    if (gate.Type == GateType.AND)
                    {
                        renames.Add(gate.WireOut, $"r{s}");
                    }
                    else if (gate.Type == GateType.XOR)
                    {
                        renames.Add(gate.WireOut, $"z{s:00}");
                    }
                }
            }
            
            // hN | rN => cN
            foreach (var gate in gates)
            {
                var a = ParseWire(gate.WireA);
                var b = ParseWire(gate.WireB);
            
                var h = ChooseWire('h', a, b);
                var r = ChooseWire('r', a, b);

                if (h != i)
                {
                    continue;
                }
                
                if (h.HasValue && r.HasValue && h.Value == r.Value)
                {
                    if (gate.Type == GateType.OR)
                    {
                        renames.Add(gate.WireOut, $"c{h}");
                        break;
                    }
                }
            }
        }

        (char, int)? ParseWire(string input) => int.TryParse(input[1..], out var value) ? (input[0], value) : renames.TryGetValue(input, out var rename) ? ParseWire(rename) : null; 
        static int? ChooseWire(char target, (char, int)? a, (char, int)? b)
        {
            if (a.HasValue && a.Value.Item1 == target) return a.Value.Item2;
            if (b.HasValue && b.Value.Item1 == target) return b.Value.Item2;
            return null;
        }

        var counter = 1;
        foreach (var gate in gates)
        {
            Output.WriteLine($"gate{counter} [label=\"{gate.Type}\"];");
            Output.WriteLine($"{gate.WireA} -> gate{counter};");
            Output.WriteLine($"{gate.WireB} -> gate{counter};");
            Output.WriteLine($"gate{counter} -> {gate.WireOut};");

            counter++;
        }
        
        foreach (var kvp in renames)
        {
            Output.WriteLine($"{kvp.Key} [label=\"{kvp.Value}\"];");
        }
        
        Output.WriteLine("}");

        return string.Join(",", switches.Keys.OrderBy(x => x));
    }
}