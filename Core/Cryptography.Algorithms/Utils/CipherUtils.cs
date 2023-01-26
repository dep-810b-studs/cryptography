using System;
using System.Threading.Tasks;

namespace Cryptography.Algorithms.Utils;

public class CipherUtils
{
    public static byte[] XorByteArrays(byte[] firstArray, byte[] secondArray)
    {
        if (firstArray.Length != secondArray.Length)
            throw new ArgumentException("One of array has illegal length");

        var xoredArray = new byte[firstArray.Length];
        Parallel.For(0, xoredArray.Length, index => xoredArray[index] = (byte)(firstArray[index] ^ secondArray[index]));
        return xoredArray;
    }
}