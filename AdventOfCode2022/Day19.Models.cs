using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace AdventOfCode2022;

public partial class Day19
{
    public record Blueprint(int Id, Resources[] Robots)
    {
        public Resources MaxRobots { get; private set; }

        public static Blueprint Create(int id, Resources ore, Resources clay, Resources obsidian, Resources geode)
        {
            var blueprint = new Blueprint(id, new[] { ore, clay, obsidian, geode });

            blueprint.MaxRobots = new Resources(Vector128.Create(
                blueprint.Robots.Max(x => x.Get(Resource.Ore)),
                blueprint.Robots.Max(x => x.Get(Resource.Clay)),
                blueprint.Robots.Max(x => x.Get(Resource.Obsidian)),
                100_000
            ));

            return blueprint;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Resources Get(Resource type) => Robots[(int)type];
    }

    public readonly struct Resources
    {
        public static readonly Resources Empty = new(Vector128<int>.Zero);
        public static readonly Resources Ore = new(Vector128.Create(1, 0, 0, 0));
        public static readonly Resources Clay = new(Vector128.Create(0, 1, 0, 0));
        public static readonly Resources Obsidian = new(Vector128.Create(0, 0, 1, 0));
        public static readonly Resources Geode = new(Vector128.Create(0, 0, 0, 1));

        public readonly Vector128<int> Values;
        public Resources(Vector128<int> values)
        {
            Values = values;
        }

        public static Resources operator +(Resources a, Resources b) =>
            new(Sse2.Add(a.Values, b.Values));

        public static Resources operator -(Resources a, Resources b) =>
            new(Sse2.Subtract(a.Values, b.Values));

        public static Resources operator *(int a, Resources b) => new(a * b.Values);

        public static bool operator >(Resources a, Resources b) =>
            Sse2.MoveMask(Sse2.CompareGreaterThan(a.Values, b.Values).AsByte()) != 0;

        public static bool operator <(Resources a, Resources b) =>
            Sse2.MoveMask(Sse2.CompareLessThan(a.Values, b.Values).AsByte()) != 0;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Get(Resource resource) => Values[(int)resource];
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
        public static State New(Blueprint blueprint) =>
            new(blueprint, Resources.Empty, Resources.Ore);
    }
}