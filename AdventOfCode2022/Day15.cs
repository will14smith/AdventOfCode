using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2022;

[Day]
public partial class Day15 : ParseLineDay<Day15.Input, long, long>
{
    private const string Sample = "Sensor at x=2, y=18: closest beacon is at x=-2, y=15\nSensor at x=9, y=16: closest beacon is at x=10, y=16\nSensor at x=13, y=2: closest beacon is at x=15, y=3\nSensor at x=12, y=14: closest beacon is at x=10, y=16\nSensor at x=10, y=20: closest beacon is at x=10, y=16\nSensor at x=14, y=17: closest beacon is at x=10, y=16\nSensor at x=8, y=7: closest beacon is at x=2, y=10\nSensor at x=2, y=0: closest beacon is at x=2, y=10\nSensor at x=0, y=11: closest beacon is at x=2, y=10\nSensor at x=20, y=14: closest beacon is at x=25, y=17\nSensor at x=17, y=20: closest beacon is at x=21, y=22\nSensor at x=16, y=7: closest beacon is at x=15, y=3\nSensor at x=14, y=3: closest beacon is at x=15, y=3\nSensor at x=20, y=1: closest beacon is at x=15, y=3";

    protected override TextParser<Input> LineParser =>
        from sensorX in Span.EqualTo("Sensor at x=").IgnoreThen(Numerics.IntegerInt32)
        from sensorY in Span.EqualTo(", y=").IgnoreThen(Numerics.IntegerInt32)
        from beaconX in Span.EqualTo(": closest beacon is at x=").IgnoreThen(Numerics.IntegerInt32)
        from beaconY in Span.EqualTo(", y=").IgnoreThen(Numerics.IntegerInt32)
        select new Input(new Position(sensorX, sensorY), new Position(beaconX, beaconY));
    
    [Sample(Sample, 26L)]
    protected override long Part1(IEnumerable<Input> input)
    {
        var inputList = input.ToList();
        var targetRow = inputList.First().Sensor.X == 2 ? 10 : 2000000;

        var coverage = GetCoverage(inputList, targetRow);
        foreach (var item in inputList)
        {
            if (item.Beacon.Y == targetRow)
            {
                coverage.RemoveAt(item.Beacon.X);
            }
        }
        
        return coverage.Ranges.Sum(x => x.End - x.Start + 1);
    }
    
    [Sample(Sample, 56000011L)]
    protected override long Part2(IEnumerable<Input> input)
    {
        var inputList = input.ToList();
        var searchSize = inputList.First().Sensor.X == 2 ? 20 : 4000000;

        for (var y = 0; y <= searchSize; y++)
        {
            var coverage = GetCoverage(inputList, y);
            var range = coverage.Ranges.FirstOrDefault(r => r.Start > 0 && r.Start <= searchSize);
                
            if(range != null)
            {
                return (range.Start - 1) * 4000000L + y;
            }
        }
        
        throw new Exception("no solution");
    }

    private static Coverage GetCoverage(IEnumerable<Input> input, int y)
    {
        var coverage = new Coverage();
        
        foreach (var item in input)
        {
            var distance = (item.Sensor - item.Beacon).TaxiDistance();
            var width = Math.Max(0, distance - Math.Abs(item.Sensor.Y - y) + 1);

            if (width <= 0)
            {
                continue;
            }
            
            var offset = width - 1;
            var newCoveredRange = new CoveredRange(item.Sensor.X - offset, item.Sensor.X + offset);

            coverage.Add(newCoveredRange);
        }

        return coverage;
    }
    
    public record Input(Position Sensor, Position Beacon);

    public record CoveredRange(int Start, int End)
    {
        public bool OverlapsWith(CoveredRange other) => Start <= other.End && End >= other.Start;
        public CoveredRange MergeWith(CoveredRange other) => new(Math.Min(Start, other.Start), Math.Max(End, other.End));
    }

    public class Coverage
    {
        private readonly List<CoveredRange> _ranges = new();
        public IReadOnlyList<CoveredRange> Ranges => _ranges;
        
        public void Add(CoveredRange newCoveredRange)
        {
            if (_ranges.Count == 0)
            {
                _ranges.Add(newCoveredRange);
                return;
            }

            for (var index = 0; index < _ranges.Count; index++)
            {
                var item = _ranges[index];
                if (!item.OverlapsWith(newCoveredRange))
                {
                    continue;
                }
                
                item = item.MergeWith(newCoveredRange);
                _ranges[index] = item;
                Simplify(index);
                return;
            }
            
            _ranges.Add(newCoveredRange);
        }
        
        public void RemoveAt(int x)
        {
            for (var index = 0; index < _ranges.Count; index++)
            {
                var range = _ranges[index];
                if (range.Start == x)
                {
                    _ranges[index] = range with { Start = range.Start + 1 };
                } 
                else if (range.End == x)
                {
                    _ranges[index] = range with { End = range.End - 1 };
                } 
                else if (range.Start < x && x < range.End)
                {
                    _ranges[index] = range with { End = x - 1 };
                    _ranges.Insert(index + 1, range with { Start = x + 1 });
                }
            }
        }

        private void Simplify(int startIndex)
        {
            for (var i = startIndex + 1; i < _ranges.Count; i++)
            {
                var previous = _ranges[i - 1];
                var current = _ranges[i];
                
                if (previous.OverlapsWith(current))
                {
                    var newRange = previous.MergeWith(current);
                    _ranges[i - 1] = newRange;
                    _ranges.RemoveAt(i--);
                }
                else
                {
                    break;
                }
            }        
            
            for (var i = startIndex - 1; i >= 0; i--)
            {
                if (_ranges[i].OverlapsWith(_ranges[i + 1]))
                {
                    throw new NotImplementedException("merge");
                }
                else
                {
                    break;
                }
            }
        }
    }
}
