using System.Collections.Generic;

namespace Cryptography.Algorithms.Symmetric.CipherStrategy
{
    [SymmetricCipherStrategy(SymmetricCipherMode.CipherFeedback)]
    public class CipherFeedbackStrategy : ICipherStrategy
    {
        public byte[] InitializationVector { get; set; }
        public List<byte[]> Encrypt(ISymmetricCipher symmetricCipher, List<byte[]> openText)
        {
            throw new System.NotImplementedException();
        }

        public List<byte[]> Decrypt(ISymmetricCipher symmetricCipher, List<byte[]> cipherText)
        {
            throw new System.NotImplementedException();
        }
    }
}