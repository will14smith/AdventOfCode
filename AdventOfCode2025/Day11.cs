namespace AdventOfCode2025;

[Day]
public partial class Day11 : Day<Day11.Model, long, long>
{
    public record Model(IReadOnlyList<(string Source, IReadOnlyList<string> Outputs)> Devices);

    protected override Model Parse(string input) => new(input.Split('\n', StringSplitOptions.RemoveEmptyEntries)
        .Select(line =>
        {
            var parts = line.Split(":");
            var source = parts[0];
            IReadOnlyList<string> outputs = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            return (source, outputs);
        })
        .ToList());

    [Sample("aaa: you hhh\nyou: bbb ccc\nbbb: ddd eee\nccc: ddd eee fff\nddd: ggg\neee: out\nfff: out\nggg: out\nhhh: ccc fff iii\niii: out\n", 5)]
    protected override long Part1(Model input) => CountPaths1(input, "you", "out", new Dictionary<string, long>());

    private static long CountPaths1(Model model, string current, string target, Dictionary<string, long> cache)
    {
        if (current == target)
        {
            return 1;
        }
        
        if (cache.TryGetValue(current, out var result))
        {
            return result;
        }

        var device = model.Devices.First(x => x.Source == current);
        return cache[current] = device.Outputs.Sum(output => CountPaths1(model, output, target, cache));
    }

    [Sample("svr: aaa bbb\naaa: fft\nfft: ccc\nbbb: tty\ntty: ccc\nccc: ddd eee\nddd: hub\nhub: fff\neee: dac\ndac: fff\nfff: ggg hhh\nggg: out\nhhh: out\n", 2)]
    protected override long Part2(Model input) => CountPaths2(input, "svr", "out", false, false, new Dictionary<(string Current, bool VisitedDac, bool VisitedFft), long>());
    
    private static long CountPaths2(Model model, string current, string target, bool visitedDac, bool visitedFft, Dictionary<(string Current, bool VisitedDac, bool VisitedFft), long> cache)
    {
        if (current == target)
        {
            return visitedDac && visitedFft ? 1 : 0;
        }

        var key = (current, visitedDac, visitedFft);
        if (cache.TryGetValue(key, out var result))
        {
            return result;
        }

        visitedDac |= current == "dac";
        visitedFft |= current == "fft";
        
        var device = model.Devices.First(x => x.Source == current);
        return cache[key] = device.Outputs.Sum(output => CountPaths2(model, output, target, visitedDac, visitedFft, cache));
    }
}