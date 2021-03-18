using System;

namespace Cryptography.Algorithms
{
    public interface ICipher
    {
        byte[] EnCrypt(byte[] openText);
        byte[] DeCrypt(byte[] encryptedText);
    }
}