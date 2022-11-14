using System.Diagnostics;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2015;

[Day]
public partial class Day22 : ParseDay<Day22.Boss, int, int>
{
    protected override TextParser<Boss> Parser =>
        from health in Span.EqualTo("Hit Points: ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo('\n'))
        from damage in Span.EqualTo("Damage: ").IgnoreThen(Numerics.IntegerInt32)
        select new Boss(health, damage);
        
    protected override int Part1(Boss input) => Solve(input, false);

    protected override int Part2(Boss input) => Solve(input, true);

    private static int Solve(Boss input, bool hardMode)
    {
        var player = new Player(50, 500, 0);
        var initial = new Game(hardMode, player, input, State.PlayerTurn, new Effects(0, 0, 0));

        return OptimisedSearch.Solve(initial, 0, new Search()).Player.ManaSpend;
    }

    private class Search : OptimisedSearch.Search<Game, int>
    {
        public override IEnumerable<Game> Next(Game item) => PlayAllTurn(item);
        
        public override int GetPriority(Game item) => item.Player.ManaSpend;

        public override bool IsGoal(Game item) => item.State == State.PlayerWon;
        public override bool ShouldSkip(Game item) => item.State == State.BossWon;
    }

    private static IEnumerable<Game> PlayAllTurn(Game initial)
    {
        Debug.Assert(initial.State == State.PlayerTurn);
        
        foreach (var spell in Enum.GetValues<Spell>())
        {
            var game = PlayTurn(initial, spell);
            if (game.State == State.BossTurn)
            {
                game = PlayTurn(game, spell);
            }
            yield return game;
        }
    }

    private static Game PlayTurn(Game game, Spell chosenSpell)
    {
        switch (game.State)
        {
            case State.PlayerTurn:
                if (game.HardMode)
                {
                    game = game with { Player = game.Player.Hit(1) };
                    if (game.Player.Health <= 0)
                    {
                        return game with { State = State.BossWon };
                    }
                }
                
                game = ApplyEffects(game);
                if (game.Boss.Health <= 0)
                {
                    return game with { State = State.PlayerWon };
                }
                
                switch (chosenSpell)
                {
                    case Spell.MagicMissile:
                        game = game with { Player = game.Player.SpendMana(53) };
                        if (game.Player.CurrentMana < 0)
                        {
                            return game with { State = State.BossWon };
                        }
                        
                        game = game with { Boss = game.Boss.Hit(4) };
                        if (game.Boss.Health <= 0)
                        {
                            return game with { State = State.PlayerWon };
                        }
                        
                        return game with { State = State.BossTurn };

                    case Spell.Drain:
                        game = game with { Player = game.Player.SpendMana(73) };
                        if (game.Player.CurrentMana < 0)
                        {
                            return game with { State = State.BossWon };
                        }
                        
                        game = game with
                        {
                            Boss = game.Boss.Hit(2),
                            Player = game.Player.Heal(2),
                        };
                        if (game.Boss.Health <= 0)
                        {
                            return game with { State = State.PlayerWon };
                        }
                        
                        return game with { State = State.BossTurn };
                    
                    case Spell.Shield:
                        game = game with { Player = game.Player.SpendMana(113) };
                        if (game.Player.CurrentMana < 0)
                        {
                            return game with { State = State.BossWon };
                        }

                        if (game.Effects.Shield > 0)
                        {
                            return game with { State = State.BossWon };
                        }
                        return game with
                        {
                            Effects = game.Effects with { Shield = 6 },
                            State = State.BossTurn
                        };
                        
                    case Spell.Poison:
                        game = game with { Player = game.Player.SpendMana(173) };
                        if (game.Player.CurrentMana < 0)
                        {
                            return game with { State = State.BossWon };
                        }

                        if (game.Effects.Poison > 0)
                        {
                            return game with { State = State.BossWon };
                        }
                        return game with
                        {
                            Effects = game.Effects with { Poison = 6 },
                            State = State.BossTurn
                        };
                    
                    case Spell.Recharge:
                        game = game with { Player = game.Player.SpendMana(229) };
                        if (game.Player.CurrentMana < 0)
                        {
                            return game with { State = State.BossWon };
                        }

                        if (game.Effects.Recharge > 0)
                        {
                            return game with { State = State.BossWon };
                        }
                        return game with
                        {
                            Effects = game.Effects with { Recharge = 5 },
                            State = State.BossTurn
                        };
                   
                    default: throw new ArgumentOutOfRangeException(nameof(chosenSpell), chosenSpell, null);
                }
                
            case State.BossTurn:
                game = ApplyEffects(game);
                if (game.Boss.Health <= 0)
                {
                    return game with { State = State.PlayerWon };
                }

                var armor = game.Effects.Shield > 0 ? 7 : 0;
                game = game with { Player = game.Player.Hit(game.Boss.Damage - armor) };

                if (game.Player.Health <= 0)
                {
                    return game with { State = State.BossWon };
                }
                
                return game with { State = State.PlayerTurn };
            
            case State.PlayerWon:
            case State.BossWon:
                return game;
            
            default: throw new ArgumentOutOfRangeException(nameof(game), game.State, null);
        }
    }

    private static Game ApplyEffects(Game game)
    {
        var effects = game.Effects;
        
        if (effects.Shield > 0)
        {
            effects = effects with { Shield = effects.Shield - 1 };
        }
        
        if (effects.Poison > 0)
        {
            game = game with { Boss = game.Boss.Hit(3) };
            effects = effects with { Poison = effects.Poison - 1 };
        }
        
        if (effects.Recharge > 0)
        {
            game = game with { Player = game.Player.GainMana(101) };
            effects = effects with { Recharge = effects.Recharge - 1 };
        }

        return game with { Effects = effects };
    }

    public record Boss(int Health, int Damage)
    {
        public Boss Hit(int amount) => this with { Health = Health - Math.Max(1, amount) };
    }

    private record Player(int Health, int CurrentMana, int ManaSpend)
    {
        public Player Heal(int amount) => this with { Health = Health + amount };
        public Player Hit(int amount) => this with { Health = Health - Math.Max(1, amount) };

        public Player GainMana(int amount) => this with { CurrentMana = CurrentMana + amount };
        public Player SpendMana(int amount) => this with { CurrentMana = CurrentMana - amount, ManaSpend = ManaSpend + amount };
    }
    private record Effects(int Shield, int Poison, int Recharge);
    private record Game(bool HardMode, Player Player, Boss Boss, State State, Effects Effects);

    private enum Spell
    {
        MagicMissile,
        Drain,
        Shield,
        Poison,
        Recharge,
    }
    
    private enum State
    {
        PlayerTurn,
        BossTurn,
        PlayerWon,
        BossWon,
    }
}