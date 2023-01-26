using System.Collections.Generic;

namespace Cryptography.Algorithms.Symmetric.CipherStrategy;

public interface ICipherStrategy
{
    byte[] InitializationVector { get; set; }

    List<byte[]> Encrypt(ISymmetricCipher symmetricCipher, List<byte[]> openText);
    List<byte[]> Decrypt(ISymmetricCipher symmetricCipher, List<byte[]> cipherText);
}