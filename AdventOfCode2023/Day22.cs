﻿namespace AdventOfCode2023;

[Day]
public partial class Day22 : LineDay<Day22.Brick, int, int>
{
    protected override Brick ParseLine(string input)
    {
        var parts = input.Split('~')
            .Select(part => part.Split(',').Select(int.Parse).ToArray())
            .Select(part => new Position3(part[0], part[1], part[2]))
            .ToArray();

        return new Brick(parts[0], parts[1]);
    }

    [Sample("1,0,1~1,2,1\n0,0,2~2,0,2\n0,2,3~2,2,3\n0,0,4~0,2,4\n2,0,5~2,2,5\n0,1,6~2,1,6\n1,1,8~1,1,9", 5)]
    protected override int Part1(IEnumerable<Brick> input)
    {
        var model = SimulateGravity(input);
        
        var removable = 0;
        foreach (var brick in model.Bricks.Keys)
        {
            var canRemove = true;
            foreach (var supportedBrick in model.Supporting[brick])
            {
                if (model.SupportedBy[supportedBrick].Count == 1) canRemove = false;
            }

            if (canRemove) removable++;
        }

        return removable;
    }

    [Sample("1,0,1~1,2,1\n0,0,2~2,0,2\n0,2,3~2,2,3\n0,0,4~0,2,4\n2,0,5~2,2,5\n0,1,6~2,1,6\n1,1,8~1,1,9", 7)]
    protected override int Part2(IEnumerable<Brick> input)
    {
        var model = SimulateGravity(input); 
        
        var removable = 0;
        foreach (var brick in model.Bricks.Keys)
        {
            var fall = new HashSet<int>();
            var search = new Queue<int>();
            search.Enqueue(brick);

            while (search.Count > 0)
            {
                var remove = search.Dequeue();
                fall.Add(remove);
                
                foreach (var above in model.Supporting[remove])
                {
                    if (model.SupportedBy[above].All(x => fall.Contains(x)))
                    {
                        search.Enqueue(above);
                    }
                }
            }

            if (fall.Count > 1)
            {
                removable += fall.Count - 1;
            }
        }

        return removable;
    }

    private static Model SimulateGravity(IEnumerable<Brick> input)
    {
        var placed = new Dictionary<Brick, int>();
        var occupied = new Dictionary<Position3, int>();

        var down = new Position3(0, 0, -1);

        foreach (var brick in input.OrderBy(brick => Math.Min(brick.A.Z, brick.B.Z)).ToList())
        {
            var current = brick;
            
            while (true)
            {
                if (Math.Min(current.A.Z, current.B.Z) == 1) break;
                var downBrick = new Brick(current.A + down, current.B + down);
                if (downBrick.Occupied.All(p => !occupied.ContainsKey(p)))
                {
                    current = downBrick;
                }
                else
                {
                    break;
                }
            }

            var id = placed.Count;
            placed.Add(current, id);
            foreach (var position in current.Occupied)
            {
                occupied.Add(position, id);
            }
        }
        
        var supporting = new Dictionary<int, IReadOnlyCollection<int>>();
        
        foreach (var (brick, id) in placed)
        {
            var above = new HashSet<int>();
            foreach (var position in brick.Occupied)
            {
                if (occupied.TryGetValue(position - down, out var brickAbove) && brickAbove != id)
                {
                    above.Add(brickAbove);
                }
            }

            supporting[id] = above;
        }

        var supportedBy = supporting
            .SelectMany(x => x.Value.Select(y => (Below: x.Key, Above: y)))
            .GroupBy(x => x.Above)
            .ToDictionary(x => x.Key, x => (IReadOnlyCollection<int>) x.Select(y => y.Below).ToHashSet());
        
        var idToBrick = placed.ToDictionary(x => x.Value, x => x.Key);
        
        return new Model(idToBrick, supporting, supportedBy);
    }
    
    private record Model(
        IReadOnlyDictionary<int, Brick> Bricks,
        IReadOnlyDictionary<int, IReadOnlyCollection<int>> Supporting,
        IReadOnlyDictionary<int, IReadOnlyCollection<int>> SupportedBy);
    
    public record Brick(Position3 A, Position3 B)
    {
        public IEnumerable<Position3> Occupied
        {
            get
            {
                Position3 start, end, delta;
                
                if (A.X != B.X)
                {
                    delta = new Position3(1, 0, 0);
                    start = new Position3(Math.Min(A.X, B.X), A.Y, A.Z);
                    end = new Position3(Math.Max(A.X, B.X), A.Y, A.Z);
                }
                else if(A.Y != B.Y)
                {
                    delta = new Position3(0, 1, 0);
                    start = new Position3(A.X, Math.Min(A.Y, B.Y), A.Z);
                    end = new Position3(A.X, Math.Max(A.Y, B.Y), A.Z);
                }
                else if(A.Z != B.Z)
                {
                    delta = new Position3(0, 0, 1);
                    start = new Position3(A.X, A.Y, Math.Min(A.Z, B.Z));
                    end = new Position3(A.X, A.Y, Math.Max(A.Z, B.Z));
                }
                else
                {
                    delta = new Position3(0, 0, 0);
                    start = A;
                    end = A;
                }

                var p = start - delta;
                do
                {
                    p += delta;
                    yield return p;
                } while (!Equals(p, end));
            }
        }
    }
}