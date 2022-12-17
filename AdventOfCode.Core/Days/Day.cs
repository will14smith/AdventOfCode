using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AdventOfCode.Core;

public abstract class Day
{
    protected static readonly Regex NameMatch = new(".*Day(\\d+)", RegexOptions.Compiled);
}

public abstract class Day<TModel, TResult1, TResult2> : Day
{
    private int DayNumber { get; }
    private string FileName => $"Inputs/day{DayNumber}";
    private string InputString => File.ReadAllText(FileName).ReplaceLineEndings("\n");
    
    protected readonly ITestOutputHelper Output;

    protected virtual IEnumerable<(string, TResult1)> Tests1 { get; } = Enumerable.Empty<(string, TResult1)>();
    protected virtual IEnumerable<(string, TResult2)> Tests2 { get; } = Enumerable.Empty<(string, TResult2)>();
    
    protected Day(int dayNumber, ITestOutputHelper output)
    {
        DayNumber = dayNumber;
        Output = output;
    }

    [Fact]
    public void TestPart1()
    {
        var i = 0;
        foreach (var (input, expected) in Tests1)
        {
            var actual = Run($"Test.{i++} Day{DayNumber}.1", input, Part1);
            actual.Should().Be(expected, $"{input}");
        }
    }

    [Fact]
    public void TestPart2()
    {
        var i = 0;
        foreach (var (input, expected) in Tests2)
        {
            var actual = Run($"Test.{i++} Day{DayNumber}.2", input, Part2);
            actual.Should().Be(expected, $"{input}");
        }
    }

    [Fact]
    public void ActualPart1() => Run($"Actual Day{DayNumber}.1", InputString, Part1);
    [Fact]
    public void ActualPart2() => Run($"Actual Day{DayNumber}.2", InputString, Part2);

    protected TResult Run<TResult>(string name, string input, Func<TModel, TResult> func)
    {
        Output.WriteLine($"[*] Calculating for {name}");
        
        var stopwatch = new Stopwatch();
        
        stopwatch.Restart();
        var data = Parse(input);
        stopwatch.Stop();
        Output.WriteLine($"[*] Parsed input in {Math.Round(stopwatch.Elapsed.TotalMilliseconds, 2)}ms");

        stopwatch.Restart();
        var result = func(data);
        stopwatch.Stop();
        
        Output.WriteLine($"[*] Result = {result}, in {Math.Round(stopwatch.Elapsed.TotalMilliseconds, 2)}ms");

        return result;
    }

    protected abstract TModel Parse(string input);

    protected abstract TResult1 Part1(TModel input);
    protected abstract TResult2 Part2(TModel input);
}