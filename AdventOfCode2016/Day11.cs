using System.Collections.Immutable;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace AdventOfCode2016;

[Day]
public partial class Day11 : ParseDay<Day11.Model, int, int>
{
    private const string Sample = "The first floor contains a hydrogen-compatible microchip and a lithium-compatible microchip.\nThe second floor contains a hydrogen generator.\nThe third floor contains a lithium generator.\nThe fourth floor contains nothing relevant.";
    
    private static readonly TextParser<Func<string, Item>> ItemTypeParser =
            Span.EqualTo(" generator").Select(_ => new Func<string, Item>(s => new Item.Generator(s)))
        .Or(Span.EqualTo("-compatible microchip").Select(_ => new Func<string, Item>(s => new Item.Microchip(s))));
    private static readonly TextParser<Item> ItemParser = Span.EqualTo("a ").IgnoreThen(Span.Regex("[a-z]+")).Then(ItemTypeParser).Select(x => x.Item2(x.Item1.ToStringValue()));
    private static readonly TextParser<TextSpan> ItemDelimiterParser = Span.Regex("(, and | and |, )");
    
    private static readonly TextParser<Item[]> ItemsParser = Span.EqualTo("nothing relevant").Select(_ => Array.Empty<Item>()).Try().Or(ItemParser.ManyDelimitedBy(ItemDelimiterParser));
    private static readonly TextParser<Floor> FloorParser = Span.Regex("The [a-z]+ floor contains ").IgnoreThen(ItemsParser).ThenIgnore(Character.EqualTo('.')).Select(x => new Floor(x.ToImmutableHashSet()));

    protected override TextParser<Model> Parser => FloorParser.ManyDelimitedBy(Character.EqualTo('\n')).AtEnd().Select(x => new Model(x.ToImmutableList()));

    [Sample(Sample, 11)]
    protected override int Part1(Model input) => Solve(input);
    
    protected override int Part2(Model input) =>
        Solve(new Model(input.Floors.SetItem(0, new Floor(input.Floors[0].Items.Union(new Item[]
        {
            new Item.Generator("elerium"),
            new Item.Microchip("elerium"),
            new Item.Generator("dilithium"),
            new Item.Microchip("dilithium"),
        })))));

    private int Solve(Model input)
    {
        var search = new Queue<(CompressedState, int)>();
        search.Enqueue((CompressedState.FromInput(input), 0));

        var seen = new HashSet<CompressedState>();

        while (search.Count > 0)
        {
            var (state, steps) = search.Dequeue();
            if (CompressedState.IsDone(state))
            {
                return steps;
            }

            foreach (var next in Next(state))
            {
                if (CompressedState.IsSafe(next) && seen.Add(next))
                {
                    search.Enqueue((next, steps + 1));
                }
            }
        }

        throw new Exception("no solution");
    }
 
