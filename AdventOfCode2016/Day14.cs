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
        
        var hashBytes = md5.ComputeHash(inputBytes);
        var hexBuffer = new byte[32];
        for (var x = 0; x < 2016; x++)
        {
            HexExtensions.Bytes16ToHex32(hashBytes, hexBuffer);
            hashBytes = md5.ComputeHash(hexBuffer);
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
