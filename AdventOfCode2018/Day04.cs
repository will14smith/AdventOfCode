using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2018;

[Day]
public partial class Day04 : ParseLineDay<Day04.Event, int, int>
{
    public enum EventAction
    {
        BeginsShift,
        FallsAsleep,
        WakesUp,
    }
    public record Event(DateTime Time, EventAction Action, int? GuardId);

    private static TextParser<DateTime> TimeParser = 
        from year in Character.EqualTo('[').IgnoreThen(Numerics.IntegerInt32)
        from month in Character.EqualTo('-').IgnoreThen(Numerics.IntegerInt32)
        from day in Character.EqualTo('-').IgnoreThen(Numerics.IntegerInt32)
        from hour in Character.EqualTo(' ').IgnoreThen(Numerics.IntegerInt32)
        from minute in Character.EqualTo(':').IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Character.EqualTo(']'))
        select new DateTime(year, month, day, hour, minute, 0);
    
    private static TextParser<(EventAction Action, int? GuardId)> BeginsShiftParser = Span.EqualTo("Guard #").IgnoreThen(Numerics.IntegerInt32).ThenIgnore(Span.EqualTo(" begins shift")).Select(guardId => (EventAction.BeginsShift, (int?)guardId));
    private static TextParser<(EventAction Action, int? GuardId)> FallsAsleepParser = Span.EqualTo("falls asleep").Select(_ => (EventAction.FallsAsleep, (int?)null));
    private static TextParser<(EventAction Action, int? GuardId)> WakesUpParser = Span.EqualTo("wakes up").Select(_ => (EventAction.WakesUp, (int?)null));
    private static TextParser<(EventAction Action, int? GuardId)> ActionParser = BeginsShiftParser.Or(FallsAsleepParser).Or(WakesUpParser);
    
    protected override TextParser<Event> LineParser { get; } = 
        from time in TimeParser
        from action in Character.EqualTo(' ').IgnoreThen(ActionParser)
        select new Event(time, action.Action, action.GuardId);
    
    [Sample("[1518-11-01 00:00] Guard #10 begins shift\n[1518-11-01 00:05] falls asleep\n[1518-11-01 00:25] wakes up\n[1518-11-01 00:30] falls asleep\n[1518-11-01 00:55] wakes up\n[1518-11-01 23:58] Guard #99 begins shift\n[1518-11-02 00:40] falls asleep\n[1518-11-02 00:50] wakes up\n[1518-11-03 00:05] Guard #10 begins shift\n[1518-11-03 00:24] falls asleep\n[1518-11-03 00:29] wakes up\n[1518-11-04 00:02] Guard #99 begins shift\n[1518-11-04 00:36] falls asleep\n[1518-11-04 00:46] wakes up\n[1518-11-05 00:03] Guard #99 begins shift\n[1518-11-05 00:45] falls asleep\n[1518-11-05 00:55] wakes up\n", 240)]
    protected override int Part1(IEnumerable<Event> input)
    {
        var guardSleepMinutes = GroupEventsByGuardMinutes(input);

        var sleepiestGuard = guardSleepMinutes.MaxBy(kv => kv.Value.Count);
        var mostCommonMinute = sleepiestGuard.Value.GroupBy(x => x).MaxBy(g => g.Count());

        return sleepiestGuard.Key * mostCommonMinute.Key;
    }
    
    [Sample("[1518-11-01 00:00] Guard #10 begins shift\n[1518-11-01 00:05] falls asleep\n[1518-11-01 00:25] wakes up\n[1518-11-01 00:30] falls asleep\n[1518-11-01 00:55] wakes up\n[1518-11-01 23:58] Guard #99 begins shift\n[1518-11-02 00:40] falls asleep\n[1518-11-02 00:50] wakes up\n[1518-11-03 00:05] Guard #10 begins shift\n[1518-11-03 00:24] falls asleep\n[1518-11-03 00:29] wakes up\n[1518-11-04 00:02] Guard #99 begins shift\n[1518-11-04 00:36] falls asleep\n[1518-11-04 00:46] wakes up\n[1518-11-05 00:03] Guard #99 begins shift\n[1518-11-05 00:45] falls asleep\n[1518-11-05 00:55] wakes up\n", 4455)]
    protected override int Part2(IEnumerable<Event> input)
    {
        var guardSleepMinutes = GroupEventsByGuardMinutes(input);
        
        var guardMostFrequentMinute = guardSleepMinutes
            .Select(kv => new
            {
                GuardId = kv.Key,
                MostFrequentMinute = kv.Value
                    .GroupBy(x => x)
                    .Select(g => new { Minute = g.Key, Count = g.Count() })
                    .MaxBy(g => g.Count)
            })
            .MaxBy(x => x.MostFrequentMinute.Count);

        return guardMostFrequentMinute.GuardId * guardMostFrequentMinute.MostFrequentMinute.Minute;
    }
    
    private static Dictionary<int, List<int>> GroupEventsByGuardMinutes(IEnumerable<Event> input)
    {
        var currentGuardId = -1;
        var currentSleepStart = DateTime.MinValue;
        var guardSleepMinutes = new Dictionary<int, List<int>>();
        
        foreach (var @event in input.OrderBy(x => x.Time))
        {
            switch (@event.Action)
            {
                case EventAction.BeginsShift:
                    currentGuardId = @event.GuardId!.Value;
                    break;
                
                case EventAction.FallsAsleep:
                    currentSleepStart = @event.Time;
                    break;
                
                case EventAction.WakesUp:
                    if (!guardSleepMinutes.ContainsKey(currentGuardId))
                    {
                        guardSleepMinutes[currentGuardId] = [];
                    }
                    for (var time = currentSleepStart; time < @event.Time; time = time.AddMinutes(1))
                    {
                        guardSleepMinutes[currentGuardId].Add(time.Minute);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return guardSleepMinutes;
    }
}
