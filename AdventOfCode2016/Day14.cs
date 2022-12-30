using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2016;

[Day]
public partial class Day14 : Day<Day14.Model, int, int>
{
    protected override Model Parse(string input) => new(input);

    private static readonly Regex _3 = new("(000|111|222|333|444|555|666|777|888|999|AAA|BBB|CCC|DDD|EEE|FFF)", RegexOptions.Compiled);

    [Sample("abc", 22728)]
    protected override int Part1(Model input) => Solve(input, HashString);
    
    [Sample("abc", 22551)]
    protected override int Part2(Model input) => Solve(input, HashString2016);

    private static int Solve(Model input, Func<MD5, string, string> hash)
    {
        var md5 = MD5.Create();
        var i = 0;
        var found = 0;

        var buffer = new Buffer(x => hash(md5, x), input.Salt);

        while (true)
        {
            if (IsKey(buffer, i))
            {
                found++;
                if (found == 64)
                {
                    return i;
                }
            }

            i++;
        }
    }

    private static string HashString2016(MD5 md5, string inputString)
    {
        var inputBytes = Encoding.ASCII.GetBytes(inputString);
        
        var ascii128 = Vector128.Create((byte) '0', (byte) '1', (byte) '2', (byte) '3', (byte) '4', (byte) '5', (byte) '6', (byte) '7', (byte) '8', (byte) '9', (byte) 'a', (byte) 'b', (byte) 'c', (byte) 'd', (byte) 'e', (byte) 'f');
        var ascii = Vector256.Create(ascii128, ascii128);

        var spreadMaskUpper = Vector256.Create(0, 0xFF, 1, 0xFF, 2, 0xFF, 3, 0xFF, 4, 0xFF, 5, 0xFF, 6, 0xFF, 7, 0xFF, 8, 0xFF, 9, 0xFF, 10, 0xFF, 11, 0xFF, 12, 0xFF, 13, 0xFF, 14, 0xFF, 15, 0xFF).AsByte();
        var spreadMaskLower = Vector256.Create(0xFF, 0, 0xFF, 1, 0xFF, 2, 0xFF, 3, 0xFF, 4, 0xFF, 5, 0xFF, 6, 0xFF, 7, 0xFF, 8, 0xFF, 9, 0xFF, 10, 0xFF, 11, 0xFF, 12, 0xFF, 13, 0xFF, 14, 0xFF, 15).AsByte();
        
        var lowerMask = Vector256.Create((byte)0xF);
        
        var hashBytes = md5.ComputeHash(inputBytes);
        var hexBuffer2 = new byte[32];
        for (var x = 0; x < 2016; x++)
        {
            var hashVector128 = Vector128.Create(hashBytes);
            var hashVector = Vector256.Create(hashVector128, hashVector128);
            var hashVectorUpper = Avx2.Shuffle(hashVector, spreadMaskUpper);
            var hashVectorLower = Avx2.Shuffle(hashVector, spreadMaskLower);

            hashVectorUpper = Avx2.ShiftRightLogical(hashVectorUpper.AsInt16(), 4).AsByte();
            hashVectorLower = Avx2.And(hashVectorLower, lowerMask);
            hashVector = Avx2.Or(hashVectorUpper, hashVectorLower);
            
            var hexVector = Avx2.Shuffle(ascii, hashVector);
            hexVector.CopyTo(hexBuffer2);
            
            hashBytes = md5.ComputeHash(hexBuffer2);
        }
        
        return Convert.ToHexString(hashBytes);
    }

    private static string HashString(HashAlgorithm md5, string inputString)
    {
        var inputBytes = Encoding.ASCII.GetBytes(inputString);

        var hashBytes = md5.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes);
    }


    private static bool IsKey(Buffer buffer, int i)
    {
        var hashString = buffer.Hash(i);
        if (!_3.IsMatch(hashString)) return false;
        
        var c = _3.Match(hashString).Captures[0].Value[0];
        var m = $"{c}{c}{c}{c}{c}";

        for (var x = 1; x < 1001; x++)
        {
            var otherHashString = buffer.Hash(i + x);
            if (otherHashString.Contains(m))
            {
                return true;
            }
        }

        return false;
    }

    public record Model(string Salt);

    private class Buffer
    {
        private readonly Func<string, string> _hash;
        private readonly string _salt;

        private readonly Dictionary<int, string> _cache = new();

        public Buffer(Func<string, string> hash, string salt)
        {
            _hash = hash;
            _salt = salt;
        }

        public string Hash(int i)
        {
            if (_cache.TryGetValue(i, out var existing))
            {
                return existing;
            }
            
            var hash = _hash(_salt + i);
            _cache[i] = hash;
            return hash;
        }
    }
}
