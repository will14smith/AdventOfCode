using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode2015;

[Day]
public partial class Day04 : Day<string, int, int>
{
    protected override string Parse(string input) => input;

    [Sample("abcdef", 609043)]
    [Sample("pqrstuv", 1048970)]
    protected override int Part1(string input)
    {
        var i = 0;
        var md5 = MD5.Create();

        while (true)
        {
            var code = input + i;
            var codeBytes = Encoding.UTF8.GetBytes(code);
            
            var hashBytes = md5.ComputeHash(codeBytes);
            
            var valid = hashBytes[0] == 0 && hashBytes[1] == 0 && (hashBytes[2] & 0xf0) == 0;
            if (valid)
            {
                return i;
            }

            i++;
        }
    }

    [Sample("abcdef", 6742839)]
    [Sample("pqrstuv", 5714438)]
    protected override int Part2(string input)
    {
        var i = 0;
        var md5 = MD5.Create();

        while (true)
        {
            var code = input + i;
            var codeBytes = Encoding.UTF8.GetBytes(code);
            
            var hashBytes = md5.ComputeHash(codeBytes);
            
            var valid = hashBytes[0] == 0 && hashBytes[1] == 0 && hashBytes[2] == 0;
            if (valid)
            {
                return i;
            }

            i++;
        }
    }
}