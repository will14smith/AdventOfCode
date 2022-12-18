using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode2016;

[Day]
public partial class Day05 : Day<string, string, string>
{
    protected override string Parse(string input) => input;

    [Sample("abc", "18f47a30")]
    protected override string Part1(string input)
    {
        var i = 0;
        var output = new StringBuilder(8);
        
        var md5 = MD5.Create();

        while (true)
        {
            var code = input + i;
            var codeBytes = Encoding.UTF8.GetBytes(code);
            
            var hashBytes = md5.ComputeHash(codeBytes);
            
            var valid = hashBytes[0] == 0 && hashBytes[1] == 0 && (hashBytes[2] & 0xf0) == 0;
            if (valid)
            {
                var nibble = hashBytes[2] & 0x0f;
                
                output.Append((char)(nibble < 0x0a ? '0' + nibble : 'a' + nibble - 0x0a));
                if (output.Length == 8)
                {
                    break;
                }
            }

            i++;
        }

        return output.ToString();
    }

    [Sample("abc", "05ace8e3")]
    protected override string Part2(string input)
    {
        var i = 0;
        var output = new char[8];
        var filled = 0b00000000;
        
        var md5 = MD5.Create();

        while (filled != 0b11111111)
        {
            var code = input + i;
            var codeBytes = Encoding.UTF8.GetBytes(code);
            
            var hashBytes = md5.ComputeHash(codeBytes);
            
            var valid = hashBytes[0] == 0 && hashBytes[1] == 0 && (hashBytes[2] & 0xf0) == 0;
            if (valid)
            {
                var posNibble = hashBytes[2] & 0x0f;
                if (posNibble < 8 && (filled & (1 << posNibble)) == 0)
                {
                    var charNibble = hashBytes[3] >> 4;
                    var c = (char)(charNibble < 0x0a ? '0' + charNibble : 'a' + charNibble - 0x0a);

                    output[posNibble] = c;
                    filled |= 1 << posNibble;
                }
            }

            i++;
        }

        return new string(output);
    }
}
