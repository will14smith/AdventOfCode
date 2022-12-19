using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2022;

[Day]
public partial class Day19 : ParseLineDay<Day19.Blueprint, int, int>
{
    private const string Sample = "Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.\nBlueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.";

    protected override TextParser<Blueprint> LineParser =>
        from id in Span.EqualTo("Blueprint ").IgnoreThen(Numerics.IntegerInt32)
        from ore in Span.EqualTo(": Each ore robot costs ").IgnoreThen(Numerics.IntegerInt32).Select(x => new Resources(x, 0, 0, 0))
        from clay in Span.EqualTo(" ore. Each clay robot costs ").IgnoreThen(Numerics.IntegerInt32).Select(x => new Resources(x, 0, 0, 0))
        from obsidian in Span.EqualTo(" ore. Each obsidian robot costs ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" ore and ")).Then(Numerics.IntegerInt32).Select(x => new Resources(x.Item1, x.Item2, 0, 0))
        from geode in Span.EqualTo(" clay. Each geode robot costs ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" ore and ")).Then(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" obsidian.")).Select(x => new Resources(x.Item1, 0, x.Item2, 0))
        select Blueprint.Create(id, ore, clay, obsidian, geode);

    [Sample(Sample, 33)]
    protected override int Part1(IEnumerable<Blueprint> input)
    {
        return input.AsParallel().Select(blueprint => FindMostGeodes(State.New(blueprint))).Sum(x => x.Blueprint.Id * x.Current.Geode);
    }
    
    [Sample(Sample, 3472)]
    protected override int Part2(IEnumerable<Blueprint> input)
    {
        return input.Take(3).AsParallel().Select(blueprint => FindMostGeodes(State.New(blueprint), 32)).Aggregate(1, (acc, state) => acc * state.Current.Geode);
    }

    private static State FindMostGeodes(State state, int time = 24)
    {
        var best = state;

        var couldBuyAnyPrevious = false;
        
        foreach (var robotType in Resources.List)
        {
            var timeUntilNextRobot = TimeUntilRobotCanBeBuilt(state, robotType);
            var shouldBuildRobot = ShouldBuildRobot(state, robotType);

            if (!couldBuyAnyPrevious && robotType == Resource.Geode)
            {
                while (time > 0)
                {
                    timeUntilNextRobot = TimeUntilRobotCanBeBuilt(state, robotType)!;

                    if (timeUntilNextRobot.Value > time)
                    {
                        break;
                    }
                    
                    state = BuyRobot(SimulateTime(state, timeUntilNextRobot.Value + 1), robotType);
                    time -= timeUntilNextRobot.Value + 1;
                }

                return SimulateTime(state, time);
            }
            
            if (timeUntilNextRobot.HasValue && shouldBuildRobot)
            {
                couldBuyAnyPrevious = true;
                
                if (timeUntilNextRobot.Value < time)
                {
                    var candidate = FindMostGeodes(BuyRobot(SimulateTime(state, timeUntilNextRobot.Value + 1), robotType), time - timeUntilNextRobot.Value - 1);
                    if (candidate.Current.Geode > best.Current.Geode)
                    {
                        best = candidate;
                    }
                }
            }
        }

        return best;
    }
    
    private static int? TimeUntilRobotCanBeBuilt(State state, Resource robotType)
    {
        var cost = state.Blueprint.Get(robotType);

        var time = 0;
        
        foreach (var resource in Resources.List)
        {
            var resourceTime = TimeUntilResourceReady(state.Current, cost, state.Robots, resource);
            if (resourceTime == null) return null;
            
            time = Math.Max(time, resourceTime.Value);
        }
        
        return time;
    }

    private static int? TimeUntilResourceReady(Resources current, Resources cost, Resources production, Resource resource)
    {
        var resourceCost = cost.Get(resource);
        if (resourceCost == 0) return 0;

        var resourceProduction = production.Get(resource);
        if (resourceProduction == 0) return null;

        var resourceCurrent = current.Get(resource);
        var time = (resourceCost - resourceCurrent + resourceProduction - 1) / resourceProduction;
        return time < 0 ? 0 : time;
    }

    private static bool ShouldBuildRobot(State state, Resource resource) => state.Robots.Get(resource) < state.Blueprint.MaxRobots.Get(resource);

    private static State SimulateTime(State state, int time)
    {
        return state with { Current = state.Current + time * state.Robots };
    }
    
    private static State BuyRobot(State state, Resource robotType)
    {
        var cost = state.Blueprint.Get(robotType);
        var robotDelta = robotType switch
        {
            Resource.Ore => Resources.Empty with { Ore = 1 },
            Resource.Clay => Resources.Empty with { Clay = 1 },
            Resource.Obsidian => Resources.Empty with { Obsidian = 1 },
            Resource.Geode => Resources.Empty with { Geode = 1 },
        };
        
        return state with { Current = state.Current - cost, Robots = state.Robots + robotDelta };
    }
    
    public record Blueprint(int Id, Resources Ore, Resources Clay, Resources Obsidian, Resources Geode)
    {
        public Resources MaxRobots { get; private set; }
        
        public static Blueprint Create(int id, Resources ore, Resources clay, Resources obsidian, Resources geode)
        {
            var blueprint = new Blueprint(id, ore, clay, obsidian, geode);

            blueprint.MaxRobots = new Resources(
                Math.Max(Math.Max(blueprint.Ore.Ore, blueprint.Clay.Ore), Math.Max(blueprint.Obsidian.Ore, blueprint.Geode.Ore)),
                Math.Max(Math.Max(blueprint.Ore.Clay, blueprint.Clay.Clay), Math.Max(blueprint.Obsidian.Clay, blueprint.Geode.Clay)),
                Math.Max(Math.Max(blueprint.Ore.Obsidian, blueprint.Clay.Obsidian), Math.Max(blueprint.Obsidian.Obsidian, blueprint.Geode.Obsidian)),
                10_000
            );

            return blueprint;
        }
        
        public Resources Get(Resource type) => type switch
        {
            Resource.Ore => Ore,
            Resource.Clay => Clay,
            Resource.Obsidian => Obsidian,
            Resource.Geode => Geode,
        };

    }

    public record Resources(int Ore, int Clay, int Obsidian, int Geode)
    {
        public static readonly Resources Empty = new(0, 0, 0, 0);
        public static readonly Resource[] List = { Resource.Ore, Resource.Clay, Resource.Obsidian, Resource.Geode };

        public static Resources operator +(Resources a, Resources b) => new(a.Ore + b.Ore, a.Clay + b.Clay, a.Obsidian + b.Obsidian, a.Geode + b.Geode);
        public static Resources operator -(Resources a, Resources b) => new(a.Ore - b.Ore, a.Clay - b.Clay, a.Obsidian - b.Obsidian, a.Geode - b.Geode);
        public static Resources operator *(int a, Resources b) => new(a * b.Ore, a * b.Clay, a * b.Obsidian, a * b.Geode);

        public static bool operator >(Resources a, Resources b) => a.Ore > b.Ore || a.Clay > b.Clay || a.Obsidian > b.Obsidian || a.Geode > b.Geode;
        public static bool operator <(Resources a, Resources b) => a.Ore < b.Ore || a.Clay < b.Clay || a.Obsidian < b.Obsidian || a.Geode < b.Geode;

        public int Get(Resource resource) => resource switch
        {
            Resource.Ore => Ore,
            Resource.Clay => Clay,
            Resource.Obsidian => Obsidian,
            Resource.Geode => Geode,
        };
    }
    public enum Resource
    {
        Ore,
        Clay,
        Obsidian,
        Geode,
    }

    public record State(Blueprint Blueprint, Resources Current, Resources Robots)
    {
        public static State New(Blueprint blueprint) => new(blueprint, Resources.Empty, Resources.Empty with { Ore = 1 });
    }
}
