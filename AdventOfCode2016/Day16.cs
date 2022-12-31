using System.Text;

namespace AdventOfCode2016;

[Day]
public partial class Day16 : Day<Day16.Model, string, string>
{
    private const string Sample = "10000";
    
    protected override Model Parse(string input) => new(input);

    [Sample(Sample, "01100")]
    protected override string Part1(Model input) => CalculateChecksum(BuildData(input.Seed, input.Seed == Sample ? 20 : 272));
    protected override string Part2(Model input) => CalculateChecksum(BuildData(input.Seed, 35651584));

    private static string BuildData(string data, int length)
    {
        while (data.Length < length)
        {
            var b = string.Create(data.Length, data, (x, a) =>
            {
                var c = a.AsSpan();
                var cLength = c.Length;
                
                for (var i = 0; i < cLength; i++)
                {
                    x[i] = (char)(c[cLength - (i + 1)] ^ 1);
                }
            });

            data = data + '0' + b;
        }

        return data[..length];
    }

    private static string CalculateChecksum(string data)
    {
        var checksumLength = data.Length;
        var checksumDigitLength = 1;

        while ((checksumLength & 1) == 0)
        {
            checksumLength >>= 1;
            checksumDigitLength <<= 1;
        }

        var chars = data.AsSpan();
        
        var sb = new StringBuilder(checksumLength);
        var offset = 0;
        for (var i = 0; i < checksumLength; i++)
        {
            sb.Append(CalculateChecksumDigit(chars[offset..(offset + checksumDigitLength)]));
            offset += checksumDigitLength;
        }
        
        return sb.ToString();
    }

    private static char CalculateChecksumDigit(ReadOnlySpan<char> data)
    {
        var numberOf1 = 0;
        
        for (var i = 0; i < data.Length; i++)
        {
            numberOf1 += data[i] & 1;
        }
        
        // should have even number of 1s is pairs are matching
        return (numberOf1 & 1) == 0 ? '1' : '0';
    }

    public record Model(string Seed);
}
