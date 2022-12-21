using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2022;

[Day]
public partial class Day19 : ParseLineDay<Day19.Blueprint, int, int>
{
    private const string Sample = "Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.\nBlueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.";

    protected override TextParser<Blueprint> LineParser =>
        from id in Span.EqualTo("Blueprint ").IgnoreThen(Numerics.IntegerInt32)
        from ore in Span.EqualTo(": Each ore robot costs ").IgnoreThen(Numerics.IntegerInt32).Select(x => x * Resources.Ore)
        from clay in Span.EqualTo(" ore. Each clay robot costs ").IgnoreThen(Numerics.IntegerInt32).Select(x => x * Resources.Ore)
        from obsidian in Span.EqualTo(" ore. Each obsidian robot costs ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" ore and ")).Then(Numerics.IntegerInt32).Select(x => x.Item1 * Resources.Ore + x.Item2 * Resources.Clay)
        from geode in Span.EqualTo(" clay. Each geode robot costs ").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" ore and ")).Then(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" obsidian.")).Select(x => x.Item1 * Resources.Ore + x.Item2 * Resources.Obsidian)
        select Blueprint.Create(id, ore, clay, obsidian, geode);

    [Sample(Sample, 33)]
    protected override int Part1(IEnumerable<Blueprint> input)
    {
        return input.AsParallel().Select(blueprint => blueprint.Id * FindMostGeodes(State.New(blueprint))).Sum();
    }
    
    [Sample(Sample, 3472)]
    protected override int Part2(IEnumerable<Blueprint> input)
    {
        return input.Take(3).AsParallel().Select(blueprint => FindMostGeodes(State.New(blueprint), 32)).Aggregate(1, (acc, geodes) => acc * geodes);
    }

    private static int FindMostGeodes(State state, int time = 24)
    {
        var best = state.Current.Get(Resource.Geode);
        
        var shouldBuildRobot = Sse2.CompareLessThan(state.Robots.Values, state.Blueprint.MaxRobots.Values);

        if(state.Robots.Values[(int)Resource.Obsidian] > 0)
        {
            if (shouldBuildRobot[(int)Resource.Geode] != 0)
            {
                var timeUntilNextRobot = TimeUntilRobotCanBeBuilt(state, Resource.Geode);

                if (timeUntilNextRobot < time)
                {
                    var candidate = FindMostGeodes(SimulateTimeThenBuyRobot(state, timeUntilNextRobot.Value, Resource.Geode), time - timeUntilNextRobot.Value);
                    best = Math.Max(best, candidate);
                }

                best = Math.Max(best, state.Current.Get(Resource.Geode) + state.Robots.Get(Resource.Geode) * time);
                
                if (timeUntilNextRobot == 1)
                {
                    // at the point where 1 geode robot is produced per minute
                    return best;
                }
            }
        }

        if (shouldBuildRobot[(int)Resource.Ore] != 0)
        {
            var timeUntilNextRobot = TimeUntilRobotCanBeBuilt(state, Resource.Ore);

            if (timeUntilNextRobot < time)
            {
                var candidate = FindMostGeodes(SimulateTimeThenBuyRobot(state, timeUntilNextRobot.Value, Resource.Ore), time - timeUntilNextRobot.Value);
                best = Math.Max(best, candidate);
            }
        }

        if (shouldBuildRobot[(int)Resource.Clay] != 0)
        {
            var timeUntilNextRobot = TimeUntilRobotCanBeBuilt(state, Resource.Clay);

            if (timeUntilNextRobot < time)
            {
                var candidate = FindMostGeodes(SimulateTimeThenBuyRobot(state, timeUntilNextRobot.Value, Resource.Clay), time - timeUntilNextRobot.Value);
                best = Math.Max(best, candidate);
            }
        }

        if(state.Robots.Values[(int)Resource.Clay] > 0)
        {
            if (shouldBuildRobot[(int)Resource.Obsidian] != 0)
            {
                var timeUntilNextRobot = TimeUntilRobotCanBeBuilt(state, Resource.Obsidian);

                if (timeUntilNextRobot < time)
                {
                    var candidate = FindMostGeodes(SimulateTimeThenBuyRobot(state, timeUntilNextRobot.Value, Resource.Obsidian), time - timeUntilNextRobot.Value);
                    best = Math.Max(best, candidate);
                }
            }
        }
        
        return best;
    }
    
    private static int? TimeUntilRobotCanBeBuilt(State state, Resource robotType)
    {
        var current = state.Current.Values;
        var cost = state.Blueprint.Get(robotType).Values;
        var production = state.Robots.Values;
        
        var required = cost - current + production - Vector128.Create(1, 1, 1, 1);
        var requiredMask = Sse2.CompareGreaterThan(required, Vector128<int>.Zero);
        required = Sse2.And(required, requiredMask);

        var productionMask = Sse2.CompareEqual(production, Vector128<int>.Zero);
        var missingProduction = Sse2.And(requiredMask, productionMask);
        if (Sse2.MoveMask(missingProduction.AsByte()) != 0) return null;

        var time = required / (production + productionMask);
        // plus 1 since it takes a minute to build
        return Math.Max(Math.Max(time[0], time[1]), Math.Max(time[2], time[3])) + 1;
    }
    
    private static State SimulateTimeThenBuyRobot(State state, int time, Resource robotType)
    {
        var cost = state.Blueprint.Get(robotType);
        var robotDelta = robotType switch
        {
            Resource.Ore => Resources.Ore,
            Resource.Clay => Resources.Clay,
            Resource.Obsidian => Resources.Obsidian,
            Resource.Geode => Resources.Geode,
        };

        return state with { Current = state.Current + time * state.Robots - cost, Robots = state.Robots + robotDelta };
    }
}
