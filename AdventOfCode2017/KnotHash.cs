namespace AdventOfCode2017;

internal class KnotHash
{
    public byte[] Values { get; }

    private int _skip;
    private int _position;
        
    public KnotHash(int count)
    {
        Values = Enumerable.Range(0, count).Select(x => (byte)x).ToArray();
    }

    public static byte[] Standard(byte[] input)
    {
        var hash = new KnotHash(256);
        hash.Rounds(input.Concat(new byte[] { 17, 31, 73, 47, 23 }).ToArray(), 64);
        return hash.Output();
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

    public byte[] Output()
    {
        var hashSize = Values.Length / 16;

        var dense = new byte[hashSize];
        for (var i = 0; i < hashSize; i++)
        {
            for (var j = 0; j < 16; j++)
            {
                dense[i] ^= Values[i * 16 + j];
            }
        }

        return dense;
    }
}