        private IEnumerable<CompressedState> Next(CompressedState state)
    {
        var currentFloor = state.CurrentFloor;
        var upFloor = state.UpFloor;
        var downFloor = state.DownFloor;

        var chips = currentFloor.ChipIds.ToList();
        var generators = currentFloor.GeneratorIds.ToList(); 
        
        // 1 item up
        // 1 item down
        foreach (var chip in chips)
        {
            var currentFloorWithoutChip = currentFloor with { Chips = currentFloor.Chips & ~(1 << chip) };

            if (upFloor.HasValue) yield return state.With(state.Elevator, currentFloorWithoutChip).With(state.Elevator + 1, upFloor.Value with { Chips = upFloor.Value.Chips | (1 << chip) }) with { Elevator = state.Elevator + 1 };
            if (downFloor.HasValue) yield return state.With(state.Elevator, currentFloorWithoutChip).With(state.Elevator - 1, downFloor.Value with { Chips = downFloor.Value.Chips | (1 << chip) }) with { Elevator = state.Elevator - 1 };
        }
        
        foreach (var generator in generators)
        {
            var currentFloorWithoutGenerator = currentFloor with { Generators = currentFloor.Generators & ~(1 << generator) };

            if (upFloor.HasValue) yield return state.With(state.Elevator, currentFloorWithoutGenerator).With(state.Elevator + 1, upFloor.Value with { Generators = upFloor.Value.Generators | (1 << generator) }) with { Elevator = state.Elevator + 1 };
            if (downFloor.HasValue) yield return state.With(state.Elevator, currentFloorWithoutGenerator).With(state.Elevator - 1, downFloor.Value with { Generators = downFloor.Value.Generators | (1 << generator) }) with { Elevator = state.Elevator - 1 };
        }

        // 2 chips up/down
        foreach (var chip1 in chips)
        foreach (var chip2 in chips)
        {
            if (chip1 != chip2)
            {
                var currentFloorWithoutChips = currentFloor with { Chips = currentFloor.Chips & ~(1 << chip1) & ~(1 << chip2) };

                if (upFloor.HasValue) yield return state.With(state.Elevator, currentFloorWithoutChips).With(state.Elevator + 1, upFloor.Value with { Chips = upFloor.Value.Chips | (1 << chip1) | (1 << chip2) }) with { Elevator = state.Elevator + 1 };
                if (downFloor.HasValue) yield return state.With(state.Elevator, currentFloorWithoutChips).With(state.Elevator - 1, downFloor.Value with { Chips = downFloor.Value.Chips | (1 << chip1) | (1 << chip2) }) with { Elevator = state.Elevator - 1 };
            }
        }
        
        // 2 generators up/down
        foreach (var generator1 in generators)
        foreach (var generator2 in generators)
        {
            if (generator1 != generator2)
            {
                var currentFloorWithoutGenerators = currentFloor with { Generators = currentFloor.Generators & ~(1 << generator1) & ~(1 << generator2) };

                if (upFloor.HasValue) yield return state.With(state.Elevator, currentFloorWithoutGenerators).With(state.Elevator + 1, upFloor.Value with { Generators = upFloor.Value.Generators | (1 << generator1) | (1 << generator2) }) with { Elevator = state.Elevator + 1 };
                if (downFloor.HasValue) yield return state.With(state.Elevator, currentFloorWithoutGenerators).With(state.Elevator - 1, downFloor.Value with { Generators = downFloor.Value.Generators | (1 << generator1) | (1 << generator2) }) with { Elevator = state.Elevator - 1 };
            }
        }
        
        // 1 chip + same generator
        foreach (var chip in chips)
        {
            if (generators.Contains(chip))
            {
                var currentFloorWithoutChipAndGenerator = new CompressedFloor(currentFloor.Chips & ~(1 << chip), currentFloor.Generators & ~(1 << chip));

                if (upFloor.HasValue) yield return state.With(state.Elevator, currentFloorWithoutChipAndGenerator).With(state.Elevator + 1, new CompressedFloor(upFloor.Value.Chips | (1 << chip), upFloor.Value.Generators | (1 << chip))) with { Elevator = state.Elevator + 1 };
                if (downFloor.HasValue) yield return state.With(state.Elevator, currentFloorWithoutChipAndGenerator).With(state.Elevator - 1, new CompressedFloor(downFloor.Value.Chips | (1 << chip), downFloor.Value.Generators | (1 << chip))) with { Elevator = state.Elevator - 1 };
            }
        }
    }
    
    public readonly record struct CompressedState(int Elevator, CompressedFloor Floor0, CompressedFloor Floor1, CompressedFloor Floor2, CompressedFloor Floor3)
    {
        public static CompressedState FromInput(Model input)
        {
            var mapping = new Dictionary<string, int>();

            var floors = new List<CompressedFloor>();
            
            foreach (var floor in input.Floors)
            {
                var chips = 0;
                var generators = 0;

                foreach (var item in floor.Items)
                {
                    if (!mapping.TryGetValue(item.Material, out var id))
                    {
                        id = mapping.Count;
                        mapping.Add(item.Material, id);
                    }

                    if (item is Item.Microchip)
                    {
                        chips |= 1 << id;
                    }
                    else
                    {
                        generators |= 1 << id;
                    }
                }
                
                floors.Add(new CompressedFloor(chips, generators));
            }

            return new CompressedState(0, floors[0], floors[1], floors[2], floors[3]);
        }

        public CompressedFloor? UpFloor => Elevator < 3 ? this[Elevator + 1] : null;
        public CompressedFloor? DownFloor => Elevator >= 1 ? this[Elevator - 1] : null;
        public CompressedFloor CurrentFloor => this[Elevator];

        public CompressedFloor this[int index] => index switch
        {
            0 => Floor0,
            1 => Floor1,
            2 => Floor2,
            3 => Floor3,
        };

        public CompressedState With(int index, CompressedFloor floor) => index switch
        {
            0 => this with { Floor0 = floor },
            1 => this with { Floor1 = floor },
            2 => this with { Floor2 = floor },
            3 => this with { Floor3 = floor },
        };

        public static bool IsDone(CompressedState state) => state.Floor0.IsEmpty && state.Floor1.IsEmpty && state.Floor2.IsEmpty;
        public static bool IsSafe(CompressedState next) => next.Floor0.IsSafe && next.Floor1.IsSafe && next.Floor2.IsSafe && next.Floor3.IsSafe;
    }

    public readonly record struct CompressedFloor(int Chips, int Generators)
    {
        public IEnumerable<int> ChipIds => EnumerateIds(Chips);
        public IEnumerable<int> GeneratorIds => EnumerateIds(Generators);

        private static IEnumerable<int> EnumerateIds(int ids) => Enumerable.Range(0, 32).Where(i => (ids & (1 << i)) != 0);
        
        public bool IsEmpty => Chips == 0 && Generators == 0;
        public bool IsSafe => Generators == 0 || (Chips & ~Generators) == 0;
    }
    
    public record Model(ImmutableList<Floor> Floors);
    public record Floor(ImmutableHashSet<Item> Items);
    public abstract record Item(string Material)
    {
        public record Generator(string Material) : Item(Material);
        public record Microchip(string Material) : Item(Material);
    }
}
