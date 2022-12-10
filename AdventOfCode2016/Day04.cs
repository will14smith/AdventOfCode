using System.Text;
using Superpower;
using Superpower.Parsers;

namespace AdventOfCode2016;

[Day]
public partial class Day04 : ParseLineDay<Day04.Model, int, int>
{
    public record Model(IReadOnlyList<string> Name, string Checksum, int SectorId);

    private static readonly TextParser<string> NameSegmentParser = Span.Regex("[a-z]+").Select(x => x.ToStringValue());
    private static readonly TextParser<string[]> NameParser = NameSegmentParser.Try().ManyDelimitedBy(Character.EqualTo('-'));
    private static readonly TextParser<int> Sector = Numerics.IntegerInt32;
    private static readonly TextParser<string> Checksum = Character.EqualTo('[').IgnoreThen(NameSegmentParser).ThenIgnore(Character.EqualTo(']'));

    protected override TextParser<Model> LineParser => NameParser.ThenIgnore(Character.EqualTo('-')).Then(Sector).Then(Checksum).Select(x => new Model(x.Item1.Item1, x.Item2, x.Item1.Item2));

    [Sample("aaaaa-bbb-z-y-x-123[abxyz]", 123)]
    [Sample("a-b-c-d-e-f-g-h-987[abcde]", 987)]
    [Sample("not-a-real-room-404[oarel]", 404)]
    [Sample("totally-real-room-200[decoy]", 0)]
    protected override int Part1(IEnumerable<Model> input) => input.Where(IsReal).Sum(x => x.SectorId);
    
    protected override int Part2(IEnumerable<Model> input)
    {
        var decoded = input.Select(Decode).ToList();

        return decoded.First(x => x.Name.Contains("north")).Sector;
    }

    private bool IsReal(Model room)
    {
        var letterFrequencies = room.Name.SelectMany(x => x).GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
        var orderedFrequencies = letterFrequencies.OrderByDescending(x => x.Value).ThenBy(x => x.Key);
        var expectedChecksum = orderedFrequencies.Select(x => x.Key).Take(5).Join();

        return room.Checksum == expectedChecksum;
    }
    
    
    private (string Name, int Sector) Decode(Model room)
    {
        var sb = new StringBuilder();

        foreach (var segment in room.Name)
        {
            foreach (var c in segment)
            {
                sb.Append((char)((c - 'a' + room.SectorId) % 26 + 'a'));
            }

            sb.Append(' ');
        }

        return (sb.ToString(), room.SectorId);
    }
}
