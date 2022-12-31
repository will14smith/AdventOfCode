using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace AdventOfCode.Core;

public static class HexExtensions
{
    private static readonly Vector256<byte> Ascii;
    private static readonly Vector256<byte> ShuffleMaskLower;
    private static readonly Vector256<byte> ShuffleMaskUpper;
    private static readonly Vector256<byte> LowerNibbleMask;

    static HexExtensions()
    {
        var ascii128 = Vector128.Create((byte) '0', (byte) '1', (byte) '2', (byte) '3', (byte) '4', (byte) '5', (byte) '6', (byte) '7', (byte) '8', (byte) '9', (byte) 'a', (byte) 'b', (byte) 'c', (byte) 'd', (byte) 'e', (byte) 'f');
        Ascii = Vector256.Create(ascii128, ascii128);

        ShuffleMaskUpper = Vector256.Create(0, 0xFF, 1, 0xFF, 2, 0xFF, 3, 0xFF, 4, 0xFF, 5, 0xFF, 6, 0xFF, 7, 0xFF, 8, 0xFF, 9, 0xFF, 10, 0xFF, 11, 0xFF, 12, 0xFF, 13, 0xFF, 14, 0xFF, 15, 0xFF).AsByte();
        ShuffleMaskLower = Vector256.Create(0xFF, 0, 0xFF, 1, 0xFF, 2, 0xFF, 3, 0xFF, 4, 0xFF, 5, 0xFF, 6, 0xFF, 7, 0xFF, 8, 0xFF, 9, 0xFF, 10, 0xFF, 11, 0xFF, 12, 0xFF, 13, 0xFF, 14, 0xFF, 15).AsByte();
        
        LowerNibbleMask = Vector256.Create((byte)0xF);
    }
    
    public static void Bytes16ToHex32(ReadOnlySpan<byte> input, Span<byte> output)
    {
        var input128 = Vector128.Create(input);
        var vector = Vector256.Create(input128, input128);
        var upper = Avx2.Shuffle(vector, ShuffleMaskUpper);
        var lower = Avx2.Shuffle(vector, ShuffleMaskLower);

        upper = Avx2.ShiftRightLogical(upper.AsInt16(), 4).AsByte();
        lower = Avx2.And(lower, LowerNibbleMask);
        vector = Avx2.Or(upper, lower);
            
        var hexVector = Avx2.Shuffle(Ascii, vector);
        hexVector.CopyTo(output);
    }
}