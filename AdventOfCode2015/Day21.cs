using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2015;

[Day]
public partial class Day21 : ParseDay<Day21.Player, int, int>
{
    private static readonly IReadOnlyList<Item> Weapons = new[]
    {
        new Item("Dagger", 8, 4, 0),
        new Item("Shortsword", 10, 5, 0),
        new Item("Warhammer", 25, 6, 0),
        new Item("Longsword", 40, 7, 0),
        new Item("Greataxe", 74, 8, 0),
    };

    private static readonly IReadOnlyList<Item> Armor = new[]
    {
        new Item("No Armor", 0, 0, 0),
        new Item("Leather", 13, 0, 1),
        new Item("Chainmail", 31, 0, 2),
        new Item("Splintmail", 53, 0, 3),
        new Item("Bandedmail", 75, 0, 4),
        new Item("Platemail", 102, 0, 5),
    };

    private static readonly IReadOnlyList<Item> Rings = new[]
    {
        new Item("No Ring 1", 0, 0, 0),
        new Item("No Ring 2", 0, 0, 0),
        new Item("Damage +1", 25, 1, 0),
        new Item("Damage +2", 50, 2, 0),
        new Item("Damage +3", 100, 3, 0),
        new Item("Defense +1", 20, 0, 1),
        new Item("Defense +2", 40, 0, 2),
        new Item("Defense +3", 80, 0, 3),
    };
    
    protected override TextParser<Player> Parser =>
        from health in Span.EqualTo("Hit Points: ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo('\n'))
        from damage in Span.EqualTo("Damage: ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo('\n'))
        from armor in Span.EqualTo("Armor: ").IgnoreThen(Numerics.IntegerInt32)
        select new Player(health, damage, armor);
        
    protected override int Part1(Player input)
    {
        var player = new Player(100, 0, 0);
        var loadOuts = CalculateAllLoadOuts(player);
        var p2 = new PlayerWithItems(input, Array.Empty<Item>());

        return loadOuts.Where(p1 => Play(p1, p2)).Min(x => x.Cost);
    }
    
    protected override int Part2(Player input)
    {
        var player = new Player(100, 0, 0);
        var loadOuts = CalculateAllLoadOuts(player);
        var p2 = new PlayerWithItems(input, Array.Empty<Item>());

        return loadOuts.Where(p1 => !Play(p1, p2)).Max(x => x.Cost);
    }
    
    private static IEnumerable<PlayerWithItems> CalculateAllLoadOuts(Player player)
    {
        var weaponArmor = Product.Get(Weapons, Armor, (w, a) => new[] { w, a }).ToList();
        var rings = Product.Get(Rings, Rings, (a, b) => new[] { a, b }).Where(x => x[0] != x[1]).ToList();

        return Product.Get(weaponArmor, rings, (a, b) => new PlayerWithItems(player, a.Concat(b).ToList()));
    }

    private static bool Play(PlayerWithItems loadOut1, PlayerWithItems loadOut2)
    {
        var p1 = loadOut1.Player;
        var p2 = loadOut2.Player;
        
        while (true)
        {
            var attack1 = Math.Max(1, loadOut1.Damage - loadOut2.Armor);
            p2 = p2 with { Health = p2.Health - attack1 };
            
            if (p2.Health <= 0)
            {
                break;
            }
            
            var attack2 = Math.Max(1, loadOut2.Damage - loadOut1.Armor);
            p1 = p1 with { Health = p1.Health - attack2 };
            
            if (p1.Health <= 0)
            {
                break;
            }
        }

        return p1.Health > 0;
    }
    
    public record Player(int Health, int Damage, int Armor);
    public record Item(string Name, int Cost, int Damage, int Armor);

    private record PlayerWithItems(Player Player, IReadOnlyList<Item> Items)
    {
        public int Damage => Player.Damage + Items.Sum(x => x.Damage);
        public int Armor => Player.Armor + Items.Sum(x => x.Armor);
        public int Cost => Items.Sum(x => x.Cost);
    }
}