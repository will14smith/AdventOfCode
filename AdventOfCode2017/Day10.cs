using System.Text;

namespace AdventOfCode2017;

[Day]
public partial class Day10 : Day<Day10.Model, int, string>
{
    protected override Model Parse(string input) => new (input);

    protected override int Part1(Model input)
    {
        var lengths = input.Value.Split(',').Select(byte.Parse).ToList();
        
        var hash = new Hash(256);
        hash.Round(lengths);
        return hash.Values[0] * hash.Values[1];
    }

    protected override string Part2(Model input)
    {
        var newInput = Encoding.ASCII.GetBytes(input.Value).Concat(new byte[] { 17, 31, 73, 47, 23 }).ToArray();

        var hash = new Hash(256);
        hash.Rounds(newInput, 64);

        var dense = new byte[16];
        for (var i = 0; i < 16; i++)
        {
            for (var j = 0; j < 16; j++)
            {
                dense[i] ^= hash.Values[i * 16 + j];
            }
        }

        return Convert.ToHexString(dense).ToLower();
    }

    public record Model(string Value);

    private class Hash
    {
        public byte[] Values { get; }

        private int _skip;
        private int _position;
        
        public Hash(int count)
        {
            Values = Enumerable.Range(0, count).Select(x => (byte)x).ToArray();
        }

        public void Rounds(IReadOnlyList<byte> lengths, int count)
        {
            for (var i = 0; i < count; i++)
            {
                Round(lengths);
            }
        }
        
        public void Round(IReadOnlyList<byte> lengths)
        {
            foreach (var size in lengths)
            {
                for (var i = 0; i < (size + 1) / 2; i++)
                {
                    var a = (_position + i) % Values.Length;
                    var b = (_position + (size - i - 1)) % Values.Length;
                
                    (Values[a], Values[b]) = (Values[b], Values[a]);
                }

                _position += size + _skip++;
            }
        }
    }
}
