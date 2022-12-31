using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode2016;

[Day]
public partial class Day17 : Day<Day17.Model, string, int>
{
    protected override Model Parse(string input) => new(input);

    [Sample("ihgpwlah", "DDRRRD")]
    [Sample("kglvqrro", "DDUDRLRRUDRD")]
    [Sample("ulqzkmiv", "DRURDRUDDLLDLUURRDULRLDUUDDDRR")]
    protected override string Part1(Model input)
    {
        var md5 = MD5.Create();

        var start = new Position(0, 0);
        var target = new Position(3, 3);

        return OptimisedSearch.Solve((Position: start, Path: ""), x => x.Position == target, x => Next(md5, input, x.Path, x.Position), _ => false, x => x, x => x.Path.Length).Path;
    }
    
    private static IEnumerable<(Position, string)> Next(MD5 md5, Model input, string path, Position current)
    {
        var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(input.Key + path));

        if (current.Y > 0 && (hash[0] >> 4) > 10) yield return (current + new Position(0, -1), path + "U");
        if (current.Y < 3 && (hash[0] & 0xf) > 10) yield return (current + new Position(0, 1), path + "D");
        
        if (current.X > 0 && (hash[1] >> 4) > 10) yield return (current + new Position(-1, 0), path + "L");
        if (current.X < 3 && (hash[1] & 0xf) > 10) yield return (current + new Position(1, 0), path + "R");
    }

    [Sample("ihgpwlah", 370)]
    [Sample("kglvqrro", 492)]
    [Sample("ulqzkmiv", 830)]
    protected override int Part2(Model input)
    {
        var md5 = MD5.Create();

        var start = new Position(0, 0);
        var target = new Position(3, 3);

        var search = new Queue<(Position, string)>();
        search.Enqueue((start, ""));
        
        var solutions = new List<string>();
        
        while (search.Count > 0)
        {
            var (current, path) = search.Dequeue();
            if (current == target)
            {
                solutions.Add(path);
                continue;
            }

            foreach (var next in Next(md5, input, path, current))
            {
                search.Enqueue(next);
            }
        }
        
        return solutions.Max(x => x.Length);
    }

    public record Model(string Key);
}
